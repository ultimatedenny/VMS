using LinqToExcel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace VMS.Library
{
    public static class SysUtil
    {
        static string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        #region DATABASE EXECUTE RESULT
        public static VMSRes<string> NonQueryResults(this long NonQuery, string msg = null)
        {
            return new VMSRes<string>()
            {
                Success = (NonQuery > 0) ? true : false,
                Message = (NonQuery > 0) ? $"Success! {NonQuery} row(s) affected" : "There is No Data been Updated/Inserted",
                data = (!string.IsNullOrEmpty(msg)) ? msg : ""
            };
        }
        public static bool dtHavingRow(this DataTable dt)
        {
            return (dt.Rows.Count > 0) ? true : false;
        }
        public static string dtHavingRowMsg(this DataTable dt)
        {
            return (dt.Rows.Count > 0) ? $"Data Has {dt.Rows.Count} row(s)" : "Data Not Found";
        }
        public static VMSRes<string> ScalarResult(this string NonQuery)
        {
            return new VMSRes<string>()
            {
                Success = (NonQuery != "") ? true : false,
                Message = (NonQuery != "") ? "" : "There is No Data been Retrieve",
                data = NonQuery
            };

        }
        public static VMSRes<string> OneRowdtScalarResult(this string NonQuery)
        {
            if (NonQuery.IndexOf('%') == 0)
            {
                return new VMSRes<string>
                {
                    Success = false,
                    Message = "No Data",
                    data = null
                };
            }
            List<string> sWordSplit = NonQuery.Split('%').ToList();
            return new VMSRes<string>
            {
                Success = sWordSplit[0].ToBoolean(),
                Message = sWordSplit[1],
                data = sWordSplit[1],
            };
        }

        #endregion
        public static string ConvertDateFormat(this string Date, string dateformatBefore, string dateFormatResult)
        {
            DateTime date = Convert.ToDateTime(Date);
            return date.ToString(dateFormatResult);
        }
        public static string StandardSQLDateS(this string Date, string dateformatBefore)
        {
            DateTime date = Convert.ToDateTime(Date);
            return date.ToString("yyyy-MM-dd");
        }
        public static string StandardSQLDateD(this DateTime Date)
        {
            return Date.ToString("yyyy-MM-dd");
        }
        public static void Converts<T>(this DataColumn column, Func<object, T> conversion)
        {
            foreach (DataRow row in column.Table.Rows)
            {
                row[column] = conversion(row[column]);
            }
        }
        public static DataTable ConvertStdDT(this DataTable dt)
        {
            List<string> columnDate = new List<string>();
            DataTable dtClone = dt.Clone();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.DataType == Type.GetType("System.DateTime"))
                {
                    dtClone.Columns[dc.ColumnName].DataType = typeof(string);
                    columnDate.Add(dc.ColumnName);
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                dtClone.ImportRow(row);
            }
            foreach (var col in columnDate)
            {
                dtClone.Columns[col].Converts(val => DateTime.Parse(val.ToString()).ToString("yyyy-MM-yy"));
            }
            return dtClone;
        }
        public static string Encrypt(this string clearText, string EncryptionKey = "TEAMSHIMANO")
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(this string cipherText, string EncryptionKey = "TEAMSHIMANO")
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch
            {
                return "null";
            }

        }
        public static VMSRes<string> PassingError(this string message)
        {
            return new VMSRes<string>()
            {
                Message = message,
                Success = false,
                data = null
            };
        }
        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }
        public static List<string> SentencesToArray(this string text, bool toSentences, int length)
        {
            List<string> txtArray = text.Split(' ').ToList();
            if (toSentences)
            {
                List<string> sentencesArray = new List<string>();
                string sentences = "";
                for (int i = 0; i < txtArray.Count; i++)
                {
                    if (sentences.Length + txtArray[i].Length > length)
                    {
                        sentencesArray.Add(sentences);
                        sentences = "";
                    }
                    sentences += txtArray[i] + " ";
                }
                sentencesArray.Add(sentences);
                return sentencesArray;
            }
            return txtArray;
        }
        public static bool isSystemConnected()
        {
            bool con = true;
            using (var sql = new MSSQL())
            {
                con = sql.IsSystemConnected(ConnectionDB, false);
            }
            return con;
        }
        public static bool IsBetweenII<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return (min.CompareTo(value) <= 0) && (value.CompareTo(max) <= 0);
        }
        public static bool IsBetweenEI<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return (min.CompareTo(value) < 0) && (value.CompareTo(max) <= 0);
        }
        public static bool IsBetweenIE<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return (min.CompareTo(value) <= 0) && (value.CompareTo(max) < 0);
        }
        public static bool IsBetweenEE<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return (min.CompareTo(value) < 0) && (value.CompareTo(max) < 0);
        }
        public static bool ToBoolean<T>(this T data)
        {
            bool result = false;
            var str = data.ToString().ToUpper();
            if (str == "TRUE" || str == "1" || str == "SUCCESS")
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// to Return in dt.Rows[0][1].ToString as message feedback
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static VMSRes<string> OneRowdtToResult(this DataTable dt, string data = "")
        {
            return new VMSRes<string>
            {
                Message = dt.Rows[0][1].ToString(),
                Success = dt.Rows[0][0].ToBoolean(),
                data = data
            };
        }
        public static bool MoveFileToInternalFolderASP(string fileName, string FromDirectory, string ToDirectory)
        {
            string sourceFile = System.IO.Path.Combine(FromDirectory, fileName);
            string destFile = System.IO.Path.Combine(ToDirectory, fileName);
            if (System.IO.Directory.Exists(FromDirectory))
            {
                string[] files = System.IO.Directory.GetFiles(FromDirectory);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(ToDirectory, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            return true;
        }
        public static DataTable ConvertToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;

        }
        public static DataTable GetTableFromExcel(string PathFile,string FileName, string SheetName)
        {
            List<string> queries = new List<string>();
            var connectionStringExcel =
                (FileName.EndsWith(".xls")
                ? string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", PathFile)
                : string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", PathFile));
            var adapter = new OleDbDataAdapter($"SELECT * FROM [{SheetName}$]", connectionStringExcel);
            CultureInfo myCultureInfo = new CultureInfo("en-gb");
            var ds = new DataSet();
            adapter.Fill(ds, SheetName);
            ds.Locale = myCultureInfo;
            DataTable table = new DataTable();
            table = ds.Tables[SheetName];
            table.Locale = myCultureInfo;
            return table.ConvertStdDT();
        }
        public static List<T> GetObjectFromExcel<T>(string PathFile, string SheetName)
        {
            var excelFile = new ExcelQueryFactory(PathFile);
            var objects = (from a in excelFile.Worksheet<T>(SheetName)
                           select a).ToList();
            return objects;
        }
        public static string IsStringNull<T>(this T text, params T[] items)
        {
            if (text == null)
            {
                return "";
            }
            if (string.IsNullOrWhiteSpace(text.ToString()))
            {
                return "";
            }
            else
            {
                if (items == null)
                {
                    return text.ToString();
                }
                else
                {
                    string realtext = text.ToString();
                    foreach (var item in items)
                    {
                        realtext = realtext + item.ToString();
                    }
                    return realtext;
                }
            }

        }
        public static string DateStd(this string Date)
        {

            try
            {
                if (string.IsNullOrEmpty(Date))
                {
                    return Date;
                }
                DateTime date = Convert.ToDateTime(Date);
                return date.ToString("yyyy-MM-dd");
            }
            catch
            {
                return Date;
            }

        }
    }

    public class VMSRes<T>
    {
        public T data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class auSystemLog
    {
        public string ComputerName { get; set; } = SysUtil.DetermineCompName(HttpContext.Current.Request.UserHostName);
        public string ActionName { get; set; } = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
        public string ComputerIP { get; set; } = HttpContext.Current.Request.UserHostName;
        public string UserID { get; set; } = HttpContext.Current.Request.Cookies["UserID"] != null ? HttpContext.Current.Request.Cookies["UserID"].Value.Decrypt() : "NO USER";
        public string Query { get; set; }
        public string SystemMessage { get; set; }
    }
}
