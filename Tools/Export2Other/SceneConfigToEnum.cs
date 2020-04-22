using System;
using System.Data;
using System.IO;
using System.Text;
using ExcelToCS;

namespace Export2Other
{
    public static class SceneConfigToEnum
    {
        private const string ConfigFile = "../../../../../Excel/Server/ServerConfig.xlsx";
        private const string SaveConfigFile = "../../../../../Server/Model/Base/Module/Scene/SceneType.cs";

        public static void Run()
        {
            if (!File.Exists(ConfigFile))
            {
                throw new Exception("ServerConfig.xlsx not found");
            }

            using var br = new BinaryReader(new FileStream(ConfigFile, FileMode.Open, FileAccess.Read));

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("namespace Sining\n{");
            strBuilder.AppendLine("\t// 生成器自动生成，请不要手动编辑。");
            strBuilder.AppendLine("\tpublic enum SceneType\n\t{");
            strBuilder.AppendLine("\t\tNone = 0,");
            
            foreach (DataTable table in ExcelHelper.LoadExcel(ConfigFile).Tables)
            {
                if (table.TableName != "SceneConfig") continue;

                for (var i = 4; i < table.Rows.Count; i++)
                {
                    strBuilder.AppendLine($"\t\t{table.Rows[i][2]} = {i - 3},");
                }
                
                break;
            }
            
            strBuilder.AppendLine("\t}\n}");
            using var cs = new StreamWriter(SaveConfigFile);
            cs.WriteAsync(strBuilder.ToString());
        }
    }
}