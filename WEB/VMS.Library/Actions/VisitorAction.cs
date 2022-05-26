using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Models;
using VMS.Library.Constants;
using System.Globalization;

namespace VMS.Library.Actions
{
    public class VisitorAction 
    {
        Database SQLCon = new Database();
        public MessageNonQuery deleteVisitorAppointment(int Id, string UserName)
        {
            string query = "DELETE FROM temp_visitor where VisitorId='" + Id + "' and HostId='" + UserName + "'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public List<Visitor> getAllVisitor(string NameorCompany)
        {
            DataTable dt = new DataTable();
            string sql = $@"DECLARE @Plant varchar(4) = '{Sessions.GetUsePlant()}'
SELECT id, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No AppointMent') as HostID,ISNULL(VL.Status, '0') as Status
	FROM Visitor V
	LEFT OUTER JOIN VisitLog VL on V.id = VL.VisitorId and CONVERT(varchar(10), GETDATE(),121) 
								between CONVERT(varchar(10), vl.DateStart,121) 
								and CONVERT(varchar(10),vl.DateEnd,121)
								and Plant=@Plant
	LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
	                            WHERE (FullName 
	                            like '%{NameorCompany}%' or Company like '%{NameorCompany}%') 
								and isActive=1
								order by fullname, Company ";

            dt = SQLCon.getDatatableFromSQL(sql, ConnectionDB.VMSConnection);
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
                                 isActive = rw["isActive"].ToString()
                             }).ToList();
            return _visitors;
        }
        public HostAppointment GetHostAppointment(string CardId, string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT t2.LogId, t1.UseID, t1.UseNam, t1.UseDep, t1.UseTel, Area, t4.VisitorType, t2.[Status], Remark, TimeStart,TimeEnd, NeedApprove
                FROM Usr t1 
                INNER JOIN VisitLog t2 on t1.USeID=t2.HostId
                INNER JOIN Visitor t3 on t2.VisitorId = t3.Id
                INNER JOIN VisitorType t4 on t2.VisitType = t4.Id
                WHERE t3.visitorCardNo ='" + CardId + @"' and t2.[Status] in ('PENDING', 'REJECT', 'APPROVED', 'CHECKOUT') and GETDATE() between Datestart and DATEADD(DAY, 1, DateEnd) and t3.isActive=1 and t2.Plant='"+ Plant + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
                                        TimeStart = rw["TimeStart"].ToString(),
                                        TimeEnd = rw["TimeEnd"].ToString(),
                                        NeedApprove = rw["NeedApprove"].ToString(),
                                    }).FirstOrDefault();
            return _hostAppointment;

        }
        public List<Visitor> getListVisitorAfterInvite(string UserName)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT VisitorId, FullName, Company, JobDesc 
	            from temp_visitor t1 INNER JOIN 
                Visitor t2 on t1.VisitorId = t2.Id where hostId = '" + UserName + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
        public List<Visitor> getListVisitorBeforeInvite(string NameorCompany, string Plant, string dateFrom, string dateTo)
        {
            DataTable dt = new DataTable();
            string query = @"EXEC [GetVisitorListAppointment] 
                        @NameorCompany='" + NameorCompany + @"',
                        @Plant='"+Plant+ @"',
                        @dateFrom='" + dateFrom + @"',
                        @dateTo= '" + dateTo + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
        public Visitor getVisitorByPassCard(string PassCardNo)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, FullName, Company, Phone, Email, JobDesc, Photo
                            FROM Visitor 
                            WHERE VisitorCardNo ='" + PassCardNo + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                                 Photo = rw["Photo"].ToString()
                             }).FirstOrDefault();
            return _visitors;
        }
        public MessageNonQuery postAddVisitorAppointment(int Id, string UserName)
        {
            string query = "INSERT INTO temp_visitor values('" + Id + "', '" + UserName + "')";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery postNewVisitor(Visitor visitor)
        {
            string query = @"IF EXISTS(SELECT * FROM VISITOR where Id='"+visitor.id+@"')
BEGIN 
	UPDATE Visitor Set fullName='" + visitor.FullName.ToUpper() + @"',
 Company='" + visitor.Company.ToUpper() + @"',
 JobDesc='" + visitor.JobDesc.ToUpper() + @"',
 Phone='" + visitor.Phone + @"',
 email='" + visitor.Email + @"',
 UpdateBy='" + visitor.UpdateBy + @"',
 UpdateAt='" + DateTime.Now.ToString("yyyy-MM-dd") + @"' where id='" + visitor.id + @"'
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
           ,[UpdateAt], isActive) values(@MaxId, '" + visitor.FullName.ToUpper() + @"',
            '" + visitor.Company.ToUpper() + @"',
            '" + visitor.JobDesc.ToUpper() + @"',
            '" + visitor.Phone + @"',
            '" + visitor.Email + @"',
            '" + visitor.UpdateBy + @"',
            '" + DateTime.Now.ToString("yyyy-MM-dd") + @"', 1)
END";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery postUpdateCardRegister(int Id, string CardId)
        {
            string query = "UPDATE visitor set VisitorCardNo ='" + CardId + "' where id='" + Id.ToString() + "'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery putPhotoVisitor(int idVisitor, string pathPhoto)
        {
            string query = "UPDATE visitor set Photo ='" + pathPhoto + "' where id='" + idVisitor.ToString() + "'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public Visitor GetVisitorDetail(string Id)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, FullName, Company, Phone, Email, JobDesc, Photo
                            FROM Visitor 
                            WHERE id ='" + Id + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 id = Convert.ToInt16(rw["id"].ToString()),
                                 FullName = rw["FullName"].ToString(),
                                 Company = rw["Company"].ToString(),
                                 Email = rw["Email"].ToString(),
                                 Phone = rw["Phone"].ToString(),
                                 JobDesc = rw["JobDesc"].ToString(),
                             }).FirstOrDefault();
            return _visitors;
        }
        public List<LogHistory> GetVisitorLists()
        {
            string query = @"select VL.LogId, VL.VisitorId, fullName + ' (' + Company +')' as FullName, (select AreaName + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) as Area, 
UseNam+' ('+UseDep+')' as Host, 
CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, VL.[Status]
from VisitLog VL 
INNER JOIN Visitor V on VL.VisitorId = V.id
LEFT OUTER JOIN Usr on VL.HostId = Usr.UseID
LEFT OUTER JOIN VisitLogDet VLD on VL.LogId = VLD.LogId WHERE VL.[Status] in ('CHECKIN','BREAK') and V.isActive = 1";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var datas = (from rw in dt.AsEnumerable()
                         select new LogHistory
                         {
                             LogId = rw["LogId"].ToString(),
                             VisitorId = rw["VisitorId"].ToString(),
                             FullName = rw["FullName"].ToString(),
                             Area = rw["Area"].ToString(),
                             HostName = rw["Host"].ToString(),
                             TimeStart = rw["TimeIn"].ToString(),
                             Status = rw["Status"].ToString()
                         }).ToList();
            return datas;
        }
        public MessageNonQuery CheckVisitorName(string Name, string Company, string JobDesc)
        {
            string query = @"select top 1 FullName, Company from Visitor where Fullname='" + Name + "' and Company = '"+ Company + @"' and JobDesc = '"+JobDesc+"' ";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            bool isAvailable = dt.Rows.Count > 0 ? false : true;
            return new MessageNonQuery
            {
                isSuccess = isAvailable,
                message = isAvailable 
                ? dt.Rows[0][0].ToString() + " from " + dt.Rows[0][1].ToString() 
                : "Success"
            };
        }
        public List<RegisterWifi> GetListRegister(string Name, string DateFrom, string DateTo)
        {
            string query = @"
    DECLARE @Name nvarchar(max) = '"+Name+ @"'
SELECT WA.Id, UH.UseNam as HostName, V.FullName, UC.UseNam as CreName, WA.Username, WA.Password,Convert(nvarchar(10),WA.CreDate,121) as CreDate, TimeExpired 
FROM WifiAccount WA
	LEFT JOIN Usr UH on WA.HostId = UH.UseID
	LEFT JOIN Visitor V on WA.VisitorId = V.Id
	LEFT JOIN Usr UC on WA.CreUser = UC.UseID 
    where (UH.UseNam like '%'+ @Name +'%'
        OR Fullname like '%'+ @Name +'%')
        and CONVERT(nvarchar(10), WA.CreDate, 121) between
        CONVERT(nvarchar(10), '"+DateFrom+ @"' ,121) and CONVERT(nvarchar(10), '" + DateTo + @"' ,121)  and V.isActive=1
";
            return (from rw in
               SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
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
        public MessageNonQuery PostRegisterWifi(RegisterWifi RegisterWifi)
        {
            string query = @"

    DECLARE @WifiId nvarchar(5)= ''
SET @WifiId = (SELECT isnull(MAX(Id),0)+1 from WifiAccount)
INSERT INTO WifiAccount (Id, HostId, VisitorId, Username, [Password], TimeExpired, CreDate, CreUser)
        SELECT @WifiId, 
        '" + RegisterWifi.HostId+ @"', 
        '" + RegisterWifi.VisitorId + @"', 
        '" + RegisterWifi.Username + @"', 
        '" + RegisterWifi.Password + @"', 
        '" + RegisterWifi.TimeExpired + @"', 
        CONVERT(nvarchar(16), GETDATE(),121),
        '" + RegisterWifi.CreUser + @"'
EXEC dbo.RegisterWifiEmail @WifiId";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public List<Visitor> GetVisitorForWifi(string HostId)
        {
            string query = @"select DISTINCT V.Id, V.FullName from VisitLog VL 
INNER JOIN 
Visitor V on VL.VisitorId = V.id
where status='CHECKIN' and CONVERT(nvarchar(10),GETDATE(),121) between 
CONVERT(nvarchar(10),DateStart,121) and CONVERT(nvarchar(10),DateEnd,121) and HostId='"+ HostId + "'";
            return (from rw in
               SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
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
            return (from rw in
               SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
                    select new User
                    {
                        UseID = rw["UseID"].ToString(),
                        UseNam = rw["UseNam"].ToString(),
                    }).ToList();
        }
    }
}
