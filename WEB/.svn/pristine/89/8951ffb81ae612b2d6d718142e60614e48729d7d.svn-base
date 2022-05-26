using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;
using VMS.Library.Constants;

namespace VMS.Library.Class
{
    public class Database
    {
        public void SaveErrorToCSV(string ErrorMessage)
        {
            string csvPath = ConfigurationManager.AppSettings[cAppSetting.PathErrLog];
        }
        public void saveError(string errorMessage)
        {
            string query = @"INSERT INTO ErrorLog Values ('" + errorMessage + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
            SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionDB.VMSConnection].ConnectionString);
            SqlCommand cmd;
            try
            {
                using (myConn)
                {
                    cmd = new SqlCommand(query, myConn);
                    myConn.Open();
                    cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                myConn.Dispose();
                throw ex;
            }
            finally
            {
                myConn.Dispose();
            }
        }
        public MessageNonQuery executeNonQueryBeginTran(string query, string connection = ConnectionDB.VMSConnection)
        {
            using (SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
            {
                
                using (var tran = myConn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        myConn.Open();
                        SqlCommand cmd = new SqlCommand(query, myConn, tran);
                        cmd.ExecuteNonQuery();
                        tran.Commit();
                        return new MessageNonQuery { isSuccess = true, message = "Success" };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        saveError(ex.ToString());
                        return new MessageNonQuery { isSuccess = false, message = ex.ToString() };
                    }
                }
            }
        }
        public DataTable getDatatableFromSQL(string query, string connection = ConnectionDB.VMSConnection)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd;
            SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString);
            try
            {
                myConn.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd = new SqlCommand(query, myConn);
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                
                myConn.Dispose();
                saveError(ex.ToString());

            }
            finally
            {
                myConn.Dispose();
            }


            return dt;
        }
        public MessageNonQuery executeNonQuery(string query, string connection)
        {
            SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString);
            SqlCommand cmd;
            try
            {
                using (myConn)
                {
                    cmd = new SqlCommand(query, myConn);
                    myConn.Open();
                    cmd.ExecuteNonQuery();

                }
                return new MessageNonQuery { isSuccess = true, message = "Success" };
            }
            catch (Exception ex)
            {
                myConn.Dispose();
                saveError(ex.ToString());
                return new MessageNonQuery { isSuccess = false, message = "ERROR! Please See Error Log" };

            }
            finally
            {
                myConn.Dispose();
            }
        }
        public DataTable executeStoreGetDataTable(List<SqlVariable> param, string nameStoreProc, string connection)
        {
            DataTable table = new DataTable();
            try
            {
                
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
                using (var cmd = new SqlCommand(nameStoreProc, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar, 255).Value = row.Value;
                    }
                    da.Fill(table);
                }
                
            }
            catch (Exception ex)
            {
                saveError(ex.ToString());
            }
            return table;
        }
        public MessageNonQuery executeStoreProcNonQuery(List<SqlVariable> param, string nameStoreProc, string connection)
        {
            try
            {
                DataTable table = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
                using (var cmd = new SqlCommand(nameStoreProc, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var row in param)
                    {
                        cmd.Parameters.Add("@" + row.Name, SqlDbType.VarChar, 255).Value = row.Value;
                    }
                    da.Fill(table);
                }
                var _datas = (from rw in table.AsEnumerable()
                              select new MessageNonQuery
                              {
                                  isSuccess = rw["isSuccess"].ToString().ToUpper() == "SUCCESS" ? true : false,
                                  message = rw["Messages"].ToString()
                              }).FirstOrDefault();
                return _datas;
            }
            catch (Exception ex)
            {
                saveError(ex.ToString());
                return new MessageNonQuery { isSuccess = false, message = ex.ToString() };
            }
            
        }
    }

    public class WindowsAuth
    {
        //Header
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(
        string lpszUsername,
        string lpszDomain,
        string lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        out IntPtr phToken);

        [DllImport("advapi32.DLL")]
        public static extern int ImpersonateLoggedOnUser(IntPtr hToken); //handle to token for logged-on user

        [DllImport("advapi32.DLL")]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hToken);


        public string SqlConStr = "";
        public string DefaultSqlConStr = "";
        public byte[] RoleCookie;
        public bool ConnectionStatus = false;
        public String SysNam = "";// ConfigurationManager.AppSettings["SystemName"].ToString();
        public String ErrPath = ""; //ConfigurationManager.AppSettings["ErrLogPath"].ToString()
        public IntPtr admin_token = IntPtr.Zero;
        //public  SqlConnection SqlConHd;
        public static int TotalConStr = 10;
        //public  SqlConnection SqlConHd = new SqlConnection();
        public SqlConnection[] PSqlConHd = new SqlConnection[TotalConStr]; // maximum 10 persistent connection

        enum LogonProvider
        {
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT50 = 3,
            LOGON32_PROVIDER_WINNT40 = 2,
            LOGON32_PROVIDER_WINNT35 = 1,
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7
        }
        public Boolean WinAuth(String UsrNam, String PasWrd, String ConStr, bool UseRole, String RoleName, String RolePassword, bool DefaultCon = false)
        {
            //String ssDomain = Environment.UserDomainName;
            String ssDomain = "SHIMANOACE";
            IntPtr phToken = IntPtr.Zero;
            //CloseHandle(admin_token);
            //RevertToSelf();
            //admin_token = IntPtr.Zero;

            int valid = LogonUser(
                UsrNam,
                ssDomain,
                PasWrd,
                (int)LogonProvider.LOGON32_LOGON_INTERACTIVE,
                (int)LogonProvider.LOGON32_PROVIDER_DEFAULT,
                out admin_token);

            if (valid != 0)
            {
                //int IPI = ImpersonateLoggedOnUser(admin_token);
                //if (IPI != 0)
                //{

                ConnectionStatus = SQLCon(ConStr, UseRole, RoleName, RolePassword, DefaultCon);
                CloseHandle(admin_token);
                RevertToSelf();
                admin_token = IntPtr.Zero;
                if (ConnectionStatus == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                //}
                //else
                //{
                //    return false;
                //}
            }
            else
            {
                return false;
            }

        }

        //*****************************************************************************
        //                                   SQLCON 
        //*****************************************************************************
        public Boolean SQLCon(string ConStr, bool UseRole, String RoleName, String RolePassword, bool DefaultCon = false)
        {
            if (DefaultCon == true)
            {
                DefaultSqlConStr = ConStr;
            }

            SqlConnectionStringBuilder sBuild = new SqlConnectionStringBuilder(ConStr);

            for (int ConNum = 0; ConNum < TotalConStr; ConNum++)
            {
                if (PSqlConHd[ConNum] != null)
                {
                    if (PSqlConHd[ConNum].DataSource.ToString() == sBuild.DataSource.ToString() && PSqlConHd[ConNum].Database.ToString() == sBuild.InitialCatalog.ToString())
                    {
                        if (PSqlConHd[ConNum].State == ConnectionState.Open)
                        {
                            if (UseRole == true)
                            {
                                UnsetApprole(PSqlConHd[ConNum], RoleCookie);
                            }
                            PSqlConHd[ConNum].Close();
                        }
                        PSqlConHd[ConNum] = null;
                    }
                }

                if (PSqlConHd[ConNum] == null)
                {
                    PSqlConHd[ConNum] = new SqlConnection();
                    PSqlConHd[ConNum].ConnectionString = ConStr;
                    PSqlConHd[ConNum].Open();
                    if (UseRole == true)
                    {
                        RoleCookie = SetApprole(PSqlConHd[ConNum], RoleName, RolePassword);
                    }
                    return true;
                }
            }
            return false;
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
            //string sqlText = "exec sp_unsetapprole @cookie=@approleCookie";

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
    }
}
