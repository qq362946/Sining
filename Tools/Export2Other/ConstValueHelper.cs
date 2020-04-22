using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelToCS;

namespace Export2Other
{
    public static class ConstValueHelper
    {
        private const string ConfigFile = "../../../../../Excel/#ConstValue.xlsx";
        private const string SaveConfigFile = "../../../../../Server/Model/Base/ConstValue.cs";
        
        public static void Run()
        {
            if (!File.Exists(ConfigFile))
            {
                throw new Exception("ConstValue.csv not found");
            }
            
            using var br = new BinaryReader(new FileStream(ConfigFile, FileMode.Open, FileAccess.Read));

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("namespace Sining\n{");
            strBuilder.AppendLine("\t// 生成器自动生成，请不要手动编辑。");
            strBuilder.AppendLine("\tpublic class ConstValue\n\t{");
            
            foreach (DataTable table in ExcelHelper.LoadExcel(ConfigFile).Tables)
            {
                if (table.TableName != "#ConstValue") continue;

                for (var i = 3; i < table.Rows.Count; i++)
                {
                    var typeStr = table.Rows[i][2].ToString();

                    strBuilder.AppendLine(
                        $"\t\tpublic const {typeStr} {table.Rows[i][1]} = {DefaultValue(typeStr, table.Rows[i][3].ToString())};//{table.Rows[i][4]}");
                }

                break;
            }
            
            strBuilder.AppendLine("\t}\n}");
            using var cs = new StreamWriter(SaveConfigFile);
            cs.WriteAsync(strBuilder.ToString());
        }
        
        private static string DefaultValue(string type,string value)
        {
            return type switch
            {
                "int[]" => $"new int[] {{ {value}}}",
                "int32[]" => $"new int[] {{ {value}}}",
                "long[]" => $"new long[] {{ {value}}}",
                "string[]" => $"new string[] {{ {value}}}",
                "double[]" => $"new double[] {{ {value}}}",
                "float[]" => $"new float[] {{ {value}}}",
                "int" => $"{value}",
                "bool" => $"{value}",
                "uint" => $"{value}",
                "int32" => $"{value}",
                "int64" => $"{value}",
                "long" => $"{value}",
                "float" => $"{value}",
                "double" => $"{value}",
                "string" => $"\"{value}\"",
                _ => throw new Exception($"不支持此类型: {type}")
            };
        }
    }
}