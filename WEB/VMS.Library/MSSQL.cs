using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Library
{
    public class MSSQL : IDisposable
    {

        static string ConnectionDB = ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        bool disposed = false;
        public SqlConnection SqlConHd = new SqlConnection();
        /// <summary>
        /// Open Connection to Database
        /// </summary>
        /// <param name="ConStr">Connection string</param>
        /// <returns>true or false</returns>
        public bool SQLCon(string ConStr)
        {
            SqlConHd.ConnectionString = ConStr;
            try
            {
                SqlConHd.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SQLCloseCon()
        {
            SqlConnection.ClearPool(SqlConHd);
            SqlConHd.Dispose();
            SqlConHd.Close();
        }

        /// <summary>
        /// Function to call query into sql server database and return DATATABLE format
        /// </summary>
        /// <param name="Connection">Connection string</param>
        /// <param name="command">SQL Command to be execute</param>
        /// <param name="parameters">SQL Parameters Query to prevent SQL Injection</param>
        /// <param name="Tranx">Indicate if Transaction exist; NULL if non-transcational</param>
        /// <param name="_persist">FALSE means will close Connection immediately after query</param>
        /// <returns></returns>
        public DataTable ExecDTQuery(string Connection, string command, List<ctSqlVariable> parameters, SqlTransaction Tranx, bool _persist)
        {
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }
                SqlCommand Selectcommand = new SqlCommand(command, SqlConHd);
                DataSet ds = new DataSet();
                SqlDataAdapter DataAd = new SqlDataAdapter();
                DataAd.SelectCommand = Selectcommand;
                if (parameters != null)
                {
                    foreach (var row in parameters)
                    {
                        Selectcommand.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                }

                if (Tranx != null)
                {
                    DataAd.SelectCommand.Transaction = Tranx;
                }
                DataAd.Fill(ds);
                if ((ds != null) && (ds.Tables.Count > 0))
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                if (Tranx != null)
                {
                    Tranx.Rollback();
                }
               throw ex;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }
            }
        }
        /// <summary>
        /// Function to call query into sql server database and return DATASET format
        /// </summary>
        /// <param name="StrCon">Connection string</param>
        /// <param name="SqlCmd">SQL Command to be execute</param>
        /// <param name="parameters">SQL Parameters Query to prevent SQL Injection</param>
        /// <param name="Tranx">Indicate if Transaction exist; NULL if non-transcational</param>
        /// <param name="_persist">FALSE means will close Connection immediately after query</param>
        /// <returns></returns>
        public DataSet ExecDSQuery(string Connection, string command, List<ctSqlVariable> parameters, SqlTransaction Tranx, bool _persist)
        {
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }

                SqlCommand Selectcommand = new SqlCommand(command, SqlConHd);
                DataSet ds = new DataSet();
                SqlDataAdapter DataAd = new SqlDataAdapter();
                DataAd.SelectCommand = Selectcommand;
                if (parameters != null)
                {
                    foreach (var row in parameters)
                    {
                        Selectcommand.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                }
                if (Tranx != null)
                {
                    DataAd.SelectCommand.Transaction = Tranx;
                }
                DataAd.Fill(ds);
                if ((ds != null) && (ds.Tables.Count > 0))
                {
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (Tranx != null)
                {
                    Tranx.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }

            }
        }

        /// <summary>
        ///  Function to execute query into sql server database and return number of record affected
        /// </summary>
        /// <param name="StrCon">Connection string</param>
        /// <param name="SqlCmd">SQL Command to be execute</param>
        /// <param name="Tranx">Indicate if Transaction exist; NULL if non-transcational</param>
        /// <param name="_persist">FALSE means will close Connection immediately after query</param>
        /// <returns></returns>
        public Int64 ExecNonQuery(string Connection, string command, List<ctSqlVariable> parameters, SqlTransaction Tranx, bool _persist)
        {
            bool isSuccess = true;
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    return -1;
                }
                SqlCommand SqlCom = new SqlCommand(command, SqlConHd);
                if (parameters != null)
                {
                    foreach (var row in parameters)
                    {
                        SqlCom.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                }
                if (Tranx != null)
                {
                    SqlCom.Transaction = Tranx;

                }

                Int64 num = new Int64();

                num = SqlCom.ExecuteNonQuery();

                return num;
            }
            catch (Exception Exp)
            {
                isSuccess = false;
                if (Tranx != null)
                {
                    Tranx.Rollback();
                }
                throw Exp;
            }
            finally
            {
                if (!_persist)
                {
                    if (Tranx != null && isSuccess)
                    {
                        Tranx.Commit();
                    }
                    SQLCloseCon();
                }
            }
        }
        public Int64 ExecuteNonQuery(string Connection, string command, SqlTransaction Tranx, bool _persist)
        {
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    return -1;
                }
                SqlCommand SqlCom = new SqlCommand(command, SqlConHd);
                if (Tranx != null)
                {
                    SqlCom.Transaction = Tranx;
                }

                Int64 num = new Int64();

                num = SqlCom.ExecuteNonQuery();

                return num;
            }
            catch (Exception Exp)
            {
                if (Tranx != null)
                {
                    Tranx.Rollback();
                }
                throw Exp;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }
            }
        }
        public string ExecuteScalar(string Connection, string command, List<ctSqlVariable> parameters, SqlTransaction Tranx, bool _persist)
        {
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    return "-1";
                }

                SqlCommand SqlCom = new SqlCommand(command, SqlConHd);
                if (Tranx != null)
                {
                    SqlCom.Transaction = Tranx;
                }


                if (SqlCom.ExecuteScalar() == null)
                {
                    return "";
                }
                else
                {
                    return SqlCom.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                if (Tranx != null)
                {
                    Tranx.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }
            }
        }

        //STORE PROCEDURES
        public DataTable ExecuteStoProcDT(string StrCon, string SqlCmd, List<ctSqlVariable> param)
        {
            DataTable table = new DataTable();
            try
            {

                using (var con = new SqlConnection(StrCon))
                using (var cmd = new SqlCommand(SqlCmd, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                    da.Fill(table);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }
        public DataSet ExecuteStoProcDS(string StrCon, string SqlCmd, List<ctSqlVariable> param)
        {
            DataSet dataSet = new DataSet();
            try
            {

                using (var con = new SqlConnection(StrCon))
                using (var cmd = new SqlCommand(SqlCmd, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                    da.Fill(dataSet);
                    return ((dataSet != null) && (dataSet.Tables.Count > 0)) ? dataSet : null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public long ExecuteStoProcNonQuery(string StrCon, string SqlCmd, List<ctSqlVariable> param)
        {
            try
            {

                using (var con = new SqlConnection(StrCon))
                using (var cmd = new SqlCommand(SqlCmd, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                    return cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ExecuteStoProcScalar(string StrCon, string SqlCmd, List<ctSqlVariable> param)
        {
            try
            {

                using (var con = new SqlConnection(StrCon))
                using (var cmd = new SqlCommand(SqlCmd, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar).Value = row.Value;
                    }
                    return cmd.ExecuteScalar().ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SetApprole(SqlConnection connection, string approle, string approlePassword)
        {
            StringBuilder sqlText = new StringBuilder();

            sqlText.Append("DECLARE @cookie varbinary(8000);");
            sqlText.Append("exec sp_setapprole @rolename = '" + approle + "', @password = '" + approlePassword + "'");
            sqlText.Append(",@fCreateCookie = true, @cookie = @cookie OUTPUT;");
            sqlText.Append(" SELECT @cookie");
            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlText.ToString();
                return (byte[])cmd.ExecuteScalar();
            }
        }
        public void UnsetApprole(SqlConnection connection, byte[] approleCookie)
        {
            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_unsetapprole";
                cmd.Parameters.AddWithValue("@cookie", approleCookie);
                cmd.ExecuteNonQuery();
            }
        }
        public long SaveError(auSystemLog ErrorLog, bool _persist)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"
            INSERT INTO auSystemLog(ComputerIP,ComputerName,ActionName,Query,SystemMessage,CreateUser, CreateDate)
            VALUES('{ErrorLog.ComputerIP}',
'{ErrorLog.ComputerName}','{ErrorLog.ActionName}','{ErrorLog.Query}','{ErrorLog.SystemMessage.Replace("'", "''")}','{ErrorLog.UserID}', GETDATE()
            )");
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(ConnectionDB);
                }
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    return -1;
                }
                SqlCommand SqlCom = new SqlCommand(sb.ToString(), SqlConHd);
                long num = new long();
                num = SqlCom.ExecuteNonQuery();
                //SaveErrorToText("ERRORISSUE", ErrorLog);
                return -1;
            }
            catch (Exception ex)
            {
                ErrorLog.SystemMessage += "________" + ex.ToString();
                //SaveErrorToText("CONNECTIONISSUE", ErrorLog);
                return -1;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }
            }

        }
        public void SaveErrorToText(string ErrorCause, auSystemLog SystemLog)
        {
            string fileName = ConfigurationManager.AppSettings["ERRORPATH"].ToString()
                + $"{ErrorCause}_{DateTime.Now.ToString("yyMMddHHmmssss")}_By_{SystemLog.UserID}_at_{SystemLog.ComputerName}.txt";
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                sw.WriteLine($"This error show because {ErrorCause}");
                sw.WriteLine($"Error Detail= {SystemLog.SystemMessage}");
                sw.WriteLine($"Error Query= {SystemLog.Query}");
                sw.WriteLine($"Error UserID= {SystemLog.UserID}");
                sw.WriteLine($"Action Name= {SystemLog.ActionName}");
            }
            using (StreamReader sr = System.IO.File.OpenText(fileName))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }
        /// <summary>
        /// Comment by Jemmy, because it is not approriate to use common framework to cater for MVS Select List Item
        /// </summary>
        //public List<SelectListItem> DataTableToSelectList(DataTable _dt, string _display, string _value)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();

        //    foreach (DataRow row in _dt.Rows)
        //    {
        //        list.Add(new SelectListItem()
        //        {
        //            Text = row[_display].ToString(),
        //            Value = row[_value].ToString()
        //        });
        //    }
        //    return list;
        //}
        public bool IsSystemConnected(string Connection, bool _persist)
        {
            try
            {
                if (SqlConHd.State == ConnectionState.Closed)
                {
                    SQLCon(Connection);
                }

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (!_persist)
                {
                    SQLCloseCon();
                }
            }
            return true;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                
            }
            disposed = true;
        }
        ~MSSQL()
        {
            Dispose(false);
        }
    }
    public class ctSqlVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

}
