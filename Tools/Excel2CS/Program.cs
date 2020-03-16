using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Sining.Config;
using Sining.Tools;

namespace ExcelToCS
{
    internal static class Program
    {
        private const string ExcelDirectory = "../../../../../Excel/";
        public const string ConfigDirectory = "../../../../../Config/";
        public const string CsFileDirectory = "../../../../../Server/Model/Base/Module/Config/";
        private static Assembly __assembly;

        private static readonly ConcurrentQueue<(DataTable, Dictionary<string, string>)>
            Queue = new ConcurrentQueue<(DataTable, Dictionary<string, string>)>();

        private static void Main(string[] args)
        {
            // 1、把Excel文件转换成cs实体类。
            
            ExportToCs(ExcelDirectory);
           
            // 2、反射出转换出的cs实体类。
            
            __assembly = LoadCsFile.Load();
            
            // 3、把Excel导出二进制数据。
            
            ToBinaryFile();
            
            Console.WriteLine("export succeeded!");
        }
        private static void ExportToCs(string filePath)
        {
            foreach (var file in Directory.GetFiles(filePath).Where(d => Path.GetExtension(d) == ".xlsx"))
            {
                var excel = ExcelHelper.LoadExcel(file);

                foreach (DataTable excelTable in excel.Tables)
                {
                    if (excelTable.TableName.StartsWith("#") ||
                        excelTable.Rows.Count < 5 || excelTable.Columns.Count == 0) continue;
                    
                    ToCsFile(excelTable);
                }
            }

            foreach (var directory in Directory.GetDirectories(filePath))
            {
                ExportToCs(directory);
            }
        }
        private static void ToBinaryFile()
        {
            foreach (var type in __assembly.GetTypes())
            {
                BsonClassMap.LookupClassMap(type);
            }

            while (Queue.TryDequeue(out var table))
            {
                var currentTable = table.Item1;
                var currentCols = table.Item2;

                var binaryFileClass = new Dictionary<string, IConfig>();

                for (var row = 4; row < currentTable.Rows.Count; row++)
                {
                    var config = (IConfig) Activator.CreateInstance(
                        __assembly.GetType($"Sining.Config.{currentTable.TableName}"));
                    var ts = config.GetType();

                    var currentRow = currentTable.Rows[row];

                    var index = 0;
                    foreach (var (key, value) in currentCols)
                    {
                        ts.GetProperty(key).SetNewValue(config, value, currentRow[index + 1].ToString());
                        index++;
                    }

                    binaryFileClass.Add(config.Id.ToString(), config);
                }

                using var br = new FileStream(
                    Path.Combine(ConfigDirectory, $"{currentTable.TableName}.byte"),
                    FileMode.Create);
                br.Write(binaryFileClass.ToBsonDocument().SerializeToByte());
            }
        }
        private static async void ToDataFile(DataTable excelTable)
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine(TemplateConst.ConfigDataTemplateHead);

            strBuilder.AppendLine(TemplateConst.ConfigCsCategoryAttribute);
            strBuilder.AppendLine(string.Format(TemplateConst.ConfigCsCategoryClassName, excelTable.TableName));
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine(string.Format(TemplateConst.ConfigCsCategoryInstanceName, excelTable.TableName));
            strBuilder.AppendLine("\t}\n}");

            await using var cs =
                new StreamWriter(Path.Combine($"{CsFileDirectory}Data/", $"{excelTable.TableName}Data.cs"));
            await cs.WriteAsync(strBuilder.ToString());
        }
        private static async void ToCsFile(DataTable excelTable)
        {
            ToDataFile(excelTable);

            var strBuilder = new StringBuilder();

            strBuilder.Clear();
            strBuilder.AppendLine(TemplateConst.ConfigCsTemplateHead);
            strBuilder.AppendLine(string.Format(TemplateConst.ConfigCsClassName, excelTable.TableName));
            strBuilder.AppendLine(TemplateConst.ConfigCsClassId);

            var headRow = excelTable.Rows[2];
            var headTypeRow = excelTable.Rows[3];

            var colDictionary = new Dictionary<string, string>();

            for (var i = 1; i < excelTable.Columns.Count; i++)
            {
                if (headRow[i].ToString().StartsWith("#")) continue;

                var headTypeValue = headTypeRow[i].ToString();

                if (headRow[i].ToString() != "Id")
                {
                    if (headTypeValue == "float")
                    {
                        strBuilder.AppendLine(TemplateConst.FieldByFloat);
                    }

                    strBuilder.Append(string.Format(TemplateConst.FieldByBsonDefaultValue,
                        DefaultValue(headTypeValue)));

                    strBuilder.Append(
                        string.Format(
                            TemplateConst.FieldByPublic,
                            headTypeValue,
                            headRow[i]));
                }

                colDictionary.Add(headRow[i].ToString(), headTypeValue);
            }

            Queue.Enqueue((excelTable, colDictionary));

            strBuilder.AppendLine("\t}\n}");

            await using var cs =
                new StreamWriter(Path.Combine($"{CsFileDirectory}Entity/", $"{excelTable.TableName}.cs"));
            await cs.WriteAsync(strBuilder.ToString());
        }
        private static void SetNewValue(this PropertyInfo propertyInfo,IConfig config, string type, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            switch (type)
            {
                case "int":
                    propertyInfo.SetValue(config, Convert.ToInt32(value));
                    break;
                case "string":
                    propertyInfo.SetValue(config, value);
                    break;
                case "bool":
                    propertyInfo.SetValue(config, Convert.ToBoolean(value));
                    break;
                case "long":
                    propertyInfo.SetValue(config, Convert.ToInt64(value));
                    break;
                case "double":
                    propertyInfo.SetValue(config, Convert.ToDouble(value));
                    break;
                case "float":
                    propertyInfo.SetValue(config, Convert.ToSingle(value));
                    break;
                case "int32[]":
                case "int[]":
                    propertyInfo.SetValue(config,
                        value.Split(",").Select(d => Convert.ToInt32(d)).ToArray());
                    break;
                case "long[]":
                    propertyInfo.SetValue(config,
                        value.Split(",").Select(d => Convert.ToInt64(d)).ToArray());
                    break;
                case "double[]":
                    propertyInfo.SetValue(config,
                        value.Split(",").Select(d => Convert.ToDouble(d)).ToArray());
                    break;
                case "string[]":
                    propertyInfo.SetValue(config, value.Split(",").ToArray());
                    break;
                case "float[]":
                    propertyInfo.SetValue(config,
                        value.Split(",").Select(d => Convert.ToSingle(d)).ToArray());
                    break;
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }
        private static string DefaultValue(string type)
        {
            return type switch
            {
                "int[]" => "new int[] { }",
                "int32[]" => "new int[] { }",
                "long[]" => "new long[] { }",
                "string[]" => "new string[] { }",
                "double[]" => "new double[] { }",
                "float[]" => "new float[] { }",
                "int" => "0",
                "bool" => "false",
                "uint" => "0",
                "int32" => "0",
                "int64" => "0",
                "long" => "0L",
                "float" => "0f",
                "double" => "0.0",
                "string" => "\"\"",
                _ => throw new Exception($"不支持此类型: {type}")
            };
        }
    }
}