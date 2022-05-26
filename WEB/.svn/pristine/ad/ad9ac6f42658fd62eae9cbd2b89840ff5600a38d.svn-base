using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Linq.Dynamic;

using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Models;
using System.Data.OleDb;
using LinqToExcel;
using VMS.Library.Constants;

namespace VMS.Library.Actions
{
    public class TSVisitorAction 
    {
        //=======API
        Database SQLCon = new Database();
        public Visitor_TS GetVisitorFromBadge(string ShimanoBadge)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Id, [Name], [Department], [Email], [Phone]
                            FROM [VisitorTS] 
                            WHERE [ShimanoBadge] ='" + ShimanoBadge + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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

            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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

            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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

            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Temp_VisitorTS
                             {
                                 Id = Convert.ToInt16(rw["Id"].ToString()),
                                 Name = rw["Name"].ToString(),
                                 Department = rw["Department"].ToString()
                             }).ToList();
            return _visitors;

        }

        public MessageNonQuery PostNewWIFI(string Host, string Visitor,string UserName,string Password,string CreBy, string expTime= "23:59:59")
        {
            string query = @"

    DECLARE @WifiId nvarchar(5)= ''
SET @WifiId = (SELECT isnull(MAX(Id),0)+1 from WifiAccount)
INSERT INTO WifiAccount (Id, HostId, VisitorId, Username, [Password], TimeExpired, CreDate, CreUser)
        SELECT @WifiId, 
        '" + Host + @"', 
        '" + Visitor + @"', 
        '" + UserName + @"', 
        '" + Password + @"', 
        convert(time, '" + expTime + @"'), 
        CONVERT(nvarchar(16), GETDATE(),121),
        '" + CreBy + "'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
            
        public MessageNonQuery PostNewShimanoBadge(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @tempVisitor nvarchar(20)='" + postData.Temp_Visitor + @"'
                             DECLARE @ShimanoBadge nvarchar(20)='" + postData.ShimanoBadge + @"'
                             DECLARE @PhotoName nvarchar(20)='" + postData.PhotoName + @"'

INSERT INTO VisitorTS(Id, [Name], EmployeeNo,Department,ShimanoBadge,Plant,Photo,CreUser,CreDate)
SELECT (select isnull(max(id), 0)+1 from VisitorTS) as Id,
[Name],EmployeeNo,Department,@ShimanoBadge,Plant,@PhotoName,'INSERT FROM TABLET', GETDATE() from Temp_VisitorTS where id = @tempVisitor";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery PostChangeShimanoBadge(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @Id nvarchar(20)='" + postData.Temp_Visitor + @"'
                             DECLARE @ShimanoBadge nvarchar(20)='" + postData.ShimanoBadge + @"'

            UPDATE V set ShimanoBadge=@ShimanoBadge from VisitorTS V where Id=@Id";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public List<CodLst> GetCodLst(string GrpCod)
        {
            string query = "select Cod, CodAbb from CodLst where GrpCod='" + GrpCod + @"'";
            return (from rw in SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
                    select new CodLst
                    {
                        Cod = rw["Cod"].ToString(),
                        CodAbb = rw["CodAbb"].ToString()
                    }).ToList();
        }
        public MessageNonQuery PostVisitorCheckIn(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @ShimanoBadge nvarchar(50) = '"+ postData.ShimanoBadge+ @"'
                            DECLARE @Temp_Id nvarchar(50) = '" + postData.Temp_Visitor + @"'
                            DECLARE @NeedLunch nvarchar(50) = '" + postData.NeedLunch.ToString() + @"'
INSERT INTO VisitLog_TS
SELECT (SELECT ISNULL(MAX(LogId), 0)+1 from VisitLog_TS), (select Id from VisitorTS where ShimanoBadge = @ShimanoBadge),
HostName,HostDepartment,CONVERT(TIME, GETDATE(),103),null, Convert(nvarchar(10), GETDATE(), 121),@NeedLunch,'CHECKIN', plant, null
from Temp_VisitorTS where id=@Temp_Id";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery PostVisitorCheckOut(string LogId)
        {
            string query = @"DECLARE @LogId Nvarchar(10) = '"+ LogId + @"'
update VL set [Status]='CHECKOUT', [TimeOut]=CONVERT(TIME, GETDATE(),103) from VisitLog_TS VL where LogId=@LogId";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
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
ELSE IF EXISTS (select MnuCod from levelmenu where BusFunc in (select BusFunc from Usr where UseID=@ShimanoBadge) and MnuCod='REGWIFI') --Add by bondra
BEGIN
	SELECT '999999999' as 'VisitorId', @ShimanoBadge as Name, 'NULL' as Department, '0' as Temp_VisitorId,
	'WIFI' as Status 
END
ELSE
BEGIN
SELECT '0' as 'VisitorId', 'NONAME' as Name, 'NULL' as Department, '0' as Temp_VisitorId,
	'PENDING' as Status 
END";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
        //=======API


        //===================================================

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
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
            string query = @"DECLARE 
@dateFrom nvarchar(10) = '"+dateFrom+ @"',
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
[Status]
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(NVARCHAR(10), DateVisit, 126) between @dateFrom and @dateTo";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
                            NeedLunch = rw["NeedLunch"].ToString(),
                            Status = rw["Status"].ToString(),
                        }).ToList();
            return data;
        }

        public System.Data.DataTable GetDtTSReport(string dateFrom, string dateTo)
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
[Status]
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(NVARCHAR(10), DateVisit, 126) between @dateFrom and @dateTo";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
        public List<TSLogHistory> GetVisitorTSToday(string Name)
        {
            string query = @"DECLARE @Name nvarchar(50) = '"+ Name + @"'

SELECT V.Name,  CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as TimeIn,
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,  HostName + ' (' + HostDepartment + ')' as HostName, [Status]
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id and CONVERT(NVARCHAR(10), DateVisit, 126) = CONVERT(NVARCHAR(10), GETDATE(), 126)
where Name like '%' +@name+ '%'";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var data = (from rw in dt.AsEnumerable()
                        select new TSLogHistory
                        {
                            TimeStart = rw["TimeIn"].ToString(),
                            FullName = rw["Name"].ToString(),
                            Duration = rw["Duration"].ToString(),
                            HostName = rw["HostName"].ToString(),
                            Status = rw["Status"].ToString(),
                        }).ToList();
            return data;
        }
        public MessageNonQuery PostTempVisitor(string PathFile, string filename)
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
CONVERT(nvarchar(20),UploadDate,121) and CONVERT(nvarchar(20),DateOfEnd,121) and plant='" + row.Plant+@"')
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
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery SaveVisitorTemp(Temp_VisitorTS tempvisitor)
        {

            string query = @"
IF EXISTS(SELECT * FROM Temp_VisitorTS where Id='"+ tempvisitor.Id+ @"')
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
           ,'" + tempvisitor.Name+ @"'
           ,'" + tempvisitor.Department + @"'
           ,'" + tempvisitor.Plant + @"'
           ,'" + tempvisitor.EmployeeNo + @"'
           ,'" + tempvisitor.HostName + @"'
           ,'" + tempvisitor.HostDepartment + @"'
           ,GETDATE()
           ,'" + tempvisitor.DateManual.ToString("yyyy-MM-dd") + @"')
END
";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
    }
}
