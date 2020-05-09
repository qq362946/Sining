using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Sining.Tools;

namespace Sining.ProtoBufTool
{
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;

        public OpcodeInfo(string name, int opcode)
        {
            Name = name;
            Opcode = opcode;
        }
    }

    internal static class Program
    {
        private const string OuterMessageName = "OuterMessage.proto";
        private const string InnerMessageName = "InnerMessage.proto";
        private const string OuterOpcodeName = "OuterOpcode";
        private const string OuterOpcodeCsName = OuterOpcodeName + ".cs";
        private const string InnerOpcodeName = "InnerOpcode";
        private const string InnerOpcodeCsName = InnerOpcodeName + ".cs";
        private const string ProtoBufPath = "../../../../../ProtoBuf/";
        private const string ServerPath = "../Server/Model/Base/Module/Message/";

        private static readonly List<OpcodeInfo> Opcodes = new List<OpcodeInfo>();

        private static readonly char[] SplitChars = {' ', '\t'};

        private static void Main(string[] args)
        {
            string protoToolName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                protoToolName = "protoc.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                protoToolName = "protoc";
            }
            else
            {
                Console.WriteLine("ProtoBufTool不支持当前系统!");

                return;
            }

            var startOpcode = 100;

            ProcessHelper.Run(
                Path.Combine(ProtoBufPath, protoToolName),
                $"--proto_path=./ {OuterMessageName} --csharp_out={ServerPath}",
                ProtoBufPath, true);

            Opcode(ref startOpcode, OuterMessageName, OuterOpcodeName, OuterOpcodeCsName);

            InnerMessage();
            // ProcessHelper.Run(
            //     Path.Combine(ProtoBufPath, protoToolName),
            //     $"--proto_path=./ {InnerMessageName} --csharp_out={ServerPath}",
            //     ProtoBufPath, true);

            Opcode(ref startOpcode, InnerMessageName, InnerOpcodeName, InnerOpcodeCsName);

            Console.WriteLine("proto2cs succeed!");
        }

        private static void InnerMessage()
        {
            var proto = Path.Combine(ProtoBufPath, InnerMessageName);
            
            var protoFile = File.ReadAllText(proto);
            
            var file = new StringBuilder();
            file.Append($"using Sining.Module;\n");
            file.Append($"using Sining.Network;\n\n");
            file.Append($"namespace Sining.Message\n");
            file.Append("{\n");
            
            var isMsgStart = false;
            
            foreach (var line in protoFile.Split('\n'))
            {
                var newline = line.Trim();
                
                if (newline == "") continue;
                
                if (newline.StartsWith("//"))
                {
                    file.Append($"\t{newline}\n");

                    continue;
                }

                if (newline.StartsWith("message"))
                {
                    isMsgStart = true;
                    var className = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    var parameter = line.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries)[1];
                    var interfaceName = parameter.Trim();

                    file.Append($"\tpublic partial class {className} ");
                    file.Append($": {interfaceName} ");
                    continue;
                }

                if (!isMsgStart) continue;
                
                switch (newline)
                {
                    case "{":
                        file.Append("\n\t{\n");
                        continue;
                    case "}":
                        isMsgStart = false;
                        file.Append("\t}\n");
                        continue;
                }

                if (newline.Trim().StartsWith("//"))
                {
                    file.AppendLine(newline);
                    continue;
                }

                if (newline.Trim() == "" || newline == "}") continue;
                
                if (newline.StartsWith("repeated"))
                {
                    Repeated(file,newline);
                }
                else
                {
                    Members(file, newline, true);
                }
            }
            
            file.Append("}\n");
            
            using var sw = new StreamWriter($"../../../../{ServerPath}InnerMessage.cs");

            sw.Write(file.ToString());
        }
        
        private static void Repeated(StringBuilder sb,string newline)
        {
            try
            {
                var index = newline.IndexOf(";", StringComparison.Ordinal);
                newline = newline.Remove(index);
                var ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                var type = ss[1];
                type = ConvertType(type);
                var name = ss[2];

                sb.Append($"\t\tpublic List<{type}> {name} = new List<{type}>();\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }

        private static void Members(StringBuilder sb, string newline, bool isRequired)
        {
            try
            {
                var index = newline.IndexOf(";", StringComparison.Ordinal);
                newline = newline.Remove(index);
                var ss = newline.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                var type = ss[0];
                var name = ss[1];
                var typeCs = ConvertType(type);

                sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{newline}\n {e}");
            }
        }
        private static void Opcode(ref int opcodeIndex, string messageName, string opcodeName, string opcodeCsName)
        {
            using var protoFile = new StreamReader(Path.Combine(ProtoBufPath, messageName));

            var file = new StringBuilder();

            file.Append("using Sining.Network;\n\n");
            file.Append("namespace Sining.Message\n");
            file.Append("{\n");

            for (;;)
            {
                var line = protoFile.ReadLine();

                if (line == null) break;

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("//"))
                {
                    file.Append($"\t{line}\n");
                    continue;
                }

                if (!line.StartsWith("message")) continue;

                var className = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                var parameter = line.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries)[1];
                var interfaceName = parameter.Trim();

                file.Append($"\t[Message({opcodeName}.{className})]\n");

                file.Append($"\tpublic partial class {className} ");
                file.Append($": {interfaceName} ");
                file.Append("{}\n\n");
                Opcodes.Add(new OpcodeInfo(className, ++opcodeIndex));
            }

            file.Append("}\n");

            GenerateOpcode(file, opcodeName);

            using var sw = new StreamWriter($"../../../../{ServerPath}{opcodeCsName}");

            sw.Write(file.ToString());
        }

        private static void GenerateOpcode(StringBuilder file, string opcodeName)
        {
            file.AppendLine("namespace Sining.Message");
            file.AppendLine("{");
            file.AppendLine($"\tpublic static partial class {opcodeName}");
            file.AppendLine("\t{");
            foreach (var info in Opcodes)
            {
                file.AppendLine($"\t\t public const ushort {info.Name} = {info.Opcode};");
            }

            Opcodes.Clear();
            file.AppendLine("\t}");
            file.AppendLine("}");
        }
        
        private static string ConvertType(string type)
        {
            return type switch
            {
                "int[]" => "int[] { }",
                "int32[]" => "int[] { }",
                "int64[]" => "long[] { }",
                "int32" => "int",
                "int64" => "long",
                _ => type
            };
        }
    }
}