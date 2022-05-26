using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using VMS.Library;

namespace VMS.Web.Models
{

    public class VisitorAction
    {
        DataTable dt;
        long num;
        string scalar;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public VMSRes<LogHistory> GetVisitorInArea(string CardId, string Plant, string ColumnName)
        {

            string query = $@"SELECT t1.LogId, t1.VisitorId, t2.fullName, t3.UseNam + ' (' + t3.UseDep+ ') Ext. ' + UseTel as UseNam, t2.Company,t1.[Status],
t4.Remark as RemarkVisitor
 FROM VISITLOG t1 
	                            INNER JOIN VISITOR  t2 on t1.VisitorId = t2.id
								INNER JOIN Usr t3 on t1.HostId = t3.UseId
								Left JOIN VisitLogDet t4 on t1.LogId=t4.LogId and t1.VisitorId=t4.VisitorId and 
								convert(varchar, [date], 23)=convert(varchar, getdate(), 23)
	                            where t1.{ColumnName} ='{CardId}' 
	                            and GETDATE() between t1.Datestart and DATEADD(DAY, 1, DateEnd)
	                            and t1.[Status] in ('BREAK', 'CHECKIN') and t1.Plant='{Plant}'";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = rw["LogId"].ToString(),
                              VisitorId = rw["VisitorId"].ToString(),
                              FullName = rw["fullName"].ToString(),
                              Company = rw["Company"].ToString(),
                              Status = rw["Status"].ToString(),
                              HostName = rw["UseNam"].ToString(),
                              Remark = rw["RemarkVisitor"].ToString()
                          }).FirstOrDefault();
            return new VMSRes<LogHistory>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _datas
            };
            
        }
        public VMSRes<Visitor> GetVisitorByPassCard(string PassCardNo, string ColumnFilter)
        {
            DataTable dt = new DataTable();
            string query = $@"SELECT id, FullName, Company, Phone, Email, JobDesc, Photo, VehicleNo
                            FROM Visitor 
                            WHERE {ColumnFilter} ='{PassCardNo}'";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Photo = rw["Photo"].ToString(),
                                 VehicleNo = rw["VehicleNo"].ToString()
                             }).FirstOrDefault();
            return new VMSRes<Visitor>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _visitors
            };
        }
        public VMSRes<HostAppointment> GetHostAppointment(string CardId, string Plant, string ColumnFilter)
        {
            DataTable dt = new DataTable();
            string query = $@"SELECT t2.LogId, t1.UseID, t1.UseNam, t1.UseDep, t1.UseTel, Area, t4.VisitorType, t2.[Status], Remark, TimeStart,TimeEnd, NeedApprove
                FROM Usr t1 
                INNER JOIN VisitLog t2 on t1.USeID=t2.HostId
                INNER JOIN Visitor t3 on t2.VisitorId = t3.Id
                INNER JOIN VisitorType t4 on t2.VisitType = t4.Id
                WHERE t3.{ColumnFilter} ='{CardId}' and t2.[Status] in ('PENDING', 'REJECT', 'APPROVED', 'CHECKOUT') and GETDATE() between Datestart and DATEADD(DAY, 1, DateEnd) and t3.isActive=1 and t2.Plant='{Plant}'";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            } 
            var _hostAppointment = (from rw in dt.AsEnumerable()
                                    select new HostAppointment
                                    {
                                        LogId = rw["LogId"].ToString(),
                                        UseID = rw["UseID"].ToString(),
                                        UseNam = rw["UseNam"].ToString(),
                                        UseDep = rw["UseDep"].ToString(),
                                        UseTel = rw["UseTel"].ToString(),
                                        Area = rw["Area"].ToString(),
                                        VisitorType = rw["VisitorType"].ToString(),
                                        Status = rw["Status"].ToString(),
                                        Remark = rw["Remark"].ToString(),
                                        TimeStart = rw["TimeStart"].ToString().Substring(0,5),
                                        TimeEnd = rw["TimeEnd"].ToString().Substring(0, 5),
                                        NeedApprove = rw["NeedApprove"].ToString(),
                                    }).FirstOrDefault();
            return new VMSRes<HostAppointment>()
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _hostAppointment
            };

        }
        public VMSRes<List<Visitor>> GetVisitorsForAppointment(string NameorCompany)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE @Plant varchar(4) = '{Sessions.GetUsePlant()}'
SELECT id, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No Appointment') as HostName,ISNULL(VL.Status, '0') as Status
	, VehicleNo FROM Visitor V
	LEFT OUTER JOIN VisitLog VL on V.id = VL.VisitorId and CONVERT(varchar(10), GETDATE(),121) 
								between CONVERT(varchar(10), vl.DateStart,121) 
								and CONVERT(varchar(10),vl.DateEnd,121)
								and Plant=@Plant
	LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
	                            WHERE (FullName 
	                            like '%{NameorCompany}%' or Company like '%{NameorCompany}%' or Phone like '%{NameorCompany}%') 
								and isActive=1
								order by fullname, Company ";

            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 VisitorCardNo = rw["VisitorCardNo"].ToString(),
                                 isActive = rw["isActive"].ToBoolean(),
                                 HostName = rw["HostName"].ToString(),
                                 Status = rw["Status"].ToString(),
                                 VehicleNo = rw["VehicleNo"].ToString()
                             }).ToList();
            return new VMSRes<List<Visitor>>
            {
                Success = dt.dtHavingRow(),
                data = _visitors
            };
        }
        public VMSRes<string> PostSaveAppointment(VisitLog VisitLog, string query)
        {

            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            Type model = typeof(VisitLog);
            PropertyInfo[] properties = model.GetProperties();
            foreach (var field in properties)
            {

                Console.WriteLine("{0} = {1}", field.Name, field.GetValue(VisitLog, null));
                ctSqlVariable param = new ctSqlVariable();
                if (field.GetValue(VisitLog, null) != null)
                {
                    param.Name = field.Name;
                    param.Type = field.PropertyType.FullName;
                    if (field.PropertyType.FullName.ToUpper().Contains(SqlDbType.DateTime.ToString().ToUpper()))
                    {
                        param.Value = Convert.ToDateTime(field.GetValue(VisitLog, null).ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        param.Value = field.GetValue(VisitLog, null).ToString();
                    }
                    paramss.Add(param);
                }

            }
            string queryScalar = "select MAX(LogId) from VisitLog";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecuteStoProcDT(ConnectionDB, query, paramss);
                scalar = s.ExecuteScalar(ConnectionDB, queryScalar, null, null, false);
            }
            return dt.OneRowdtToResult(scalar);
        }
        public VMSRes<string> GetVisitorByPassCard(string PassCardNo)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT FullName + ' FROM ' +  Company as VisitorName FROM Visitor 
                            WHERE VisitorCardNo ='" + PassCardNo + "'";
            using (var sql = new MSSQL())
            {
                scalar = sql.ExecuteScalar(ConnectionDB, query, null, null, false);
            }
            return scalar.ScalarResult();
        }
        public VMSRes<string> PostUpdateCardRegister(int Id, string CardId)
        {
            string query = "UPDATE visitor set VisitorCardNo ='" + CardId + "' where id='" + Id.ToString() + "'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostPhotoVisitor(int idVisitor, string pathPhoto)
        {
            string query = "UPDATE visitor set Photo ='" + pathPhoto + "' where id='" + idVisitor.ToString() + "'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostUpdateStatus(string Id, string LogId, string CardId, string Query, string Username = "", string Method = "", string Remark = "")
        {
            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            paramss.Add(new ctSqlVariable { Name = "Id", Value = Id, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "CardId", Value = CardId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Username", Value = Username, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Method", Value = Method, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Remark", Value = Remark, Type = "String" });

            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "VisitorCheckIn", paramss);
            }
            return dt.OneRowdtToResult();
        }
        public VMSRes<Visitor> GetVisitorDetail(string Id)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, FullName, Company, Phone, Email, JobDesc, Photo, VehicleNo
                            FROM Visitor 
                            WHERE id ='" + Id + "'";
            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 VehicleNo = rw["VehicleNo"].ToString()
                             }).FirstOrDefault();
            return new VMSRes<Visitor>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _visitors
            };
        }
        public VMSRes<string> GetCheckVisitorName(string Name, string Company, string JobDesc)
        {
            string query = @"select top 1 FullName + '(' + Company + ')' as FullName from Visitor where Fullname='" + Name + "' and Company = '" + Company + @"' ";
            using (var sql = new MSSQL())
            {
                scalar = sql.ExecuteScalar(ConnectionDB, query, null,null, false);
            }
            return scalar.ScalarResult();
        }

        //
        public VMSRes<string> PostAddEditVisitor(Visitor visitor)
        {
            string query = $@"IF EXISTS(SELECT * FROM VISITOR where Id='{visitor.id}')
BEGIN 
	UPDATE Visitor Set fullName='{visitor.FullName.ToUpper()}',
 Company='{visitor.Company.ToUpper() }',
 JobDesc='{(string.IsNullOrEmpty(visitor.JobDesc) ? "" : visitor.JobDesc.ToUpper())}',
 Phone='" + visitor.Phone + @"',
 email='" + visitor.Email + @"',
 VehicleNo='" + visitor.VehicleNo + @"',
 UpdateBy='" + visitor.UpdateBy + @"',
 UpdateAt=GETDATE() where id='" + visitor.id + @"'
END
else
BEGIN
	DECLARE 
	@maxId int
	set @MaxId = (select isnull (max(cast(id as int))+1,1) from Visitor)
	INSERT INTO Visitor([id],
            [fullName]
           ,[Company]
           ,[JobDesc]
           ,[phone]
           ,[email]
           ,[UpdateBy]
           ,[UpdateAt], isActive, VehicleNo) values(@MaxId, '" + visitor.FullName.ToUpper() + @"',
            '" + visitor.Company.ToUpper() + @"',
            '" + (string.IsNullOrEmpty(visitor.JobDesc) ? "" : visitor.JobDesc.ToUpper())
            + @"',
            '" + visitor.Phone + @"',
            '" + visitor.Email + @"',
            '" + visitor.UpdateBy + @"',
            GETDATE(), 1, '"+visitor.VehicleNo + @"')
END";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<Visitor> GetListVisitorAfterInvite(string UserName)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT VisitorId, FullName, Company, JobDesc 
	            from temp_visitor t1 INNER JOIN 
                Visitor t2 on t1.VisitorId = t2.Id where hostId = '" + UserName + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["VisitorId"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString()
                             }).ToList();
            return _visitors;
        }
        public List<Visitor> GetListVisitorBeforeInvite(string Name, string Company, string Plant, string dateFrom, string dateTo)
        {
            DataTable dt = new DataTable();
            string query = @"EXEC [GetVisitorListAppointment] 
                        @Name='" + Name + @"',
                        @Company='" + Company + @"',
                        @Plant='" + Plant + @"',
                        @dateFrom='" + dateFrom + @"',
                        @dateTo= '" + dateTo + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Remark = rw["Remark"].ToString(),
                                 Photo = rw["Photo"].ToString()
                             }).ToList();
            return _visitors;
        }
        public VMSRes<string> PostAddVisitorAppointment(int Id, string UserName)
        {
            string query = "INSERT INTO temp_visitor values('" + Id + "', '" + UserName + "')";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> DeleteVisitorAppointment(int Id, string UserName)
        {
            string query = "DELETE FROM temp_visitor where VisitorId='" + Id + "' and HostId='" + UserName + "'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<RegisterWifi> GetListRegister(string Name, string DateFrom, string DateTo)
        {
            string query = @"
    DECLARE @Name nvarchar(max) = '" + Name + @"'
SELECT WA.Id, UH.UseNam as HostName, V.FullName, UC.UseNam as CreName, WA.Username, WA.Password,Convert(nvarchar(10),WA.CreDate,121) as CreDate, TimeExpired 
FROM WifiAccount WA
	LEFT JOIN Usr UH on WA.HostId = UH.UseID
	LEFT JOIN Visitor V on CONVERT(VARCHAR,WA.VisitorId) = CONVERT(VARCHAR,V.Id)
	LEFT JOIN Usr UC on WA.CreUser = UC.UseID 
    where (UH.UseNam like '%'+ @Name +'%'
        OR Fullname like '%'+ @Name +'%')
        and CONVERT(nvarchar(10), WA.CreDate, 121) between
        CONVERT(nvarchar(10), '" + DateFrom + @"' ,121) and CONVERT(nvarchar(10), '" + DateTo + @"' ,121)  and V.isActive=1
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return (from rw in dt.AsEnumerable()
                    select new RegisterWifi
                    {
                        Id = int.Parse(rw["Id"].ToString()),
                        HostName = rw["HostName"].ToString(),
                        FullName = rw["FullName"].ToString(),
                        CreName = rw["CreName"].ToString(),
                        Username = rw["Username"].ToString(),
                        Password = rw["Password"].ToString(),
                        CreDate = rw["CreDate"].ToString(),
                        sTimeExpired = rw["TimeExpired"].ToString(),
                    }).ToList();
        }
        public VMSRes<string> PostRegisterWifi(RegisterWifi RegisterWifi)
        {
            string query = @"

    DECLARE @WifiId nvarchar(5)= ''
SET @WifiId = (SELECT isnull(MAX(Id),0)+1 from WifiAccount)
INSERT INTO WifiAccount (Id, HostId, VisitorId, Username, [Password], TimeExpired, CreDate, CreUser)
        SELECT @WifiId, 
        '" + RegisterWifi.HostId + @"', 
        '" + RegisterWifi.VisitorId + @"', 
        '" + RegisterWifi.Username + @"', 
        '" + RegisterWifi.Password + @"', 
        '" + RegisterWifi.TimeExpired + @"', 
        CONVERT(nvarchar(16), GETDATE(),121),
        '" + RegisterWifi.CreUser + @"'
EXEC dbo.RegisterWifiEmail @WifiId";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<Visitor> GetVisitorForWifi(string HostId)
        {
            string query = @"select DISTINCT V.Id, V.FullName from VisitLog VL 
INNER JOIN 
Visitor V on VL.VisitorId = V.id
where status='CHECKIN' and CONVERT(nvarchar(10),GETDATE(),121) between 
CONVERT(nvarchar(10),DateStart,121) and CONVERT(nvarchar(10),DateEnd,121) and HostId='" + HostId + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return (from rw in dt.AsEnumerable()
                    select new Visitor
                    {
                        id = int.Parse(rw["Id"].ToString()),
                        FullName = rw["FullName"].ToString(),
                    }).ToList();
        }
        public List<User> GetHostForWifi()
        {
            string query = @"select DISTINCT UH.UseID, UH.UseNam from VisitLog VL 
INNER JOIN 
Usr UH on VL.HostID = UH.UseID
where status='CHECKIN' and CONVERT(nvarchar(10),GETDATE(),121) between 
CONVERT(nvarchar(10),DateStart,121) and CONVERT(nvarchar(10),DateEnd,121)";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return (from rw in dt.AsEnumerable()
                    select new User
                    {
                        UseID = rw["UseID"].ToString(),
                        UseNam = rw["UseNam"].ToString(),
                    }).ToList();
        }
        public VMSRes<dynamic> GetDateMinMax()
        {
            string query = $@"select TIMESTARTMAX, TIMESTARTMIN, TIMEENDMAX,TIMEENDMIN
from
(
  select cod as [Desc], CodDes as [Time] from CodLst where GrpCod = 'VISITORCONFIGTIME'
) d
pivot
(
  max(TIME)
  for [DESC] in ( TIMESTARTMAX, TIMESTARTMIN, TIMEENDMAX,TIMEENDMIN)
) piv;";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var data = (from rw in dt.AsEnumerable()
                        select new
                        {
                            TIMEENDMAX = rw["TIMEENDMAX"].ToString(),
                            TIMEENDMIN = rw["TIMEENDMIN"].ToString(),
                            TIMESTARTMAX = rw["TIMESTARTMAX"].ToString(),
                            TIMESTARTMIN = rw["TIMESTARTMIN"].ToString(),
                        }).FirstOrDefault();
            return new VMSRes<dynamic>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = data
            };
        }
        public List<Visitor> GetAllVisitor(string NameorCompany)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE @Plant varchar(4) = '{Sessions.GetUsePlant()}'
SELECT id, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No AppointMent') as HostID,ISNULL(VL.Status, '0') as Status
	, VehicleNo FROM Visitor V
	LEFT OUTER JOIN VisitLog VL on V.id = VL.VisitorId and CONVERT(varchar(10), GETDATE(),121) 
								between CONVERT(varchar(10), vl.DateStart,121) 
								and CONVERT(varchar(10),vl.DateEnd,121)
								and Plant=@Plant
	LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
	                            WHERE (FullName 
	                            like '%{NameorCompany}%' or Company like '%{NameorCompany}%') 
								and isActive=1
								order by fullname, Company ";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }


            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 VisitorCardNo = rw["VisitorCardNo"].ToString(),
                                 isActive = rw["isActive"].ToBoolean(),
                                 VehicleNo = rw["VehicleNo"].ToString()
                             }).ToList();
            return _visitors;
        }
        public List<Visitor> GetVisitorMasterData(string Name)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE @Plant varchar(4) = '{Sessions.GetUsePlant()}'
SELECT id, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No AppointMent') as HostID,ISNULL(VL.Status, '0') as Status
	, VehicleNo FROM Visitor V
	LEFT OUTER JOIN VisitLog VL on V.id = VL.VisitorId and CONVERT(varchar(10), GETDATE(),121) 
								between CONVERT(varchar(10), vl.DateStart,121) 
								and CONVERT(varchar(10),vl.DateEnd,121)
								and Plant=@Plant
	LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
	                            WHERE (FullName 
	                            like '%{Name}%' or Company like '%{Name}%') 
								order by fullname, Company ";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }


            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 VisitorCardNo = rw["VisitorCardNo"].ToString(),
                                 isActive = rw["isActive"].ToBoolean(),
                                 VehicleNo = rw["VehicleNo"].ToString()
                             }).ToList();
            return _visitors;
        }
        public DataTable GetTemplateVisitor()
        {
            //Modified by CANICE 20210407
//            string query = @"select top 10  FullName, Company,JobDesc,'' phone,'' email, '' as VehicleNo from Visitor where ID in 
//(select top 10 percent ID from Visitor order by newid())";
            string query = @"select top 10  FullName, Company,JobDesc ,'' Phone,'' Email, '' as VehicleNo, '' as [Host-WindowsID] from Visitor where ID in 
(select top 10 percent ID from Visitor order by newid())";
            //End Modified 20210407
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public VMSRes<string> PostUploadVisitor(string PhysicalPath, string SheetName, string DateStart, string DateEnd, string HostId)
        {
            List<Visitor> listVisitor = SysUtil.GetObjectFromExcel<Visitor>(PhysicalPath, SheetName);
            int countSuccess = 0, countFail = 0;
            List<string> _message = new List<string>();
            foreach(var row in listVisitor)
            {
                var para = new List<ctSqlVariable>();
                para.Add(new ctSqlVariable { Name = "VisitorName", Type = "String", Value = row.FullName.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "Company", Type = "String", Value = row.Company.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "JobDesc", Type = "String", Value = row.JobDesc.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "Phone", Type = "String", Value = row.Phone.IsStringNull() });
                para.Add(new ctSqlVariable { Name = "Email", Type = "String", Value = row.Email.IsStringNull() });
                para.Add(new ctSqlVariable { Name = "DateStart", Type = "String", Value = DateStart });
                para.Add(new ctSqlVariable { Name = "DateEnd", Type = "String", Value = DateEnd });
                para.Add(new ctSqlVariable { Name = "VehicleNo", Type = "String", Value = row.VehicleNo.IsStringNull() });
                para.Add(new ctSqlVariable { Name = "HostId", Type = "String", Value = row.HostId.IsStringNull() == "" ? HostId : row.HostId.IsStringNull() }); //Modified by CANICE 20210407 //para.Add(new ctSqlVariable { Name = "HostId", Type = "String", Value = Sessions.GetUseID() });
                using (var sql = new MSSQL())
                {
                    dt = sql.ExecuteStoProcDT(ConnectionDB, "[GetOrMakeAppointmentFromExcel]", para);
                }
                var result = dt.OneRowdtToResult();
                _message.Add(result.Message);
                if (result.Success) countSuccess++; else countFail++;
            }
            var finalMessage =
                    $@"Total Success={countSuccess.ToString()}, Total Fail={countFail}<br/>Remark : <br/>"
                    + string.Join("<br/>", _message);
            return new VMSRes<string>
            {
                Success = countFail == 0 ? true : false,
                Message = finalMessage,
                data = "true"
            };
        }
    }
    public class Visitor
    {
        public int id { get; set; }
        public string VisitorCardNo { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Company { get; set; }
        public string JobDesc { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public bool isActive { get; set; }
        public string HostName { get; set; }
        public string Status { get; set; }
        public string VehicleNo { get; set; }
        public string LogId { get; set; }
        public string HostId { get; set; }
        public bool Unknown { get; set; }
        public string DisclaimerStatus { get; set; }
        public string IsRead { get; set; }

    }
}