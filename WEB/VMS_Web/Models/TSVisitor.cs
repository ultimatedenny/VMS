using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using VMS.Library;

namespace VMS.Web.Models
{
    public class TSVisitorAction
    {
        DataTable dt;
        long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public Visitor_TS GetVisitorFromBadge(string ShimanoBadge)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Id, [Name], [Department], [Email], [Phone]
                            FROM [VisitorTS] 
                            WHERE [ShimanoBadge] ='" + ShimanoBadge + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor_TS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Department = rw["Department"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString()
                             }).FirstOrDefault();
            return _visitors;
        }
        public List<Visitor_TS> GetListVisitorTS(string Name)
        {
            DataTable dt = new DataTable();
            string query = @"select Id, Name, Plant from VisitorTS where Name like '%" + Name + @"%' ";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor_TS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Plant = rw["Plant"].ToString()
                             }).ToList();
            return _visitors;

        }
        public Visitor_TS GetVisitorTSForCheck(string ShimanoBadge)
        {
            DataTable dt = new DataTable();
            string query = @"select Id, Name, Plant from VisitorTS where ShimanoBadge = '" + ShimanoBadge + @"' ";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor_TS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Plant = rw["Plant"].ToString()
                             }).FirstOrDefault();
            return _visitors;

        }

        public List<Temp_VisitorTS> GetTemp_VisitorList(string Name)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT TV.Id, TV.[Name], TV.[Department] FROM [temp_VisitorTS]  TV
LEFT JOIN VisitorTS V on TV.Name = V.Name and TV.Department = V.Department 
	                            WHERE V.Id is null and TV.Name like '%" + Name + @"%' 
								and convert(nvarchar(20),GETDATE(),121) between convert(nvarchar(20),TV.UploadDate,121) and convert(nvarchar(20),TV.DateOfEnd,121) ";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitors = (from rw in dt.AsEnumerable()
                             select new Temp_VisitorTS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Department = rw["Department"].ToString()
                             }).ToList();
            return _visitors;

        }
        public VMSRes<string> PostNewShimanoBadge(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @tempVisitor nvarchar(20)='" + postData.Temp_Visitor + @"'
                             DECLARE @ShimanoBadge nvarchar(20)='" + postData.ShimanoBadge + @"'
                             DECLARE @PhotoName nvarchar(20)='" + postData.PhotoName + @"'

INSERT INTO VisitorTS(Id, [Name], EmployeeNo,Department,ShimanoBadge,Plant,Photo,CreUser,CreDate)
SELECT (select isnull(max(id), 0)+1 from VisitorTS) as Id,
[Name],EmployeeNo,Department,@ShimanoBadge,Plant,@PhotoName,'INSERT FROM TABLET', GETDATE() from Temp_VisitorTS where id = @tempVisitor";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostChangeShimanoBadge(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @Id nvarchar(20)='" + postData.Temp_Visitor + @"'
                             DECLARE @ShimanoBadge nvarchar(20)='" + postData.ShimanoBadge + @"'

            UPDATE V set ShimanoBadge=@ShimanoBadge from VisitorTS V where Id=@Id";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<CodLst> GetCodLst(string GrpCod)
        {
            string query = "select Cod, CodAbb from CodLst where GrpCod='" + GrpCod + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            return (from rw in dt.AsEnumerable()
                    select new CodLst
                    {
                        Cod = rw["Cod"].ToString(),
                        CodAbb = rw["CodAbb"].ToString()
                    }).ToList();
        }
        public VMSRes<string> PostVisitorCheckIn(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @ShimanoBadge nvarchar(50) = '" + postData.ShimanoBadge + @"'
                            DECLARE @Temp_Id nvarchar(50) = '" + postData.Temp_Visitor + @"'
                            DECLARE @NeedLunch nvarchar(50) = '" + postData.NeedLunch.ToString() + @"'
INSERT INTO VisitLog_TS
SELECT (SELECT ISNULL(MAX(LogId), 0)+1 from VisitLog_TS), (select Id from VisitorTS where ShimanoBadge = @ShimanoBadge),
HostName,HostDepartment,CONVERT(TIME, GETDATE(),103),null, Convert(nvarchar(10), GETDATE(), 121),@NeedLunch,'CHECKIN', plant, null
from Temp_VisitorTS where id=@Temp_Id";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostVisitorCheckOut(string LogId)
        {
            string query = @"DECLARE @LogId Nvarchar(10) = '" + LogId + @"'
update VL set [Status]='CHECKOUT', [TimeOut]=CONVERT(TIME, GETDATE(),103) from VisitLog_TS VL where LogId=@LogId";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VisitorJoinLog GetorPostVisitorTS(string ShimanoBadge)
        {
            string query = @"DECLARE @ShimanoBadge as nvarchar(20) = '" + ShimanoBadge + @"'

IF EXISTS(SELECT 1 FROM VisitorTS V INNER JOIN VisitLog_TS VL on V.Id = VL.TSVisitorId where ShimanoBadge=@ShimanoBadge and VL.Status = 'CHECKIN' and CONVERT(nvarchar(10),DateVisit,121) = CONVERT(nvarchar(10),GETDATE(),121))
BEGIN
SELECT V.Id as VisitorId, V.Name,V.Department, LogId as Temp_VisitorId, 
Status FROM VisitorTS V INNER JOIN VisitLog_TS VL on V.Id = VL.TSVisitorId 
where ShimanoBadge=@ShimanoBadge and CONVERT(nvarchar(10),DateVisit,121) = CONVERT(nvarchar(10),GETDATE(),121) AND status='CHECKIN'
END
ELSE if exists(SELECT 1  from VisitorTS V
	LEFT OUTER JOIN temp_VisitorTS TV on V.Name = TV.Name 
	and CONVERT(nvarchar(10),GETDATE(),121) between CONVERT(nvarchar(10),UploadDate,121) 
	and CONVERT(nvarchar(10),DateOfEnd,121) where ShimanoBadge=@ShimanoBadge)
BEGIN
	SELECT V.Id as 'VisitorId', V.Name, V.Department, isnull(TV.Id, '0') as Temp_VisitorId,
	'PENDING' as Status from VisitorTS V
	LEFT OUTER JOIN temp_VisitorTS TV on V.Name = TV.Name 
	and CONVERT(nvarchar(10),GETDATE(),121) between CONVERT(nvarchar(10),UploadDate,121) 
	and CONVERT(nvarchar(10),DateOfEnd,121 )
	where ShimanoBadge=@ShimanoBadge
END
ELSE
BEGIN
SELECT '0' as 'VisitorId', 'NONAME' as Name, 'NULL' as Department, '0' as Temp_VisitorId,
	'PENDING' as Status 
END";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var data = (from rw in dt.AsEnumerable()
                        select new VisitorJoinLog
                        {
                            VisitorId = rw["VisitorId"].ToString(),
                            Name = rw["Name"].ToString(),
                            Department = rw["Department"].ToString(),
                            Temp_VisitorId = rw["Temp_VisitorId"].ToString(),
                            Status = rw["Status"].ToString(),
                        }).FirstOrDefault();
            return data;
        }
        public List<Temp_VisitorTS> GetTempVisitor(string dateFrom, string dateTo, string name)
        {
            DataTable dt = new DataTable();
            string query = @"
            DECLARE @name nvarchar(max) = '" + name + @"'
SELECT [Id]
                          ,[Name]
                          ,[Department]
                          ,[Plant]
                          ,[EmployeeNo]
                          ,[HostName]
                          ,[HostDepartment]
                          ,CONVERT(nvarchar(10),[UploadDate], 121) as UploadDate
                          ,CONVERT(nvarchar(10),[DateOfEnd], 121) [DateOfEnd]
                           FROM [temp_VisitorTS] WHERE ([Name] like '%' + @name + '%' or [HostName] like '%' + @name + '%') and 
 CONVERT(nvarchar(10), UploadDate, 121) between '" + dateFrom + "' and '" + dateTo + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitors = (from rw in dt.AsEnumerable()
                             select new Temp_VisitorTS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Department = rw["Department"].ToString(),
                                 Plant = rw["Plant"].ToString(),
                                 EmployeeNo = rw["EmployeeNo"].ToString(),
                                 HostName = rw["HostName"].ToString(),
                                 HostDepartment = rw["HostDepartment"].ToString(),
                                 UploadDate = rw["UploadDate"].ToString(),
                                 DateEnd = rw["DateOfEnd"].ToString(),
                             }).ToList();
            return _visitors;
        }
        public List<TSLogHistory> GetTSReport(string dateFrom, string dateTo)
        {
            string query = $@"DECLARE 
@dateFrom nvarchar(10) = '{dateFrom}',
@dateTo nvarchar(10) = '{dateTo}'

SELECT 
HostName + ' (' + HostDepartment + ')' as HostName,
V.Name + ' (' + V.Department + ')' as Name,
VL.Plant,
isnull(Remark, 'NA') as Remark,
convert(varchar(10),DateVisit, 121) as DateVisit,
CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as TimeIn,
CONVERT(varchar,isnull(TimeOut ,'00:00'), 108) as TimeOut,
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,   
[Status], NeedLunch, NeedStay,convert(varchar(10),DateOfEnd, 121) as StayDate
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(NVARCHAR(10), DateVisit, 126) between @dateFrom and @dateTo order by DateVisit";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var data = (from rw in dt.AsEnumerable()
                        select new TSLogHistory
                        {
                            HostName = rw["HostName"].ToString(),
                            FullName = rw["Name"].ToString(),
                            Plant = rw["Plant"].ToString(),
                            DateStart = rw["DateVisit"].ToString(),
                            Remark = rw["Remark"].ToString(),
                            TimeStart = rw["TimeIn"].ToString(),
                            TimeEnd = rw["TimeOut"].ToString(),
                            Duration = rw["Duration"].ToString(),
                            Status = rw["Status"].ToString(),
                            NeedLunch = rw["NeedLunch"].ToBoolean(),
                            NeedStay = rw["NeedStay"].ToBoolean(),
                            StayDate = rw["StayDate"].ToString()
                        }).ToList();
            return data;
        }
        public DataTable GetExportExcel(string dateFrom, string dateTo)
        {
            string query = $@"DECLARE 
@dateFrom nvarchar(10) = '{dateFrom}',
@dateTo nvarchar(10) = '{dateTo}'

SELECT 
VL.Plant,
V.Name + ' (' + V.Department + ')' as [Visitor Name],
 HostName + ' (' + HostDepartment + ')' as [Host Name],
isnull(Remark, 'NA') as Remark,
convert(varchar(10),DateVisit, 121) as [Date Visit],
CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as [Time In],
CONVERT(varchar,isnull(TimeOut ,'00:00'), 108) as [Time Out],
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,   
CASE NeedLunch WHEN 1 then 'YES' else 'NO' end as [Need Lunch?], [Status]
, CASE NeedStay WHEN 1 then 'YES' else 'NO' end as [Need Stay?],convert(varchar(10),DateOfEnd, 121) as StayDate
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(NVARCHAR(10), DateVisit, 126) between @dateFrom and @dateTo order by DateVisit, TimeIn";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public Temp_VisitorTS GetTSVisitorTempDet(string Id)
        {
            string query = @"SELECT [Id]
      ,[Name]
      ,[Department]
      ,[Plant]
      ,[EmployeeNo]
      ,[HostName]
      ,[HostDepartment]
      ,[UploadDate]
      ,[DateOfEnd]
  FROM [temp_VisitorTS] where Id = '" + Id + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var data = (from rw in dt.AsEnumerable()
                        select new Temp_VisitorTS
                        {
                            Id = int.Parse(rw["Id"].ToString()),
                            Name = rw["Name"].ToString(),
                            Department = rw["Department"].ToString(),
                            Plant = rw["Plant"].ToString(),
                            EmployeeNo = rw["EmployeeNo"].ToString(),
                            HostName = rw["HostName"].ToString(),
                            HostDepartment = rw["HostDepartment"].ToString(),
                            DateEnd = rw["DateOfEnd"].ToString(),
                        }).FirstOrDefault();
            return data;
        }
        public DataTable GetDtTSReport(string dateFrom, string dateTo)
        {
            string query = @"DECLARE 
@dateFrom nvarchar(10) = '" + dateFrom + @"',
@dateTo nvarchar(10) = '" + dateTo + @"'

SELECT 
HostName + ' (' + HostDepartment + ')' as HostName,
V.Name + ' (' + V.Department + ')' as Name,
VL.Plant,
isnull(Remark, 'NA') as Remark,
convert(varchar(10),DateVisit, 121) as DateVisit,
CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as TimeIn,
CONVERT(varchar,isnull(TimeOut ,'00:00'), 108) as TimeOut,
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,   
case when VL.NeedLunch=1 then 'Yes' else 'No' end as NeedLunch, 
case when VL.NeedStay=1 then 'Yes' else 'No' end as NeedStay,
[Status]
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(date, DateVisit) between CONVERT(date,@dateFrom) and CONVERT(date,@dateTo) order by DateVisit";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public List<TSLogHistory> GetVisitorTSToday(string Name)
        {
            string query = @"DECLARE @Name nvarchar(50) = '" + Name + @"'

SELECT V.Name,  CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as TimeIn,
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,  HostName + ' (' + HostDepartment + ')' as HostName, [Status], NeedLunch
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id and CONVERT(NVARCHAR(10), DateVisit, 126) = CONVERT(NVARCHAR(10), GETDATE(), 126)
where Name like '%' +@name+ '%'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var data = (from rw in dt.AsEnumerable()
                        select new TSLogHistory
                        {
                            TimeStart = rw["TimeIn"].ToString(),
                            FullName = rw["Name"].ToString(),
                            Duration = rw["Duration"].ToString(),
                            HostName = rw["HostName"].ToString(),
                            Status = rw["Status"].ToString(),
                            NeedLunch = rw["NeedLunch"].ToBoolean()
                        }).ToList();
            return data;
        }
        public VMSRes<string> PostTempVisitor(string PathFile, string filename)
        {
            string query = "";
            var connectionString = "";
            if (filename.EndsWith(".xls"))
            {
                connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", PathFile);
            }
            else if (filename.EndsWith(".xlsx"))
            {
                connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", PathFile);
            }
            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
            var ds = new DataSet();
            adapter.Fill(ds, "ExcelTable");

            DataTable dtable = ds.Tables["ExcelTable"];
            string sheetName = "Sheet1";
            var excelFile = new ExcelQueryFactory(PathFile);
            var Visitors = from a in excelFile.Worksheet<Template_UploadVsitor>(sheetName)
                           select a;
            foreach (var row in Visitors)
            {
                query += @" 
IF NOT EXISTS(SELECT Id FROM Temp_Visitorts where Name='" + row.Name + @"' and Department='" + row.Department + @"' and CONVERT(nvarchar(20),GETDATE(),121)
 between
CONVERT(nvarchar(20),UploadDate,121) and CONVERT(nvarchar(20),DateOfEnd,121) and plant='" + row.Plant + @"')
BEGIN
INSERT INTO [dbo].[temp_VisitorTS]
           ([Id]
           ,[Name]
           ,[Department]
           ,[Plant]
           ,[EmployeeNo]
           ,[HostName]
           ,[HostDepartment]
           ,[UploadDate]
           ,[DateOfEnd])
     VALUES
           ((SELECT ISNULL(MAX(Id)+1 ,1) from temp_visitorTS)
           ,'" + row.Name + @"'
           ,'" + row.Department + @"'
           ,'" + row.Plant + @"'
           ,NULL
           ,'" + row.HostName + @"'
           ,'" + row.HostDepartment + @"'
           ,CONVERT(nvarchar(10), GETDATE(),121)
           ,'" + row.DateofEnd.ToString("yyyy-MM-dd") + @"')

END


";
            }

            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> SaveVisitorTemp(Temp_VisitorTS tempvisitor)
        {

            string query = @"
IF EXISTS(SELECT * FROM Temp_VisitorTS where Id='" + tempvisitor.Id + @"')
BEGIN
UPDATE [dbo].[temp_VisitorTS]
   SET [Name] = '" + tempvisitor.Name + @"'
      ,[Department] = '" + tempvisitor.Department + @"'
      ,[Plant] = '" + tempvisitor.Plant + @"'
      ,[EmployeeNo] = '" + tempvisitor.EmployeeNo + @"'
      ,[HostName] = '" + tempvisitor.HostName + @"'
      ,[HostDepartment] = '" + tempvisitor.HostDepartment + @"'
      ,[DateOfEnd] = '" + tempvisitor.DateManual.ToString("yyyy-MM-dd") + @"'
 WHERE Id='" + tempvisitor.Id + @"'
END
ELSE
BEGIN 
INSERT INTO [dbo].[temp_VisitorTS]
           ([Id]
           ,[Name]
           ,[Department]
           ,[Plant]
           ,[EmployeeNo]
           ,[HostName]
           ,[HostDepartment]
           ,[UploadDate]
           ,[DateOfEnd])
     VALUES
           ((SELECT ISNULL(MAX(Id)+1 , 1) from Temp_VisitorTS)
           ,'" + tempvisitor.Name + @"'
           ,'" + tempvisitor.Department + @"'
           ,'" + tempvisitor.Plant + @"'
           ,'" + tempvisitor.EmployeeNo + @"'
           ,'" + tempvisitor.HostName + @"'
           ,'" + tempvisitor.HostDepartment + @"'
           ,GETDATE()
           ,'" + tempvisitor.DateManual.ToString("yyyy-MM-dd") + @"')
END
";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> DeleteTempVisitor(string id)
        {
            string query = $@"DELETE from Temp_visitorTS where id ='{id}'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
    }
    public class TSVisitor
    {
    }
    public class Visitor_TS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string Department { get; set; }
        public string ShimanoBadge { get; set; }
        public string Plant { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Photoes { get; set; }
        public string CreUser { get; set; }
        public string CreDate { get; set; }
        public string ChgUser { get; set; }
        public string ChgDate { get; set; }
    }
    public class VisitLog_TS
    {
        public int LogId { get; set; }
        public int TSVisitorId { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Plant { get; set; }
        public string DateVisit { get; set; }
        public string NeedLunch { get; set; }
        public string Status { get; set; }
    }
    public class Temp_VisitorTS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string EmployeeNo { get; set; }
        public string Plant { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string DateEnd { get; set; }
        public string UploadDate { get; set; }
        public DateTime DateManual { get; set; }

    }
    public class Template_UploadVsitor
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string Plant { get; set; }
        public DateTime DateofTravel { get; set; }
        public DateTime DateofEnd { get; set; }

    }
    public class VisitorJoinLog
    {
        public string VisitorId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Temp_VisitorId { get; set; }
        public string Status { get; set; }

    }
    public class ShimanoBadgeModel
    {
        public string ShimanoBadge { get; set; }
        public string Temp_Visitor { get; set; }
        public bool NeedLunch { get; set; }
        public string PhotoName { get; set; }
    }
    public class PhotoAndroid
    {
        public byte[] photoData { get; set; }
        public string photoName { get; set; }
    }
    public class WifiAccount
    {
        public string WifiName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class TSLogHistory
    {
        public string VisitorId { get; set; }
        public string LogId { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public string Plant { get; set; }
        public string Remark { get; set; }
        public string Area { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public string Phone { get; set; }
        public string NeedApprove { get; set; }
        public bool NeedLunch { get; set; }
        public bool NeedStay { get; set; }
        public string StayDate { get; set; }
    }

}