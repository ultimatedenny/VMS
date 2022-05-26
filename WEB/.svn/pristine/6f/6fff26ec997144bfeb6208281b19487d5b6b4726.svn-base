using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library;

namespace VMS.Api.Models
{

    public class TSVisitorAction
    {
        //=======API
        DataTable dt = new DataTable();
        long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public MessageNonQuery PostNewWIFI(string Host, string Visitor, string UserName, string Password, string CreBy, string expTime = "23:59:59")
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
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryVMS();
        }
        public Visitor_TS GetVisitorFromBadge(string ShimanoBadge)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Id, [Name], [Department], [Email], [Phone]
                            FROM [VisitorTS] 
                            WHERE [ShimanoBadge] ='" + ShimanoBadge + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null,null,false);
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
            string query = @"SELECT TV.Id, TV.[Name], TV.[Department]
                                ,isnull(CONVERT(nvarchar(10),TV.DateOfEnd,112),'') as DateOfEnd
	                            , case when convert(date,TV.DateOfEnd)>convert(date,getdate()) then 1 else 0 end NeedStay 
                                FROM [temp_VisitorTS]  TV
                                LEFT JOIN VisitorTS V on TV.Name = V.Name and TV.Department = V.Department 
	                            WHERE V.Id is null and TV.Name like '%" + Name + @"%' 
								and convert(date,GETDATE()) between convert(date,TV.UploadDate) and convert(date,TV.DateOfEnd) ";

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
                                 DateEnd = rw["DateOfEnd"].ToString(),
                                 NeedStay = rw["NeedStay"].ToBoolean()
                             }).ToList();
            return _visitors;

        }
        public MessageNonQuery PostNewShimanoBadge(ShimanoBadgeModel postData)
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
            return num.NonQueryVMS();
        }
        public MessageNonQuery PostChangeShimanoBadge(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @Id nvarchar(20)='" + postData.Temp_Visitor + @"'
                             DECLARE @ShimanoBadge nvarchar(20)='" + postData.ShimanoBadge + @"'

            UPDATE V set ShimanoBadge=@ShimanoBadge from VisitorTS V where Id=@Id";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryVMS();
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
        public MessageNonQuery PostVisitorCheckIn(ShimanoBadgeModel postData)
        {
            string query = @"DECLARE @ShimanoBadge nvarchar(50) = '" + postData.ShimanoBadge + @"'
                             DECLARE @Temp_Id nvarchar(50) = '" + postData.Temp_Visitor + @"'
                             DECLARE @NeedLunch nvarchar(50) = '" + postData.NeedLunch.ToString() + @"'
                             DECLARE @NeedStay nvarchar(50) = '" + postData.NeedStay.ToString() + @"'
                             DECLARE @StayDate date = '" + postData.StayDate.ToString() + @"'

                            if year(@StayDate)<2000 set @StayDate=null
                            DECLARE @HostName nvarchar(100) =''
                            DECLARE @HostDepartment nvarchar(100) =''
                            DECLARE @Plant nvarchar(100) =''

                            select @HostName=b.HostName,@HostDepartment=b.HostDepartment,@Plant=b.Plant from VisitorTS a 
                            left join Temp_VisitorTS b on a.Name=b.Name
                            where a.ShimanoBadge=@ShimanoBadge


                            if @Temp_Id = '0'
                            begin
	                            UPDATE b set b.UploadDate=getdate(),b.DateOfEnd=@StayDate from VisitorTS a 
	                            join Temp_VisitorTS b on a.Name=b.Name
	                            where a.ShimanoBadge = @ShimanoBadge
                            end

                            INSERT INTO VisitLog_TS
                            SELECT (SELECT ISNULL(MAX(LogId), 0)+1 from VisitLog_TS), (select Id from VisitorTS where ShimanoBadge = @ShimanoBadge),
                            @HostName,@HostDepartment,CONVERT(TIME, GETDATE(),103),null, GETDATE(),@NeedLunch,'CHECKIN', @Plant, null,@NeedStay,@StayDate";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryVMS();
        }
        public MessageNonQuery PostVisitorCheckOut(string LogId)
        {
            string query = @"DECLARE @LogId Nvarchar(10) = '" + LogId + @"'
update VL set [Status]='CHECKOUT', [TimeOut]=CONVERT(TIME, GETDATE(),103) from VisitLog_TS VL where LogId=@LogId";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryVMS();
        }
        public VisitorJoinLog GetorPostVisitorTS(string ShimanoBadge)
        {
            string query = @"DECLARE @ShimanoBadge as nvarchar(20) = '" + ShimanoBadge + @"'

            IF EXISTS(SELECT 1 FROM VisitorTS V INNER JOIN VisitLog_TS VL on V.Id = VL.TSVisitorId where ShimanoBadge=@ShimanoBadge and VL.Status = 'CHECKIN' and CONVERT(nvarchar(10),DateVisit,121) = CONVERT(nvarchar(10),GETDATE(),121))
            BEGIN
	            SELECT V.Id as VisitorId, V.Name,V.Department, LogId as Temp_VisitorId, 
	            Status,isnull(CONVERT(nvarchar(10),VL.DateOfEnd,112),'') as DateOfEnd
	            , case when convert(date,VL.DateOfEnd)>convert(date,getdate()) then 1 else 0 end NeedStay
	            FROM VisitorTS V INNER JOIN VisitLog_TS VL on V.Id = VL.TSVisitorId 
	            where ShimanoBadge=@ShimanoBadge and CONVERT(nvarchar(10),DateVisit,121) = CONVERT(nvarchar(10),GETDATE(),121) AND status='CHECKIN'
            END
            ELSE if exists(SELECT 1  from VisitorTS V
	            LEFT OUTER JOIN temp_VisitorTS TV on V.Name = TV.Name
	            and CONVERT(date,GETDATE()) between CONVERT(date,UploadDate) 
	            and CONVERT(date,DateOfEnd) where ShimanoBadge=@ShimanoBadge)
            BEGIN
	            SELECT V.Id as 'VisitorId', V.Name, V.Department, isnull(TV.Id, '0') as Temp_VisitorId,
	            'PENDING' as Status,isnull(CONVERT(nvarchar(10),TV.DateOfEnd,112),'') as DateOfEnd
	            , case when isnull(TV.Id, '0')='0' then 0 else case when convert(date,TV.DateOfEnd)>convert(date,getdate()) then 1 else 0 end end NeedStay
	            from VisitorTS V
	            LEFT OUTER JOIN temp_VisitorTS TV on V.Name = TV.Name 
	            and CONVERT(date,GETDATE()) between CONVERT(date,UploadDate) 
	            and CONVERT(date,DateOfEnd)
	            where ShimanoBadge=@ShimanoBadge
            END
            ELSE IF EXISTS (select MnuCod from levelmenu where BusFunc in (select BusFunc from Usr where UseID=@ShimanoBadge) and MnuCod='REGWIFI') --Add by bondra
            BEGIN
	            SELECT '999999999' as 'VisitorId', @ShimanoBadge as Name, 'NULL' as Department, '0' as Temp_VisitorId,
	            'WIFI' as Status, null as DateOfEnd,0 NeedStay
            END
            ELSE
            BEGIN
            SELECT '0' as 'VisitorId', 'NONAME' as Name, 'NULL' as Department, '0' as Temp_VisitorId,
	            'PENDING' as Status, null as DateOfEnd,0 NeedStay
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
                            DateOfEnd = rw["DateOfEnd"].ToString(),
                            NeedStay = rw["NeedStay"].ToBoolean()
                        }).FirstOrDefault();
            return data;
        }
        //=======API


        //===================================================

        public List<Temp_VisitorTS> GetTempVisitor(string dateFrom, string dateTo, string name)
        {
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
[Status]
from VisitLog_TS VL
INNER JOIN VIsitorTs V on VL.TSVisitorId = V.Id 
where CONVERT(NVARCHAR(10), DateVisit, 126) between @dateFrom and @dateTo";
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
                        }).ToList();
            return data;
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
        public List<TSLogHistory> GetVisitorTSToday(string Name)
        {
            string query = @"DECLARE @Name nvarchar(50) = '" + Name + @"'

SELECT V.Name,  CONVERT(varchar,isnull(TimeIn ,'00:00'), 108) as TimeIn,
CAST (DATEDIFF(Hour,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103))) as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull([TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull([TIMEOUT], CONVERT(TIME, GETDATE(),103)))%60 as nvarchar) + ' Minute(s)'  as Duration,  HostName + ' (' + HostDepartment + ')' as HostName, [Status]
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
                        }).ToList();
            return data;
        }
        public MessageNonQuery PostTempVisitor(string PathFile, string filename)
        {
            string query = "";

            string sheetName = "Sheet1";
            var Visitors = SysUtil.GetObjectFromExcel<Template_UploadVsitor>(PathFile, sheetName);
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
            return num.NonQueryVMS();
        }
        public MessageNonQuery SaveVisitorTemp(Temp_VisitorTS tempvisitor)
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
            return num.NonQueryVMS();
        }
    }
    public class MessageNonQuery
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
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
        public bool NeedStay { get; set; }


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
        public string DateOfEnd { get; set; }
        public bool NeedStay { get; set; }


    }
    public class ShimanoBadgeModel
    {
        public string ShimanoBadge { get; set; }
        public string Temp_Visitor { get; set; }
        public bool NeedLunch { get; set; }
        public string PhotoName { get; set; }
        public bool NeedStay { get; set; }
        public string StayDate { get; set; }
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
    public class ShimanoWIFI
    {
        public string Host { get; set; }
        public string Visitor { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CreBy { get; set; }
    }
}
