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

            ProcessHelper.Run(
                Path.Combine(ProtoBufPath, protoToolName),
                $"--proto_path=./ {InnerMessageName} --csharp_out={ServerPath}",
                ProtoBufPath, true);

            Opcode(ref startOpcode, InnerMessageName, InnerOpcodeName, InnerOpcodeCsName);

            Console.WriteLine("proto2cs succeed!");
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
                var parameter = line.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries)[1]
                    .Split('|', StringSplitOptions.RemoveEmptyEntries);
                var interfaceName = parameter[0].Trim();

                if (parameter.Length == 1)
                {
                    file.Append($"\t[Message({opcodeName}.{className})]\n");
                }
                else
                {
                    var rawUrl = parameter[1].Trim();
                    file.Append($"\t[Message({opcodeName}.{className},\"{rawUrl}{className}\")]\n");
                }

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
    }
}