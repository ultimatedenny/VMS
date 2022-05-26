using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using VMS.Library;
using VMS.Web.Class;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace VMS.Web.Models
{
    public class UserAction
    {
        //StringBuilder sb;
        DataTable dt;
        long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public bool AuthentificationUser(LoginViewModel Login)
        {
            string query = @"EXEC [dbo].[spUpdateDelegation]
                             SELECT UseID, Domain, IsWindowsAuth FROM Usr where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1";
            WindowsAuth _wa = new WindowsAuth();
            var parameters = new List<ctSqlVariable>()
            {
                new ctSqlVariable { Name="UseID",Type="String",Value=Login.UseID }
            };
            dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, parameters, null, false);
            }
            Func<bool> CheckLoginNormal = () =>
            {
                query = @"SELECT 1
                            from Usr where (UseID = @UseID) and PWDCOMPARE(@UsePass, [UsePass])= 1 and isActiveAcc=1";
                var subparam = new List<ctSqlVariable>()
                {
                new ctSqlVariable { Name="UseID",Type="String",Value=dt.Rows[0][0].ToString() },
                new ctSqlVariable { Name="UsePass",Type="String",Value=Login.UsePass }
                };
                using (var sql = new MSSQL())
                {
                    return sql.ExecDTQuery(
                        ConnectionDB, query, subparam, null, false).Rows.Count > 0
                        ? true
                        : false;
                }
            };
            if (dt.Rows.Count > 0)
            {
                return (dt.Rows[0][2].ToBoolean())
                    ? _wa.WinAuth(dt.Rows[0][0].ToString(), Login.UsePass, false, dt.Rows[0][1].ToString())
                    : CheckLoginNormal();
            }
            return false;
        }
        public VMSRes<User> GetDetailUser(LoginViewModel Login)
        {
            if (!AuthentificationUser(Login))
            {
                return new VMSRes<User>()
                {
                    Success = false,
                    Message = "User Not Found",
                    data = null
                };
            }
            DataTable dt = new DataTable();

            //Modified by CANICE 20210801
            //string query = @"SELECT UseID, UseNam, UseDep, u.BusFunc, UseLev, UsePlant, isDelegate, StartPage,ApprovalGroup, isAuthoriseSeeNumber
            //            FROM Usr u
            //			INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
            //            where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1";
            //      string query = @"SELECT UseID, UseNam, UseDep, u.BusFunc, UseLev, UsePlant, isDelegate, StartPage,ApprovalGroup, isAuthoriseSeeNumber
            //                  ,case when a.WindowsID is not null then convert(bit,'true') else convert(bit,'false') end as IsAuthorisedDelegate
            //                  ,u.DelegateUseID,Convert(nvarchar(10),isnull(u.DelegateEffectiveFrom,GETDATE()),121) DelegateEffectiveFrom,Convert(nvarchar(10),isnull(u.DelegateEffectiveTo,GETDATE()),121) DelegateEffectiveTo
            //                  FROM Usr u
            //INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
            //                  LEFT JOIN (select distinct a.WindowsID from ApprovingDept a
            //                              join ApprovingDesignation b on a.Designation = b.Designation and a.Status = 'Active' and ApprovingSequence > 0 and WindowsID <> ''
            //                              ) a on u.useid = a.WindowsID
            //                  where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1"; //change by suharjo ng 2021-11-02
            //End Modified 20210801
            //modify by suharjo ng 20211102
            string query = @"SELECT 
	                            UseID
	                            , UseNam
	                            , UseDep
	                            , u.BusFunc
	                            , UseLev
	                            , UsePlant
	                            , isDelegate
	                            , StartPage
	                            , ApprovalGroup
	                            , isAuthoriseSeeNumber
	                            ,case when a.WindowsID is not null then convert(bit,'true') else convert(bit,'false') end as IsAuthorisedDelegate
	                            ,u.DelegateUseID
	                            ,STUFF(replace(convert(varchar(10), DelegateEffectiveFrom, 121)+convert(varchar(8), DelegateEffectiveFrom, 114), ' ','T'), 11,0,'T') DelegateEffectiveFrom
	                            ,STUFF(replace(convert(varchar(10), DelegateEffectiveTo, 121)+convert(varchar(8), DelegateEffectiveTo, 114), ' ','T'), 11,0,'T') DelegateEffectiveTo
                                , CASE 
		                            WHEN ISNULL(DelegateEffectiveFrom,'') = '' AND ISNULL(DelegateEffectiveTo,'') = '' THEN 'StopDelegation'
		                            WHEN ISNULL(DelegateEffectiveTo, '') = '' THEN 'StartFrom'
		                            ELSE 'Range'
	                              END As DelegationPeriod
                                , u.DelegateRemarks
                            FROM Usr u
                            INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
                            OUTER APPLY (SELECT TOP 1 * FROM mmApprovalLevel WHERE WindowsID = u.UseID) a
                            where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1";
            //End Modified 20211102
            var parameters = new List<ctSqlVariable>()
            {
                new ctSqlVariable { Name="UseID",Type="String",Value=Login.UseID }
            };
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, parameters, null, false);
            }

            var _data = (from row in dt.AsEnumerable()
                         select new User()
                         {
                             UseID = row["UseID"].ToString().ToUpper(),
                             UseNam = row["UseNam"].ToString().ToUpper(),
                             UseDep = row["UseDep"].ToString().ToUpper(),
                             BusFunc = row["BusFunc"].ToString().ToUpper(),
                             UseLev = row["UseLev"].ToString().ToUpper(),
                             isDelegate = row["isDelegate"].ToBoolean(),
                             UsePlant = row["UsePlant"].ToString().ToUpper(),
                             StartPage = row["StartPage"].ToString().ToUpper(),
                             ApprovalGroup = row["ApprovalGroup"].ToString().ToUpper(),
                             isAuthoriseSeeNumber = row["isAuthoriseSeeNumber"].ToBoolean(),
                             IsAuthorisedDelegate = row["IsAuthorisedDelegate"].ToBoolean(),    //Added by CANICE 20210801
                             DelegateUseID = row["DelegateUseID"].ToString().ToUpper(),                   //Added by CANICE 20210801
                             DelegateEffectiveFrom = row["DelegateEffectiveFrom"].ToString().ToUpper(),   //Added by CANICE 20210801
                             DelegateEffectiveTo = row["DelegateEffectiveTo"].ToString().ToUpper(),       //Added by CANICE 20210801
                             DelegationPeriod = row["DelegationPeriod"].ToString().ToUpper(),        //Added by Suharjo Ng 20211102
                             DelegateRemarks = row["DelegateRemarks"].ToString().ToUpper()       //Added by Suharjo Ng 20211102
                         }).FirstOrDefault();
            return new VMSRes<User>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _data
            };
        }
        public VMSRes<User> GetDetailUserParam(string username)
        {
            DataTable dt = new DataTable();
            
            string query = @"DECLARE @UserLogin VARCHAR(100) = '"+ username + @"'
                            SELECT 
	                            UseID
	                            , UseNam
	                            , UseDep
	                            , u.BusFunc
	                            , UseLev
	                            , UsePlant
	                            , isDelegate
	                            , StartPage
	                            , ApprovalGroup
	                            , isAuthoriseSeeNumber
	                            ,case when a.WindowsID is not null then convert(bit,'true') else convert(bit,'false') end as IsAuthorisedDelegate
	                            ,u.DelegateUseID
	                            ,STUFF(replace(convert(varchar(10), DelegateEffectiveFrom, 121)+convert(varchar(8), DelegateEffectiveFrom, 114), ' ','T'), 11,0,'T') DelegateEffectiveFrom
	                            ,STUFF(replace(convert(varchar(10), DelegateEffectiveTo, 121)+convert(varchar(8), DelegateEffectiveTo, 114), ' ','T'), 11,0,'T') DelegateEffectiveTo
                                , CASE 
		                            WHEN ISNULL(DelegateEffectiveFrom,'') = '' AND ISNULL(DelegateEffectiveTo,'') = '' THEN 'StopDelegation'
		                            WHEN ISNULL(DelegateEffectiveTo, '') = '' THEN 'StartFrom'
		                            ELSE 'Range'
	                              END As DelegationPeriod
                                , u.DelegateRemarks
                            FROM Usr u
                            INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
                            OUTER APPLY (SELECT TOP 1 * FROM mmApprovalLevel WHERE WindowsID = u.UseID) a
                            where (UseID = @UserLogin) and [isActiveAcc] = 1";
            //End Modified 20211102
            //var parameters = new List<ctSqlVariable>()
            //{
            //    new ctSqlVariable { Name="UseID",Type="String",Value=Login.UseID }
            //};
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from row in dt.AsEnumerable()
                         select new User()
                         {
                             UseID = row["UseID"].ToString().ToUpper(),
                             UseNam = row["UseNam"].ToString().ToUpper(),
                             UseDep = row["UseDep"].ToString().ToUpper(),
                             BusFunc = row["BusFunc"].ToString().ToUpper(),
                             UseLev = row["UseLev"].ToString().ToUpper(),
                             isDelegate = row["isDelegate"].ToBoolean(),
                             UsePlant = row["UsePlant"].ToString().ToUpper(),
                             StartPage = row["StartPage"].ToString().ToUpper(),
                             ApprovalGroup = row["ApprovalGroup"].ToString().ToUpper(),
                             isAuthoriseSeeNumber = row["isAuthoriseSeeNumber"].ToBoolean(),
                             IsAuthorisedDelegate = row["IsAuthorisedDelegate"].ToBoolean(),    //Added by CANICE 20210801
                             DelegateUseID = row["DelegateUseID"].ToString().ToUpper(),                   //Added by CANICE 20210801
                             DelegateEffectiveFrom = row["DelegateEffectiveFrom"].ToString().ToUpper(),   //Added by CANICE 20210801
                             DelegateEffectiveTo = row["DelegateEffectiveTo"].ToString().ToUpper(),       //Added by CANICE 20210801
                             DelegationPeriod = row["DelegationPeriod"].ToString().ToUpper(),        //Added by Suharjo Ng 20211102
                             DelegateRemarks = row["DelegateRemarks"].ToString().ToUpper()        //Added by Suharjo Ng 20211102
                         }).FirstOrDefault();
            return new VMSRes<User>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _data
            };
        }
        public void PostLastLoginDetail(User UserLastLogin)
        {
            string query = @"UPDATE us set [LastLoginDevice] = @LastLoginDevice,
                    [LastLoginDate]=GETDATE()
                    from Usr us where UseID = @UseID";
            var parameters = new List<ctSqlVariable>()
            {
                new ctSqlVariable {Name="UseID", Type="String", Value= UserLastLogin.UseID },
                new ctSqlVariable {Name="LastLoginDevice", Type="String", Value= UserLastLogin.LastLoginDevice }
            };
            using (MSSQL sql = new MSSQL())
            {
                sql.ExecNonQuery(ConnectionDB, query, parameters, null, false);
            }
        }
//=================================================================================
        public List<User> GetUsersList(string Plant)
        {
            string query = $@"SELECT UseID, UseNam, isnull(d.DeptName, 'Initial : '+ u.UseDep) as UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant,[isWindowsAuth],
[isActiveAcc],Domain,ApprovalGroup from Usr U
LEFT OUTER JOIN Dept d on U.UseDep = d.Dept and u.UsePlant=d.Plant where UsePlant='{Plant}' order by UseNam";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new User
                          {
                              UseID = rw["UseID"].ToString().ToUpper(),
                              UseNam = rw["UseNam"].ToString().ToUpper(),
                              UseDep = rw["UseDep"].ToString().ToUpper(),
                              UseLev = rw["UseLev"].ToString().ToUpper(),
                              UseEmail = rw["UseEmail"].ToString().ToUpper(),
                              UseTel = rw["UseTel"].ToString().ToUpper(),
                              BusFunc = rw["BusFunc"].ToString().ToUpper(),
                              isActiveAcc = rw["isActiveAcc"].ToBoolean(),
                              UsePlant = rw["UsePlant"].ToString().ToUpper(),
                              Domain = rw["Domain"].ToString().ToUpper(),
                              ApprovalGroup = rw["ApprovalGroup"].ToString().ToUpper(),
                              isWindowsAuth = rw["isWindowsAuth"].ToBoolean(),
                          }).ToList();
            return _datas;
        }
        public VMSRes<string> PostNewUser(User NewData, User OldData)
        {
            if (isDuplicate(NewData.UseID, OldData.UseID))
            {
                return new VMSRes<string>
                {
                    Success=false,
                    Message="This user has been registered before!"
                };

            }
            string query = @"
            IF EXISTS(SELECT * FROM Usr where UseID = '" + OldData.UseID + @"')
            BEGIN 
            UPDATE [Usr]
               SET [UseID] = UPPER('" + NewData.UseID + @"')
                  ,[UseNam] = UPPER('" + NewData.UseNam + @"')
                  ,[UseDep] = UPPER('" + NewData.UseDep + @"')
                  ,[UseLev] = UPPER('" + NewData.UseLev + @"')
                  ,[UseEmail] = UPPER('" + NewData.UseEmail + @"')
                  ,[UseIC] = UPPER('" + NewData.UseIC + @"')
                  ,[UseTel] = UPPER('" + NewData.UseTel + @"')
                  ,[BusFunc] = UPPER('" + NewData.BusFunc + @"')
                  ,[UsePlant] = UPPER('" + NewData.UsePlant + @"')
                  ,[ChgUsr] = UPPER('" + Sessions.GetUseID() + @"')
                  ,[ChgDat] = GETDATE()
                  , ApprovalGroup = UPPER('" + NewData.ApprovalGroup + @"')
             WHERE UseID='" + OldData.UseID + @"'
             SELECT 'SUCCESS' as isSuccess, 'Update Successfully' as message
            END

            ELSE
            BEGIN
            INSERT INTO [dbo].[Usr]
                       ([Group]
                       ,[UseID]
                       ,[UseNam]
                       ,[UseDep]
                       ,[UseLev]
                       ,[UseEmail]
                       ,[UseIC]
                       ,[UseTel]
                       ,[BusFunc]
                       ,[UsePlant]
                       ,[isDelegate]
                       ,[CreUsr]
                       ,[CreDat], isWindowsAuth,isActiveAcc,Domain,
            ApprovalGroup)
                 VALUES
                       (
                        UPPER('VisitorMS')
                       ,UPPER('" + NewData.UseID + @"')
                       ,UPPER('" + NewData.UseNam + @"')
                       ,UPPER('" + NewData.UseDep + @"')
                       ,UPPER('" + NewData.UseLev + @"')
                       ,UPPER('" + NewData.UseEmail + @"')
                       ,UPPER('" + NewData.UseIC + @"')
                       ,UPPER('" + NewData.UseTel + @"')
                       ,UPPER('" + NewData.BusFunc + @"')
                       ,UPPER('" + NewData.UsePlant + @"')
                       ,UPPER('0')
                       ,UPPER('" + Sessions.GetUseID()+ @"'),GETDATE(),1,1,UPPER('SHIMANOACE')
                       ,UPPER('" + NewData.ApprovalGroup + @"')
                       )
                        SELECT 'SUCCESS' as isSuccess, 'Insert Successfully' as message
                        END";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt.OneRowdtToResult();
        }
        public List<MenulistDto> GetMenuList(string BusFunc)
        {
            DataTable dt = new DataTable();
            string query = @"select MnuNam, ML.MnuCod, MnuPrt, Pic, FrmNam from Menulist ML 
INNER JOIN LevelMenu LM on ML.MnuCod = LM.MnuCod where busFunc ='" + BusFunc + "' and isView='1' order by MnuSeq";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MenulistDto
                          {
                              MenuAction = (rw["FrmNam"].ToString() == "") ? "" : (rw["FrmNam"].ToString().Split('_'))[1],
                              MenuController = (rw["FrmNam"].ToString() == "") ? "" : (rw["FrmNam"].ToString().Split('_'))[0],
                              MenuCode = rw["MnuCod"].ToString(),
                              MenuIcon = rw["Pic"].ToString(),
                              MenuName = rw["MnuNam"].ToString(),
                              MenuParent = rw["MnuPrt"].ToString()
                          }).ToList();

            return _datas;
        }
        public List<BusFunc> GetBusfunc()
        {
            string query = @"SELECT BusFunc from Busfunc";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new BusFunc
                          {
                              Id = rw["BusFunc"].ToString().ToUpper(),
                              Name = rw["BusFunc"].ToString().ToUpper(),
                          }).ToList();
            return _datas;
        }
        public List<ApprovalGroup> GetAprrovalGroupsDL()
        {
            string query = @"SELECT DISTINCT ApprovalGroup from ApprovalGroup";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new ApprovalGroup
                          {
                              Group = rw["ApprovalGroup"].ToString().ToUpper()
                          }).ToList();
            return _datas;
        }
        public bool CheckUserAvailable(string UseID)
        {
            string subquery = $@"SELECT top 1 1 from Usr where UseID='{UseID}'";
            using (var sql = new MSSQL())
            {
                return sql.ExecDTQuery(ConnectionDB, subquery, null, null, false).dtHavingRow();
            }
        }
        public bool isDuplicate(string NewUserID, string OldUserID)
        {
            bool result = false;
            //check New
            if (OldUserID == "0" && CheckUserAvailable(NewUserID))
            {
                result = true;
            }
            if (OldUserID != NewUserID && CheckUserAvailable(NewUserID))
            {
                result = true;
            }

            return result;
        }
        public VMSRes<string> PostChangeUserStatus(string UseID)
        {
            string query = $"UPDATE u set isActiveAcc = CASE WHEN isActiveAcc = 1 then 0 else 1 end from Usr u where UseID='{UseID}'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        //======================================================================================
        public List<User> GetUsersForAppointment()
        {
            string query = @"SELECT UseID, UseNam, UseDep from Usr where isActiveAcc=1";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _data = (from rw in dt.AsEnumerable()
                         select new User
                         {
                             UseID = rw["UseID"].ToString().ToUpper(),
                             UseNam = rw["UseNam"].ToString().ToUpper(), //Changed By Minthaw  //UseNam = rw["UseNam"].ToString() + " - " + rw["UseDep"].ToString(),
                             UseDep = rw["UseDep"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        //add by suharjo ng 2021-11-15
        public List<User> GetDelegateTo()
        {
            string query = @"SELECT UseID, UseNam, UseDep from Usr where isActiveAcc=1 AND ISNULL(DelegateUseID, '') = ''";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _data = (from rw in dt.AsEnumerable()
                         select new User
                         {
                             UseID = rw["UseID"].ToString().ToUpper(),
                             UseNam = rw["UseNam"].ToString().ToUpper() + " - " + rw["UseDep"].ToString().ToUpper(),
                             UseDep = rw["UseDep"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        //end add
        public List<ApprovalGroup> GetUserDeptForAppointment(string plant)
        {
            string query = @"SELECT distinct Dept FROM Dept where Plant = '" + plant + @"'";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _data = (from rw in dt.AsEnumerable()
                         select new ApprovalGroup
                         {
                             Dept = rw["Dept"].ToString()
                         }).ToList();
            return _data;
        }

        public List<Menulist> GetMenuListEdit(string BusFunc)
        {
            string query = @"select * into #temp from LevelMenu Where busfunc='" + BusFunc + @"'
select MnuNam, mnuseq, ML.MnuCod, MnuPrt, FrmNam, isnull(isView,0) as isView from Menulist ML 
LEFT OUTER JOIN #temp LM on ML.MnuCod = LM.MnuCod Where ML.mnucod !='0000000' order by MnuSeq
drop table #temp";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new Menulist
                          {
                              MenuName = rw["MnuNam"].ToString().ToUpper(),
                              MenuSeq = rw["MnuSeq"].ToString().ToUpper(),
                              MenuCode = rw["MnuCod"].ToString().ToUpper(),
                              MenuParent = rw["MnuPrt"].ToString().ToUpper(),
                              isView = rw["isView"].ToString().ToUpper(),
                          }).ToList();
            return _datas;
        }
        public VMSRes<User> GetUserDetail(string UseID)
        {
            string query = @"SELECT UseID, UseNam, UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant,[ApprovalGroup] from Usr where UseID='" + UseID + "'";
            DataTable dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new User
                          {
                              UseID = rw["UseID"].ToString().ToUpper(),
                              UseNam = rw["UseNam"].ToString().ToUpper(),
                              UseDep = rw["UseDep"].ToString().ToUpper(),
                              UseLev = rw["UseLev"].ToString().ToUpper(),
                              UseEmail = rw["UseEmail"].ToString().ToUpper(),
                              UseIC = rw["UseIC"].ToString().ToUpper(),
                              UseTel = rw["UseTel"].ToString().ToUpper(),
                              BusFunc = rw["BusFunc"].ToString().ToUpper(),
                              UsePlant = rw["UsePlant"].ToString().ToUpper(),
                              ApprovalGroup = rw["ApprovalGroup"].ToString().ToUpper()
                          }).FirstOrDefault();
            return new VMSRes<User>
            {
                Success = dt.dtHavingRow(),
                data = _datas
            };
        }
        public DataTable CheckUser(string UseID)
        {
            string query = @"SELECT UseID, UseNam, UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant from Usr where UseID='" + UseID + "'";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public List<User> GetListApprover()
        {
            string query = $@"SELECT UseID,UseNam,UseTel,UseEmail,isDelegate FROM Usr where UseLev in ('1','2') and UsePlant='{Sessions.GetUsePlant()}' order by UseLev";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new User
                          {
                              UseID = rw["UseID"].ToString().ToUpper(),
                              UseNam = rw["UseNam"].ToString().ToUpper(),
                              UseEmail = rw["UseEmail"].ToString().ToUpper(),
                              UseTel = rw["UseTel"].ToString().ToUpper(),
                              isDelegate = (rw["isDelegate"].ToString().ToUpper() == "TRUE" ? true : false)


                          }).ToList();
            return _datas;
        }
        public VMSRes<string> ChangeDelegateByRec(string UseID)
        {
            string query = @"UPDATE Usr set isDelegate = case
    when isDelegate = 1 then 0 else 1 end
	where  UseID='" + UseID + @"'
    
    EXEC [Email_Delegate] '" + UseID + "' ";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> GetUserFromAD(string Plant)
        {
            int countInsert = 0;
            try {
                using (var context = new PrincipalContext(ContextType.Domain, "shimanoace"))
                {
                    string AdMemberOf = Global.GetAdMemberOf(Plant);
                    using (var group = GroupPrincipal.FindByIdentity(context, AdMemberOf))
                    {
                        var users = group.GetMembers(true);
                        foreach (UserPrincipal user in users)
                        {
                            //user variable has the details about the user 
                            countInsert = (PostUpdateUserWithAD(user, Plant).Success) ? countInsert + 1 : countInsert;
                        }
                    }
                }
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, $@"delete from usr where creusr = 'ACTIVE-DIRECTORY' and convert(date,credat)<> convert(date,getdate()) and (convert(date,chgdat) <> convert(date,getdate()) OR chgdat is null)", null, null, false);
                }
            }
            catch (Exception ex)
            { 
                return new VMSRes<string>
                {
                    Success = false,
                    data = countInsert.ToString(),
                    Message = "Failed, Only Add " + countInsert.ToString() + " Person(s)<br/>" + ex.ToString()
                };
            }

            return new VMSRes<string>
            {
                Success = true,
                data = countInsert.ToString(),
                Message = "Success Add " + countInsert.ToString() + " Person(s)"
            };

            //string domainPath = "LDAP://{0},DC=shimanoace,DC=local";
            //List<string> ouList = Global.GetOUList();
            //string filterStr = Global.GetOUFilter();
            //string AdMemberOf = Global.GetAdMemberOf(Plant);
            //int countInsert = 0;
            //try
            //{
            //    foreach (string ou in ouList)
            //    {
            //        DirectoryEntry root = new DirectoryEntry(string.Format(domainPath, ou));
            //        DirectorySearcher searcher = new DirectorySearcher(root);
            //        searcher.Filter = filterStr;
            //        foreach (SearchResult searchResult in searcher.FindAll())
            //        {
                        
            //            foreach (string prop in searchResult.Properties["memberOf"])
            //            {
            //                if (prop.ToUpper().Contains(AdMemberOf.ToUpper()))
            //                {
            //                    countInsert = (PostNewUserWithAD(searchResult, Plant).Success) ? countInsert + 1 : countInsert;
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return new VMSRes<string>
            //    {
            //        Success = false,
            //        Message = "Failed, Only Add " + countInsert.ToString() + " Person(s)<br/>" + ex.ToString()
            //    };
            //}

            //return new VMSRes<string>
            //{
            //    Success = true,
            //    Message="Success Add " + countInsert.ToString() + " Person(s)"
            //};
        }
        public VMSRes<bool> PostUpdateUserWithAD(UserPrincipal user, string plant)
        {
            VMSRes<bool> scalar = new VMSRes<bool>();
            string sca;

            DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;

            string UserWinDowsID = user.SamAccountName ?? "";
            string UserName = user.DisplayName ?? ""; //Modified by CANICE 20210801 Change to DisplayName//string UserName = user.Name ?? "";
            object ADDept = directoryEntry.Properties["Department"].Value ?? ""; //Added by CANICE 20210407
            string UserDept = ADDept.ToString(); //Modified by CANICE 20210407 //string UserDept = user.Description ?? "";
            string UserEmail = user.EmailAddress ?? "";
            string UserTel = user.VoiceTelephoneNumber ?? "";



            UserWinDowsID = UserWinDowsID.Substring(0,Math.Min(50, UserWinDowsID.Length)).Trim().ToUpper();
            UserName = UserName.Substring(0, Math.Min(100, UserName.Length)).Trim().ToUpper();
            UserDept = UserDept.Substring(0, Math.Min(50, UserDept.Length)).Trim().ToUpper();
            UserEmail = UserEmail.Substring(0, Math.Min(50, UserEmail.Length)).Trim().ToUpper();
            UserTel = UserTel.Substring(0, Math.Min(30, UserTel.Length)).Trim().ToUpper();

            //if (UserWinDowsID == "SPL97040")
            //{
            //    string a = UserWinDowsID;
            //}
            UserDept = CheckDept(UserDept, plant);

            string checkUserQuery = $"SELECT COUNT(*) from Usr where UseID = '{UserWinDowsID}'";
            using (var sql = new MSSQL())
            {
                sca = sql.ExecuteScalar(ConnectionDB, checkUserQuery, null, null, false);
            }
            if (int.Parse(sca) == 0)
            {
                string query = $@"INSERT INTO [dbo].[Usr]
           ([Group],[UseID],[UseNam],[UsePass],[UseDep],[UseLev],[UseEmail]
           ,[UseTel],[BusFunc],[UsePlant],[isDelegate],[CreUsr],[CreDat],[isWindowsAuth],[isActiveAcc]
           ,[Domain])
     VALUES
           ('VisitorMS','{UserWinDowsID}','{UserName.Replace("'", "`")}','','{UserDept}','4','{UserEmail}'
           ,'{UserTel}','SYSTEM-USER','{plant}' ,0,'ACTIVE-DIRECTORY',GETDATE()
           ,1
           ,1
           ,'SHIMANOACE')";

                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                }
                if (num.NonQueryResults().Success)
                {

                    scalar.Success = true;
                    scalar.Message = (UserName + "#" + UserDept);
                }
                else
                {
                    scalar.Success = false;
                }
            }
            else
            {
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, $@"Update Usr Set UseNam = '{UserName}', UseDep = '{UserDept}', Chgdat = GETDATE() Where UseId = '{UserWinDowsID}'", null, null, false);
                }
            }
            return scalar;
        }

        public string CheckDept (string Dept, string Plant)
        {
            string query = "";
            string res;
            query = $"SELECT COUNT(*) from Dept where Dept = '{Dept}'";
            using (var sql = new MSSQL())
            {
                res = sql.ExecuteScalar(ConnectionDB, query, null, null, false);
            }
            if (int.Parse(res) != 0)
            {
                return Dept;
            }
            else
            {
                query = $"SELECT *from Dept where DeptName ='{Dept}'";
                using (var sql = new MSSQL())
                {
                    dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
                }
                if (dt.Rows.Count > 0)
                {
                    var _Dept = (from rw in dt.AsEnumerable()
                                 select new Department
                                 {
                                     Dept = rw["Dept"].ToString(),
                                 }).ToList();
                    return _Dept[0].Dept;
                }
                else
                {
                    //query = $@"insert into dept (Plant, Dept, DeptName, UpdateBy, UpdateAt) select '{Plant}','{Dept}','{Dept}','IMPORTUSR',GETDATE()";
                    //using (var sql = new MSSQL())
                    //{
                    //    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                    //}
                    return Dept;
                }

            }
        }


        public VMSRes<bool> PostNewUserWithAD(SearchResult searchResult, string plant)
        {

            VMSRes<bool> scalar = new VMSRes<bool>();
            DirectoryEntry de = searchResult.GetDirectoryEntry();
            string sca;
            if (IsActive(de))
            {
                string UserID = GetProperty(searchResult, ADProp.USEID);
                string userName = GetProperty(searchResult, ADProp.USENAM);
                string userEmail = GetProperty(searchResult, ADProp.USEMAIL);
                string userDept = GetProperty(searchResult, ADProp.USEDEPT);
                string UserTel = GetProperty(searchResult, ADProp.USETEL);
                UserID = UserID.IndexOf("@") < 0 ? UserID : UserID.Substring(0, UserID.IndexOf("@"));
                string checkUserQuery = $"SELECT COUNT(*) from Usr where UseID = '{UserID}'";
                using (var sql = new MSSQL())
                {
                    sca = sql.ExecuteScalar(ConnectionDB, checkUserQuery, null, null, false);
                }
                if (int.Parse(sca) == 0 && ((userEmail != "") && userEmail.ToUpper().Contains("SHIMANO")))
                {
                    string query = $@"INSERT INTO [dbo].[Usr]
           ([Group],[UseID],[UseNam],[UsePass],[UseDep],[UseLev],[UseEmail]
           ,[UseTel],[BusFunc],[UsePlant],[isDelegate],[CreUsr],[CreDat],[isWindowsAuth],[isActiveAcc]
           ,[Domain])
     VALUES
           ('VisitorMS','{UserID.ToUpper()}','{userName.Replace("'", "`")}','','{userDept}','4','{userEmail}'
           ,'{UserTel}','SYSTEM-USER','{plant}' ,0,'ACTIVE-DIRECTORY',GETDATE()
           ,1
           ,1
           ,'SHIMANOACE')";
                    using (var sql = new MSSQL())
                    {
                        num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                    }
                    if (num.NonQueryResults().Success)
                    {
                       
                        scalar.Success = true;
                        scalar.Message = (userName + "#" + userDept);
                        //CHECK Depti
                        query = $"SELECT COUNT(*) from Dept where Dept='{userDept}'";
                        using (var sql = new MSSQL())
                        {
                            sca = sql.ExecuteScalar(ConnectionDB, checkUserQuery, null, null, false);
                        }
                        if (int.Parse(sca) == 0)
                        {
                            scalar.data = false;
                        }
                    }
                    else
                    {
                        scalar.Success = false;
                    }

                }
                
            }
            return scalar;
        }
        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                if (PropertyName == "userprincipalname")
                {
                    return string.Empty;
                }
                return string.Empty;
            }
        }
        public static string GetProperty2(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                if (PropertyName == "userprincipalname")
                {
                    return string.Empty;
                }
                return string.Empty;
            }
        }
        private bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;

            int flags = (int)de.Properties["userAccountControl"].Value;

            return !Convert.ToBoolean(flags & 0x0002);
        }
        public VMSRes<string> GetCheckUserGroup(string Dept)
        {
            //Modified by CANICE 20210920
            //            string query = $@"DECLARE @Dept varchar(50)='{Dept}'
            //IF NOT EXISTS(select * from Dept where Dept=@Dept)
            //select 'false', 'Your Department informaton: {Dept} is not found from Master List. Please screenshot this message and seek for IT''s assistance. Thank  you.'
            //ELSE IF NOT EXISTS(select * from ApprovalGroup where Dept=@Dept)
            //select 'false', 'Your Approval Group informaton: {Dept} is not found from Master List. Please screenshot this message and seek for IT''s assistance. Thank  you.'
            //else
            //select 'true', 'success'";
            
            string query = $@"DECLARE @ApprovalMode VARCHAR(20) = ''
	        SET @ApprovalMode = (SELECT [dbo].[fnGetConfig] ('ApprovalConfig'))
	        IF @ApprovalMode = 'ApprovingDept'
	        BEGIN
                select 'true', 'success'
            END
            ELSE
            BEGIN
                DECLARE @Dept varchar(50)='{Dept}'
                IF NOT EXISTS(select * from Dept where Dept=@Dept)
                select 'false', 'Your Department informaton: {Dept} is not found from Master List. Please screenshot this message and seek for IT''s assistance. Thank  you.'
                ELSE IF NOT EXISTS(select * from ApprovalGroup where Dept=@Dept)
                select 'false', 'Your Approval Group informaton: {Dept} is not found from Master List. Please screenshot this message and seek for IT''s assistance. Thank  you.'
                else
                select 'true', 'success'
            END";
            //End Modified 20210920
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt.OneRowdtToResult();
        }
        public VMSRes<DataTable> GetComparDiffDept()
        {
            string query = @"select UseID as [User ID], UseNam as [User Name], UseDep as [Department] from Usr
where UseDep in (
SELECT distinct UseDep from Usr U
LEFT JOIN Dept d on U.UseDep = d.Dept where d.Dept is null)
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return new VMSRes<DataTable>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = dt
            };
        }
        public List<User> CheckDelegation (string UseID)
        {
            string query = $@"select u1.DelegateUseID,isnull(u2.UseNam,'') DelegateUseNam
                            ,Convert(nvarchar(10),isnull(u1.DelegateEffectiveFrom,GETDATE()),121) DelegateEffectiveFrom,Convert(nvarchar(10),isnull(u1.DelegateEffectiveTo,GETDATE()),121) DelegateEffectiveTo
                            from usr u1 
                            left join usr u2 on u1.DelegateUseID = u2.useid
                            where u1.useid = '{UseID}'";
            dt = new DataTable();
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _Usr = (from rw in dt.AsEnumerable()
                                 select new User
                                 {
                                     DelegateUseID = rw["DelegateUseID"].ToString().ToUpper(),
                                     DelegateUseNam = rw["DelegateUseNam"].ToString().ToUpper(),
                                     DelegateEffectiveFrom = rw["DelegateEffectiveFrom"].ToString().ToUpper(),
                                     DelegateEffectiveTo = rw["DelegateEffectiveTo"].ToString().ToUpper()
                                 }).ToList();
            return _Usr;
        }
        public VMSRes<bool> RemoveDelegation(UserPrincipal user, string plant)
        {
            VMSRes<bool> scalar = new VMSRes<bool>();
            string sca;

            DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;

            string UserWinDowsID = user.SamAccountName ?? "";
            string UserName = user.DisplayName ?? ""; //Modified by CANICE 20210801 Change to DisplayName//string UserName = user.Name ?? "";
            object ADDept = directoryEntry.Properties["Department"].Value ?? ""; //Added by CANICE 20210407
            string UserDept = ADDept.ToString(); //Modified by CANICE 20210407 //string UserDept = user.Description ?? "";
            string UserEmail = user.EmailAddress ?? "";
            string UserTel = user.VoiceTelephoneNumber ?? "";



            UserWinDowsID = UserWinDowsID.Substring(0, Math.Min(50, UserWinDowsID.Length)).Trim().ToUpper();
            UserName = UserName.Substring(0, Math.Min(100, UserName.Length)).Trim().ToUpper();
            UserDept = UserDept.Substring(0, Math.Min(50, UserDept.Length)).Trim().ToUpper();
            UserEmail = UserEmail.Substring(0, Math.Min(50, UserEmail.Length)).Trim().ToUpper();
            UserTel = UserTel.Substring(0, Math.Min(30, UserTel.Length)).Trim().ToUpper();

            //if (UserWinDowsID == "SPL97040")
            //{
            //    string a = UserWinDowsID;
            //}
            UserDept = CheckDept(UserDept, plant);

            string checkUserQuery = $"SELECT COUNT(*) from Usr where UseID = '{UserWinDowsID}'";
            using (var sql = new MSSQL())
            {
                sca = sql.ExecuteScalar(ConnectionDB, checkUserQuery, null, null, false);
            }
            if (int.Parse(sca) == 0)
            {
                string query = $@"INSERT INTO [dbo].[Usr]
           ([Group],[UseID],[UseNam],[UsePass],[UseDep],[UseLev],[UseEmail]
           ,[UseTel],[BusFunc],[UsePlant],[isDelegate],[CreUsr],[CreDat],[isWindowsAuth],[isActiveAcc]
           ,[Domain])
     VALUES
           ('VisitorMS','{UserWinDowsID}','{UserName.Replace("'", "`")}','','{UserDept}','4','{UserEmail}'
           ,'{UserTel}','SYSTEM-USER','{plant}' ,0,'ACTIVE-DIRECTORY',GETDATE()
           ,1
           ,1
           ,'SHIMANOACE')";

                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                }
                if (num.NonQueryResults().Success)
                {

                    scalar.Success = true;
                    scalar.Message = (UserName + "#" + UserDept);
                }
                else
                {
                    scalar.Success = false;
                }
            }
            else
            {
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, $@"Update Usr Set UseNam = '{UserName}', UseDep = '{UserDept}', Chgdat = GETDATE() Where UseId = '{UserWinDowsID}'", null, null, false);
                }
            }
            return scalar;
        }

        public VMSRes<string> RemoveDelegation(string UseID)
        {
            try
            {
                string query = $@"Declare @ExistingDelegateUseID VARCHAR(50) = '';
                                  SELECT @ExistingDelegateUseID = DelegateUseID FROM Usr WHERE UseId = '{UseID}';
                                  Update Usr Set DelegateUseID = '',DelegateEffectiveFrom = NULL,DelegateEffectiveTo = NULL, isDelegate=0, ChgUsr = '{UseID}'
                                  , Chgdat = GETDATE() Where UseId = '{UseID}';
                                  EXEC [Email_Delegate] @UseID = '{UseID}',@DelegateUseID = @ExistingDelegateUseID, @DelegateStatus = 'REMOVE DELEGATION'";
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB,query, null, null, false);
                }
            }
            catch (Exception ex)
            {
                return new VMSRes<string> 
                {
                    Success = false,
                    data = "",
                    Message = ex.Message
                };
            }

            return new VMSRes<string>
            {
                Success = true,
                data = "",
                Message = "Successfully remove existing delegation."
            };
        }
        public VMSRes<string> StopDelegation(string UseID)
        {
            try
            {
                string query = $@"Declare @UseID VARCHAR(50) = '" + UseID + @"'
                            Update Usr Set DelegateUseID = NULL,DelegateEffectiveFrom = NULL,DelegateEffectiveTo = NULL, ChgUsr = @UseID, DelegateRemarks = NULL
                                , Chgdat = GETDATE(), isDelegate=0 Where UseId = @UseID;
                            ";
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                }
            }
            catch (Exception ex)
            {
                return new VMSRes<string>
                {
                    Success = false,
                    data = "",
                    Message = ex.Message
                };
            }

            return new VMSRes<string>
            {
                Success = true,
                data = "",
                Message = "Successfully stop delegation."
            };
        }
        public VMSRes<string> SaveDelegation(String UseID, string DelegateUseID, string DelegateEffectiveFrom, string DelegateEffectiveTo, string DelegationRemarks)
        {
            try
            {
                //string query = $@"Update Usr Set DelegateUseID = '{DelegateUseID}'
                //            ,DelegateEffectiveFrom = '{DelegateEffectiveFrom}',DelegateEffectiveTo = '{DelegateEffectiveTo}'
                //            ,DelegateRemarks = '{DelegationRemarks}'
                //            ,ChgUsr = '{UseID}', Chgdat = GETDATE() Where UseId = '{UseID}';
                //            EXEC [Email_Delegate] @UseID = '{UseID}',@DelegateUseID = '{DelegateUseID}', @DelegateStatus = 'DELEGATION'";
                string query = string.Empty;
                if (string.IsNullOrEmpty(DelegateUseID) || string.IsNullOrEmpty(DelegateEffectiveFrom))
                {
                    query = @"DECLARE
	                           @DelegateUseID VARCHAR(50) = '" + DelegateUseID + @"'
	                          ,@DelegateEffectiveFrom VARCHAR(16) = '" + DelegateEffectiveFrom + @"'
	                          ,@DelegateEffectiveTo VARCHAR(16) = '" + DelegateEffectiveTo + @"'
	                          ,@DelegateRemarks NVARCHAR(MAX) = '" + DelegationRemarks + @"'
	                          ,@UseID VARCHAR(50) = '" + UseID + @"'

                              IF @DelegateEffectiveFrom = '' SET @DelegateEffectiveFrom = null
                              IF @DelegateEffectiveTo = '' SET @DelegateEffectiveTo = null

                              Update Usr Set DelegateUseID = @DelegateUseID
                              ,DelegateEffectiveFrom = @DelegateEffectiveFrom,DelegateEffectiveTo = @DelegateEffectiveTo
                              ,DelegateRemarks = @DelegateRemarks, isDelegate = 1
                              ,ChgUsr = @UseID, Chgdat = GETDATE() Where UseId = @UseID;
                              EXEC [Email_Delegate] @UseID, @DelegateUseID, @DelegateStatus = 'REMOVE DELEGATION'";
                }
                else
                {
                    query = @"DECLARE
	                           @DelegateUseID VARCHAR(50) = '" + DelegateUseID + @"'
	                          ,@DelegateEffectiveFrom VARCHAR(16) = '" + DelegateEffectiveFrom + @"'
	                          ,@DelegateEffectiveTo VARCHAR(16) = '" + DelegateEffectiveTo + @"'
	                          ,@DelegateRemarks NVARCHAR(MAX) = '" + DelegationRemarks + @"'
	                          ,@UseID VARCHAR(50) = '" + UseID + @"'

                              IF @DelegateEffectiveFrom = '' SET @DelegateEffectiveFrom = null
                              IF @DelegateEffectiveTo = '' SET @DelegateEffectiveTo = null

                              Update Usr Set DelegateUseID = @DelegateUseID
                              ,DelegateEffectiveFrom = @DelegateEffectiveFrom,DelegateEffectiveTo = @DelegateEffectiveTo
                              ,DelegateRemarks = @DelegateRemarks, isDelegate = 1
                              ,ChgUsr = @UseID, Chgdat = GETDATE() Where UseId = @UseID;
                              EXEC [Email_Delegate] @UseID ,@DelegateUseID,'DELEGATION'
                              ";
                }

                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                }
            }
            catch (Exception ex)
            {
                return new VMSRes<string>
                {
                    Success = false,
                    data = "",
                    Message = ex.Message
                };
            }

            return new VMSRes<string>
            {
                Success = true,
                data = "",
                Message = "Successfully save delegation."
            };
        }
        public VMSRes<User> GetDetailDelegation(string UserID)
        {
            DataTable dt = new DataTable();

            //Modified by CANICE 20210801
            //string query = @"SELECT UseID, UseNam, UseDep, u.BusFunc, UseLev, UsePlant, isDelegate, StartPage,ApprovalGroup, isAuthoriseSeeNumber
            //            FROM Usr u
            //			INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
            //            where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1";
            //      string query = @"SELECT UseID, UseNam, UseDep, u.BusFunc, UseLev, UsePlant, isDelegate, StartPage,ApprovalGroup, isAuthoriseSeeNumber
            //                  ,case when a.WindowsID is not null then convert(bit,'true') else convert(bit,'false') end as IsAuthorisedDelegate
            //                  ,u.DelegateUseID,Convert(nvarchar(10),isnull(u.DelegateEffectiveFrom,GETDATE()),121) DelegateEffectiveFrom,Convert(nvarchar(10),isnull(u.DelegateEffectiveTo,GETDATE()),121) DelegateEffectiveTo
            //                  FROM Usr u
            //INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
            //                  LEFT JOIN (select distinct a.WindowsID from ApprovingDept a
            //                              join ApprovingDesignation b on a.Designation = b.Designation and a.Status = 'Active' and ApprovingSequence > 0 and WindowsID <> ''
            //                              ) a on u.useid = a.WindowsID
            //                  where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1"; //change by suharjo ng 2021-11-02
            //End Modified 20210801
            //modify by suharjo ng 20211102
            string query = @"
                            --SET TO NULL IF EFFECTIVEDELEGATETO SMALL THAN TODAY
                            EXEC [dbo].[spUpdateDelegation]
                            -- END SET TO NULL

                            SELECT 
	                            UseID
	                            , UseNam
	                            , UseDep
	                            , u.BusFunc
	                            , UseLev
	                            , UsePlant
	                            , isDelegate
	                            , StartPage
	                            , ApprovalGroup
	                            , isAuthoriseSeeNumber
	                            ,case when a.WindowsID is not null then convert(bit,'true') else convert(bit,'false') end as IsAuthorisedDelegate
	                            ,u.DelegateUseID
	                            ,STUFF(replace(convert(varchar(10), ISNULL(DelegateEffectiveFrom, GETDATE()), 121)+convert(varchar(8), ISNULL(DelegateEffectiveFrom, GETDATE()), 114), ' ','T'), 11,0,'T') DelegateEffectiveFrom
	                            ,STUFF(replace(convert(varchar(10), ISNULL(DelegateEffectiveTo, GETDATE()), 121)+convert(varchar(8), ISNULL(DelegateEffectiveTo, GETDATE()), 114), ' ','T'), 11,0,'T') DelegateEffectiveTo
                                , CASE 
		                            WHEN ISNULL(DelegateEffectiveFrom,'') = '' AND ISNULL(DelegateEffectiveTo,'') = '' THEN 'StopDelegation'
		                            WHEN ISNULL(DelegateEffectiveTo, '') = '' THEN 'StartFrom'
		                            ELSE 'Range'
	                              END As DelegationPeriod
                                , u.DelegateRemarks
                            FROM Usr u
                            INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc 
                            OUTER APPLY (SELECT TOP 1 * FROM mmApprovalLevel WHERE WindowsID = u.UseID) a
                            where (UseID = @UseID or UseEmail = @UseID) and [isActiveAcc] = 1";
            //End Modified 20211102
            var parameters = new List<ctSqlVariable>()
            {
                new ctSqlVariable { Name="UseID",Type="String",Value=UserID }
            };
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, parameters, null, false);
            }

            var _data = (from row in dt.AsEnumerable()
                         select new User()
                         {
                             UseID = row["UseID"].ToString().ToUpper(),
                             UseNam = row["UseNam"].ToString().ToUpper(),
                             UseDep = row["UseDep"].ToString().ToUpper(),
                             BusFunc = row["BusFunc"].ToString().ToUpper(),
                             UseLev = row["UseLev"].ToString().ToUpper(),
                             isDelegate = row["isDelegate"].ToBoolean(),
                             UsePlant = row["UsePlant"].ToString().ToUpper(),
                             StartPage = row["StartPage"].ToString().ToUpper(),
                             ApprovalGroup = row["ApprovalGroup"].ToString().ToUpper(),
                             isAuthoriseSeeNumber = row["isAuthoriseSeeNumber"].ToBoolean(),
                             IsAuthorisedDelegate = row["IsAuthorisedDelegate"].ToBoolean(),    //Added by CANICE 20210801
                             DelegateUseID = row["DelegateUseID"].ToString().ToUpper(),                   //Added by CANICE 20210801
                             DelegateEffectiveFrom = row["DelegateEffectiveFrom"].ToString().ToUpper(),   //Added by CANICE 20210801
                             DelegateEffectiveTo = row["DelegateEffectiveTo"].ToString().ToUpper(),       //Added by CANICE 20210801
                             DelegationPeriod = row["DelegationPeriod"].ToString().ToUpper(),        //Added by Suharjo Ng 20211102
                             DelegateRemarks = row["DelegateRemarks"].ToString().ToUpper()        //Added by Suharjo Ng 20211102
                         }).FirstOrDefault();
            return new VMSRes<User>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _data
            };
        }
    }
    public class CountParam<T>
    {
        public T NameColumn { get; set; }
        public int Count { get; set; }

    }

    public class User
    {
        public string UseID { get; set; }
        public string UseNam { get; set; }
        public string UsePass { get; set; }
        public string UseDep { get; set; }
        public string UseLev { get; set; }
        public string UseEmail { get; set; }
        public string UseComCod { get; set; }
        public string UseIC { get; set; }
        public string UseTel { get; set; }
        public string BusFunc { get; set; }
        public DateTime CreDate { get; set; }
        public string CreUser { get; set; }
        public string ChgDate { get; set; }
        public string ChgUser { get; set; }
        public bool isDelegate { get; set; }
        public string UsePlant { get; set; }
        public string StartPage { get; set; }
        public bool isWindowsAuth { get; set; }
        public string Domain { get; set; }
        public bool isActiveAcc { get; set; }
        public string LastLoginDevice { get; set; }
        public string ApprovalGroup { get; set; }
        public bool isAuthoriseSeeNumber { get; set; }
        public bool IsAuthorisedDelegate { get; set; } //Added by CANICE 20210801
        public string DelegateUseID { get; set; } //Added by CANICE 20210801
        public string DelegateUseNam { get; set; } //Added by CANICE 20210801
        public string DelegateEffectiveFrom { get; set; } //Added by CANICE 20210801
        public string DelegateEffectiveTo { get; set; } //Added by CANICE 20210801
        public string DelegationPeriod { get; set; } //Added by Suharjo Ng 20211102
        public string DelegateRemarks { get; set; } //Added by Suharjo Ng 20211102
        public double Number { get; set; } //Added by Suharjo Ng 20211103
        public string Status { get; set; } //Added by Suharjo Ng 20211103
    }

    public class ApprovalGroup
    {
        public string Plant { get; set; }
        public string Group { get; set; }
        public string Dept { get; set; }
    }

    //Added by CANICE 20210801
    public class ApprovalDelegate
    {
        public string DelegateUseID { get; set; }
        public DateTime DelegateEffectiveFrom { get; set; }
        public DateTime DelegateEffectiveTo { get; set; }
        public string DelegateToUseID { get; set; }
        public string DelegateRemarks { get; set; }
    }
    //End Added 20210801
}