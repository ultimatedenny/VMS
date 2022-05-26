using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMS.Library;
using System.Data;

namespace VMS.Web.Models
{
    public class NewVisitor
    {
        DataTable dt;
        long num;
        //string scalar;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public VMSRes<List<LogVisitor>> GetVisitorOrLogByCard(string CardId)
        {
            List<LogVisitor> data = new List<LogVisitor>();
            string query = "spGetVisitorOrLogByCard";
            List<ctSqlVariable> param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "CardId", Value = CardId, Type = "String" });
            param.Add(new ctSqlVariable { Name = "Plant", Value = Sessions.GetUsePlant(), Type = "String" });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            var result = dt.OneRowdtToResult();
            if (result.Success)
            {
                data = (from rw in dt.AsEnumerable()
                        select new LogVisitor()
                        {
                            VisitorId = rw["VisitorId"].ToString(),
                            LogId = rw["LogId"].ToString(),
                            VisitorName = rw["FullName"].ToString(),
                            VisitorCompany = rw["Company"].ToString(),
                            Status = rw["Status"].ToString(),
                            HostName = rw["HostName"].ToString(),
                            Remark = rw["Remark"].ToString(),
                            TimeIn = rw["TimeIn"].ToString(),
                        }).ToList();
            }
            return new VMSRes<List<LogVisitor>>
            {
                Success = result.Success,
                Message = result.Message,
                data = data
            };
        }
        public VMSRes<Visitor> GetVisitorDetailById(string VisitorId)
        {
            string query = $@"select [VisitorCardNo], fullName, Photo, Company,Phone,JobDesc,Email,VehicleNo from Visitor where Id='{VisitorId}'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var data = (from rw in dt.AsEnumerable()
                        select new Visitor
                        {
                            FullName = rw["fullName"].ToString(),
                            Company = rw["Company"].ToString(),
                            JobDesc = rw["JobDesc"].ToString(),
                            Photo = rw["Photo"].ToString(),
                            Phone = rw["Phone"].ToString(),
                            Email = rw["Email"].ToString(),
                            VehicleNo = rw["VehicleNo"].ToString(),
                            VisitorCardNo = rw["VisitorCardNo"].ToString()
                        }).FirstOrDefault();
            return new VMSRes<Visitor>()
            {
                Success = dt.dtHavingRow(),
                data = data
            };
        }
        public VMSRes<string> PostUpdateVisitorWhileCheckin(Visitor Visitor)
        {
            string query = $@"UPDATE V set Phone='{Visitor.Phone}',
Email='{Visitor.Email}',
VehicleNo='{Visitor.VehicleNo}',                
JobDesc='{Visitor.JobDesc}'                   
from Visitor V where V.Id='{Visitor.id}'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();

        }
        public VMSRes<HostAppointment> GetHostAppointment(string LogId, string VisitorId)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE @LogId varchar(10) = '{LogId}',
 @VisitorId varchar(10) = '{VisitorId}'
select top 1 VL.LogId,UseID, UseNam, UseDep, UseTel, VT.VisitorType,VT.NeedApprove, VL.Remark,
(select distinct(AreaName) + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) as Area, isnull(VLD.STATUS, VL.Status) as Status, VL.TimeStart, VL.TimeEnd
 from VisitLog VL
		INNER JOIN Usr U on VL.HostId = U.UseID 
		INNER JOIN VisitorType VT on VL.VisitType = VT.Id 
		LEFT JOIN VisitLogDet VLD on VL.LogId = VLD.LogId and VL.VisitorId = VLD.VisitorId
		where VL.Visitorid	= @VisitorId and VL.LogId = @LogId order by NumberOfVisit
";
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
                                        //TimeStart = rw["TimeStart"].ToString().Substring(0, 5),
                                        //TimeEnd = rw["TimeEnd"].ToString().Substring(0, 5),
                                        TimeStart = rw["TimeStart"].ToString() == "" ? "" : rw["TimeStart"].ToString().Substring(0, 5),
                                        TimeEnd = rw["TimeEnd"].ToString() == "" ? "" : rw["TimeEnd"].ToString().Substring(0, 5),
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
            //            string query = $@"DECLARE @Plant varchar(10) = '{Sessions.GetUsePlant()}',
            //		@NameorCompany varchar(50) = '{NameorCompany}'
            //SELECT id, Vl.LogId, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No Appointment') as HostName,
            //ISNULL(VL.Status, '0') as Status
            //	, VehicleNo FROM
            //(SELECT * FROM Visitor WHERE (FullName 
            //	                            like '%'+@NameorCompany+'%' 
            //								or Company like '%'+@NameorCompany+'%' 
            //								or Phone like '%'+@NameorCompany+'%')  ) V
            // LEFT JOIN (
            //select VLD.LogID, VLD.VisitorId, VLD.Status, VL.HostId from VisitLogDet VLD 
            //				INNER JOIN VisitLog VL on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId and plant = @Plant
            //				where VLD.Status in ('BREAK','CHECKIN')
            //				UNION
            //select VL.LogID, VL.VisitorId, isnull(VLD.Status,VL.Status) as Status, VL.HostId from VisitLog VL 
            //LEFT OUTER JOIN VisitLogDet VLD on VL.VisitorId = VLD.VisitorId and VL.LogId = VLD.LogId and DATE = (CONVERT(VARCHAR(10),GETDATE(),121))
            //where  CONVERT(varchar(10), GETDATE(),121) 
            //								between CONVERT(varchar(10), VL.DateStart,121) 
            //								and CONVERT(varchar(10),VL.DateEnd,121) and plant = @Plant
            //								) VL on V.id = VL.VisitorId
            //LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
            //order by Status desc";

            string query = $@"DECLARE @Plant varchar(10) = '{Sessions.GetUsePlant()}',
		@NameorCompany varchar(50) = '{NameorCompany}'
SELECT id, Vl.LogId, FullName, Company, Phone, Email, Jobdesc, VisitorCardNo, isActive, isnull('Appointment With ' + U.UseNam, 'No Appointment') as HostName,
ISNULL(VL.Status, '0') as Status
	, VehicleNo INTO #Data FROM
(SELECT * FROM Visitor WHERE (FullName 
	                            like '%'+@NameorCompany+'%' 
								or Company like '%'+@NameorCompany+'%' 
								or Phone like '%'+@NameorCompany+'%')  ) V
 LEFT JOIN (
select VLD.LogID, VLD.VisitorId, VLD.Status, VL.HostId from VisitLogDet VLD 
				INNER JOIN VisitLog VL on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId and plant = @Plant
				where VLD.Status in ('BREAK','CHECKIN')
				UNION
select VL.LogID, VL.VisitorId, isnull(VLD.Status,VL.Status) as Status, VL.HostId from VisitLog VL 
LEFT OUTER JOIN VisitLogDet VLD on VL.VisitorId = VLD.VisitorId and VL.LogId = VLD.LogId and DATE = (CONVERT(VARCHAR(10),GETDATE(),121))
where  CONVERT(varchar(10), GETDATE(),121) 
								between CONVERT(varchar(10), VL.DateStart,121) 
								and CONVERT(varchar(10),VL.DateEnd,121) and plant = @Plant
								) VL on V.id = VL.VisitorId
LEFT OUTER JOIN Usr U on VL.HostId = U.UseID
order by Status desc

SELECT a.*,b.Disclaimer,IsRead=ISNULL(b.IsRead,1) FROM #Data a 
LEFT JOIN DisclaimerHistory b ON a.LogId=b.LogId AND a.FullName=b.VisitorName AND a.COmpany=b.Company
order by Status DESC

DROP TABLE #Data";

            using (MSSQL s = new MSSQL())
            {
                dt = s.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitors = (from rw in dt.AsEnumerable()
                             select new Visitor
                             {
                                 LogId = rw["LogId"].ToString(),
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
                                 VehicleNo = rw["VehicleNo"].ToString(),
                                 DisclaimerStatus = rw["Disclaimer"].ToString(),
                                 IsRead = rw["IsRead"].ToString(),
                             }).ToList();
            return new VMSRes<List<Visitor>>
            {
                Success = dt.dtHavingRow(),
                data = _visitors
            };
        }
        public VMSRes<LogHistory> GetVisitorToCheckout(string VisitorId, string LogId)
        {
            string query = $@"DECLARE 
	@LogID varchar(10) = '{LogId}',
	@VisitorID varchar(10) = '{VisitorId}'

select VLD.LogId, VLD.VisitorId, VLD.TimeIn, VLD.Status, U.UseNam + ' - ' + UseDep as HostName, V.FullName, 
 V.Company,V.VehicleNo, Phone, v.JobDesc, VLD.Remark from VisitLogDet VLD 
INNER JOIN VisitLog VL on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId
INNER JOIN Visitor V on V.id = VL.VisitorId and V.id = VLD.VisitorId
INNER JOIN Usr U on U.UseID = VL.HostId
where VLD.LogId=@LogID and VLD.VisitorID=@VisitorID and VLD.Status in ('CHECKIN', 'BREAK')";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var data = (from rw in dt.AsEnumerable()
                        select new LogHistory
                        {
                            LogId = rw["LogId"].ToString(),
                            VisitorId = rw["VisitorId"].ToString(),
                            TimeStart = rw["TimeIn"].ToString().Substring(0,5),
                            Status = rw["Status"].ToString(),
                            HostName = rw["HostName"].ToString(),
                            FullName = rw["FullName"].ToString(),
                            Company = rw["Company"].ToString(),
                            VehicleNo = rw["VehicleNo"].ToString(),
                            Phone = rw["Phone"].ToString(),
                            JobDesc = rw["JobDesc"].ToString(),
                            Remark = rw["Remark"].ToString()
                        }).FirstOrDefault();
            return new VMSRes<LogHistory>
            {
                data = data
            };
        }
        public VMSRes<string> PostVisitorCheckinOut(string VisitorId, string LogId, string CardId, string Username = "", string Method = "", string Remark = "")
        {
            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            paramss.Add(new ctSqlVariable { Name = "VisitorId", Value = VisitorId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "CardId", Value = CardId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Username", Value = Sessions.GetUseID(), Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Method", Value = Method, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Remark", Value = Remark, Type = "String" });

            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "spVisitorCheckIn", paramss);
            }
            return dt.OneRowdtToResult();
        }
        public VMSRes<VisitType> GetisUnknown(string LogID)
        {
            DataTable dt = new DataTable();
            string query = @"DECLARE
	                            @LogID varchar(50) = '" + LogID + @"'
	
                            SELECT 
	                            Id
	                            ,VisitorType
	                            ,NeedApprove
	                            ,NeedMailAlert
	                            ,AllowUnknown
	                            ,IsView
	                            ,UpdateAt
	                            ,UpdateOn
                            FROM VisitorType 
                            WHERE id = (SELECT TOP 1 VisitType FROM VisitLog WHERE LogId = @LogID)";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitortype = (from rw in dt.AsEnumerable()

                                   select new VisitType
                                   {
                                       Id = Convert.ToInt32(rw["Id"].ToString()),
                                       VisitorType = rw["VisitorType"].ToString(),
                                       NeedApprove = rw["NeedApprove"].ToString(),
                                       NeedMailAlert = rw["NeedMailAlert"].ToString(),
                                       AllowUnknown = rw["AllowUnknown"].ToString(),
                                       IsView = rw["IsView"].ToString(),
                                       UpdateAt = rw["UpdateAt"].ToString(),
                                       UpdateBy = rw["UpdateOn"].ToString(),
                                   }).FirstOrDefault();
            //return _visitortype;
            return new VMSRes<VisitType>()
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = _visitortype
            };
        }
    }

    public class LogVisitor
    {
        public string VisitorId { get; set; }
        public string LogId { get; set; }
        public string VisitorName { get; set; }
        public string VisitorCompany { get; set; }
        public string HostName { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string TimeIn { get; set; }
    }

}