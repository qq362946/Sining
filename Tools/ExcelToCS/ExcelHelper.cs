using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;

namespace ExcelToCS
{
    public static class ExcelHelper
    {
        static ExcelHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static DataSet LoadExcel(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return ExcelReaderFactory.CreateOpenXmlReader(stream).AsDataSet();
        }
    }
}