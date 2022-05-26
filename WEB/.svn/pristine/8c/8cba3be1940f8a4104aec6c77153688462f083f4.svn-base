using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class VisitLogAction
    {
        DataTable dt;
        long num;
        string scalar;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public List<UrgentReport> GetFileUrgentReport(string Plant)
        {
            string query = $@"DECLARE @Plant varchar(10) = '{Plant}'
SELECT CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, 
V.FullName as  [Visitor Name], V.Company, V.Phone,
CASE WHEN (CAST(DATEDIFF(DAY,[Date],GETDATE())as varchar) > 0) then
	CAST(DATEDIFF(DAY,[Date],GETDATE())as varchar) + ' Days' else
	 CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  end
 as Duration,

VT.VisitorType, UH.UseNam as [Host Name], UH.UseDep as [Host Dept], UH.UseTel as [Host Phone], (select distinct(AreaName) + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) as Area
FROM Visitlog VL
LEFT OUTER JOIN Visitor V on VL.VisitorId = V.id
LEFT OUTER JOIN Usr UH on VL.HostId = UH.UseID
LEFT OUTER JOIN VisitorType VT on VL.VisitType = VT.Id 
LEFT OUTER JOIN VisitLogDet VLD on VL.LogId = VLD.LogId 
and VL.VisitorId = VLD.VisitorId
where Plant=@Plant and isActive=1 and VLD.status='CHECKIN'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _data = (from rw in dt.AsEnumerable()
                         select new UrgentReport
                         {
                             TimeStart = rw["TimeIn"].ToString(),
                             FullName = rw["Visitor Name"].ToString(),
                             Company = rw["Company"].ToString(),
                             Phone = rw["Phone"].ToString(),
                             Duration = rw["Duration"].ToString(),
                             VisitorType = rw["VisitorType"].ToString(),
                             HostName = rw["Host Name"].ToString(),
                             HostDept = rw["Host Dept"].ToString(),
                             HostTel = rw["Host Phone"].ToString(),
                             Area = rw["Area"].ToString(),
                         }).ToList();
            return _data;
        }
        public DataTable ExportSecurityHis(string DateStart, string DateEnd, string Plant)
        {
            DataTable dt = new DataTable();
            string query = $@"spGetReportVisitorLog";
            var param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "DateStart", Type = "string", Value = DateStart });
            param.Add(new ctSqlVariable { Name = "DateEnd", Type = "string", Value = DateEnd });
            param.Add(new ctSqlVariable { Name = "Plant", Type = "string", Value = Plant });
            param.Add(new ctSqlVariable { Name = "SearchVisitor", Type = "string", Value = "" });
            param.Add(new ctSqlVariable { Name = "SearchHost", Type = "string", Value = "" });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            return dt;
        }
        public List<LogHistory> GetHistoryVisitLog(string UserName, string dateFrom, string dateTo)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT fullName, Company, VisitorType, Plant, t1.Remark, Area, DateStart, DateEnd,TimeStart, TimeEnd FROM VISITLOG t1
	INNER JOIN VISITOR t2 on t1.VisitorId = t2.id
	INNER JOIN VisitorType t3 on t1.VisitType = t3.Id where HostId = '" + UserName + @"' and t1.[status] !='WAITING APPROVAL ' --'PENDING' 
    and dateStart between '" + dateFrom + @"' and '" + dateTo + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Area = row["Area"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              DateEnd = Convert.ToDateTime(row["DateEnd"].ToString()).ToString("yyyy-MM-dd"),
                              TimeStart = row["TimeStart"].ToString(),
                              TimeEnd = row["TimeEnd"].ToString()
                          }).ToList();
            return _datas;

        }
        public List<LogHistory> GetPendingVisitLog(string UseDept, string UseID)
        {
            //bool isAuth = Class.Global.IsAuth(Sessions.GetBusFunc());
            bool isAuth = Class.Global.IsAuthToCancelAnyAppointment(Sessions.GetBusFunc()); //Modified by CANICE 20210930
            DataTable dt = new DataTable();
            string query = $@"

SELECT distinct
	VL.LogId, VL.VisitorId, 
	fullName, Company, UseNam + ' ('+UseDep+')' as UseNam,
	isnull(VLD.Status,VL.[Status]) as Status, Plant,
	VisitorType, DateStart, DateEnd,  VL.Remark, NeedApprove,
	CASE WHEN isnull(VLD.Status,VL.Status)  in ('CHECKIN', 'CANCELLED', 'REJECTED')  then 0 else 1 
	 end as IsAbleCancel into #TabelVisitor
	FROM VISITLOG VL
	INNER JOIN VISITOR V on VL.VisitorId = V.id
	INNER JOIN VisitorType VT on VL.VisitType = VT.Id
    INNER JOIN Usr u on VL.HostId = U.UseID 
	LEFT JOIN VisitLogDet VLD on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId
where "
        + (isAuth ? "" : $"((UseDep = '{UseDept}' AND UsePlant='{Sessions.GetUsePlant()}') OR VL.CreUser = '{UseID}') AND ") + //Modified by CANICE 20210930 + (isAuth ? "" : $"UseDep = '{UseDept}' AND UsePlant='{Sessions.GetUsePlant()}' AND ") +
        @"(CONVERT(nvarchar(10),GETDATE(),121)
		between CONVERT(nvarchar(10),DateStart,121) and CONVERT(nvarchar(10),DateEnd,121)
		or CONVERT(nvarchar(10),GETDATE(),121) < CONVERT(nvarchar(10),DateStart,121))
		;WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY LogId ORDER BY LogId DESC) AS rn
   FROM #TabelVisitor
)

SELECT *
FROM cte
WHERE rn = 1

drop table #TabelVisitor";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = row["LogId"].ToString(),
                              VisitorId = row["VisitorId"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              Status = row["Status"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              NeedApprove = row["NeedApprove"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              DateEnd = Convert.ToDateTime(row["DateEnd"].ToString()).ToString("yyyy-MM-dd"),
                              HostName = row["UseNam"].ToString(),
                              IsAbleCancel = row["IsAbleCancel"].ToBoolean()
                          }).ToList();
            return _datas;
        }
        public List<LogHistory> GetPendingVisitLogDet(string LogId)
        {
            string query = $@"DECLARE @LogId varchar(10) = '{LogId}'

SELECT distinct
	VL.LogId, VL.VisitorId, 
	fullName, Company,
	isnull(VLD.Status,VL.[Status]) as Status, Plant,
	VisitorType, DateStart, DateEnd,  VL.Remark, NeedApprove,
	CASE WHEN isnull(VLD.Status,VL.Status)  in ('CHECKIN', 'CANCELLED', 'REJECTED','CHECKOUT','BREAK')  then 0 else 1 
	 end as IsAbleCancel 
	FROM VISITLOG VL
	INNER JOIN VISITOR V on VL.VisitorId = V.id
	INNER JOIN VisitorType VT on VL.VisitType = VT.Id
	LEFT JOIN VisitLogDet VLD on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId
	where VL.LogId=@LogId";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = row["LogId"].ToString(),
                              VisitorId = row["VisitorId"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              Status = row["Status"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              NeedApprove = row["NeedApprove"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              DateEnd = Convert.ToDateTime(row["DateEnd"].ToString()).ToString("yyyy-MM-dd"),
                              IsAbleCancel = row["IsAbleCancel"].ToBoolean()
                          }).ToList();
            return _datas;


        }
        public List<LogHistory> GetSecurityReportHistory(string DateStart, string DateEnd, string Plant,
            string SearchVisitor, string SearchHost)
        {
            DataTable dt = new DataTable();
            string query = $@"spGetReportVisitorLog";
            var param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "DateStart", Type = "string", Value = DateStart });
            param.Add(new ctSqlVariable { Name = "DateEnd", Type = "string", Value = DateEnd });
            param.Add(new ctSqlVariable { Name = "Plant", Type = "string", Value = Plant });
            param.Add(new ctSqlVariable { Name = "SearchVisitor", Type = "string", Value = SearchVisitor });
            param.Add(new ctSqlVariable { Name = "SearchHost", Type = "string", Value = SearchHost });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = row["LogId"].ToString(),
                              HostName = row["UseNam"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Remark = row["Remark"].ToString(),
                              ReadStatus = row["ReadStatus"].ToString(), //ADD HIDEF 09-03-2022
                              DateStart = row["DateStart"].ToString(),
                              DateEnd = row["DateEnd"].ToString(),
                              TimeStart = row["TimeIn"].ToString(),
                              TimeEnd = row["TimeEnd"].ToString(),
                              Duration = row["Duration"].ToString(),
                              Area = row["Area"].ToString(),
                              Status = row["Status"].ToString(),
                              NeedApprove = row["NeedApprove"].ToString(),
                              RemarkVisitor = row["RemarkIn"].ToString(),
                              ApprovedBy = row["ApprovedBy"].ToString(),
                              VehicleNo = row["VehicleNo"].ToString(),
                              DateIn = row["DateIn"].ToString(),
                              DateOut = row["DateOut"].ToString(),
                              ShimanoBadge = row["ShimanoBadge"].ToString(),
                              UserIn = row["UserIn"].ToString(),
                              UserOut = row["UserOut"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<LogHistory> GetSecurityReportPending(string dateFrom, string dateTo)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT UseNam, fullName, Company, VisitorType, Plant, t1.Remark, Area, DateStart, DateEnd,TimeStart, TimeEnd FROM VISITLOG t1
	INNER JOIN VISITOR t2 on t1.VisitorId = t2.id
	INNER JOIN Usr on HostId= UseID
	INNER JOIN VisitorType t3 on t1.VisitType = t3.Id 
	where 
	dateStart between '" + dateFrom + @"' and '" + dateTo + @"'
	and t1.[status] ='WAITING APPROVAL '--='PENDING'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              HostName = row["UseNam"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Area = row["Area"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              DateEnd = Convert.ToDateTime(row["DateEnd"].ToString()).ToString("yyyy-MM-dd"),
                              TimeStart = row["TimeStart"].ToString(),
                              TimeEnd = row["TimeEnd"].ToString()
                          }).ToList();
            return _datas;

        }
        public LogHistory GetVisitorInArea(string CardId, string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT t1.LogId, t1.VisitorId, t2.fullName, t3.UseNam + ' (' + t3.UseDep+ ') Ext. ' + UseTel as UseNam, t2.Company,t1.[Status],
t4.Remark as RemarkVisitor
 FROM VISITLOG t1 
	                            INNER JOIN VISITOR  t2 on t1.VisitorId = t2.id
								INNER JOIN Usr t3 on t1.HostId = t3.UseId
								Left JOIN VisitLogDet t4 on t1.LogId=t4.LogId and t1.VisitorId=t4.VisitorId and 
								convert(varchar, [date], 23)=convert(varchar, getdate(), 23)
	                            where ShimanoBadge ='" + CardId + @"' 
	                            and GETDATE() between t1.Datestart and DATEADD(DAY, 1, DateEnd)
	                            and t1.[Status] in ('BREAK', 'CHECKIN') and t1.Plant='" + Plant + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
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
            return _datas;


        }
        public List<Chart> ShowVisitorByHost()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT top 5 COUNT(distinct LogId) as counting, UseNam from VisitLog VL
inner join Usr on VL.HostId = Usr.UseId
where datestart between  DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE()), -1)
group by UseNam order by COUNT(distinct LogId) desc";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new Chart
                          {
                              Data = Int16.Parse(rw["counting"].ToString()),
                              Name = rw["UseNam"].ToString()

                          }).ToList();
            return _datas;
        }
        public List<Chart> ShowVisitorOneMonth()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT count(*) as counting, [date] from VisitLogDet 
where [date] between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE()), -1)group by [date]";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new Chart
                          {
                              Data = Int16.Parse(rw["counting"].ToString()),
                              Name = rw["date"].ToString()

                          }).ToList();
            return _datas;
        }
        public List<LogHistory> ShowVisitorToday(string Name, string plant = "2300", string Status = "", string Host = "")
        {
            DataTable dt = new DataTable();
            string query = @"
DECLARE @Name varchar(MAX) = '" + Name + @"'
DECLARE @Host varchar(MAX) = '" + Host + @"'
DECLARE @Plant varchar(10) = '" + plant + @"'
DECLARE @Status varchar(10) = '" + Status + @"'


SELECT VL.VisitorId, CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, 
V.FullName + '<br/> (' + V.Company + ')' as Visitor, V.Phone,
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  as Duration,
VT.VisitorType, UH.UseNam+' ('+UH.UseDep+')' + '<br/> - Ext. ' + UH.UseTel as Host, (select distinct(AreaName) + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) as Area, 
ISNULL(VLD.[Status], VL.[Status]) + '<br/> By ' +  isnull(UA.UseNam, 'System') as [Status], 
VT.NeedApprove as NeedApprove, VisitorCardNo
FROM Visitlog VL
LEFT OUTER JOIN Visitor V on VL.VisitorId = V.id
LEFT OUTER JOIN Usr UH on VL.HostId = UH.UseID
LEFT OUTER JOIN Usr UA on VL.ApprovedBy = UA.UseID
LEFT OUTER JOIN VisitorType VT on VL.VisitType = VT.Id 
LEFT OUTER JOIN VisitLogDet VLD on VL.LogId = VLD.LogId and [date] = CONVERT(varchar(10), GETDATE(),121) 
and VL.VisitorId = VLD.VisitorId
where CONVERT(DATE, GETDATE(),103) 
between [DateStart] and [DateEnd] and (V.FullName like '%' + @Name + '%' or V.Company like '%' + @Name + '%') 
and Plant=''+@Plant+'' and ISNULL(VLD.[Status], VL.[Status]) like '%' + @Status + '%' 
    and UH.UseNam Like '%'+@Host+'%' and isActive=1
order by [Status],Visitor";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogHistory
                          {
                              TimeStart = TimeSpan.Parse(rw["TimeIn"].ToString()).ToString(),
                              FullName = rw["Visitor"].ToString(),
                              VisitorType = rw["VisitorType"].ToString(),
                              HostName = rw["Host"].ToString(),
                              Area = rw["Area"].ToString(),
                              Status = rw["Status"].ToString(),
                              Duration = rw["Duration"].ToString(),
                              Phone = rw["Phone"].ToString(),
                              NeedApprove = rw["NeedApprove"].ToString(),
                              VisitorCardNo = rw["VisitorCardNo"].ToString(),
                              VisitorId = rw["VisitorId"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<Visitor> GetChangeReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            string query = @"
    DECLARE 
@isAllVendor nvarchar(10) = '" + IsAllVendor + @"',
@LogId nvarchar(10) = '" + LogId + @"',
@VisitorId nvarchar(10) = '" + VisitorId + @"'

if (@isAllVendor = '0')
begin
SELECT V.FullName, V.Company, V.JobDesc
	FROM VisitLog VL
	INNER JOIN Visitor V on VL.VisitorId = V.Id
	Where LogId=@LogId and VisitorId=@VisitorId
END
ELSE
BEGIN
SELECT V.FullName, V.Company, V.JobDesc
	FROM VisitLog VL
	INNER JOIN Visitor V on VL.VisitorId = V.Id
	Where LogId=@LogId
END
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _data = (from rw in dt.AsEnumerable()
                         select new Visitor
                         {
                             FullName = rw["FullName"].ToString(),
                             Company = rw["Company"].ToString(),
                             JobDesc = rw["JobDesc"].ToString(),
                         }).ToList();
            return _data;

        }
        public VMSRes<string> PostChangeReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            paramss.Add(new ctSqlVariable { Name = "VisitorId", Value = VisitorId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "IsAllVendor", Value = IsAllVendor, Type = "String" });
            using (var sql = new MSSQL())
            {
                num = sql.ExecuteStoProcNonQuery(ConnectionDB, "ChangeStatusReq", paramss);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostCancelReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            string CancelUsr = Sessions.GetUseID();
            string query = @"
DECLARE @LogId nvarchar(20) = '" + LogId + @"',
		@IsAllVendor nvarchar(5) = '" + IsAllVendor + @"',
		@VisitorId nvarchar(20) = '" + VisitorId + @"'

UPDATE VL set [Status] = 'CANCELLED', ChgUser = '"+ CancelUsr + @"', ChgDate = GETDATE()
	FROM VisitLog VL 
	WHERE LogId=@LogId and VisitorId like CASE WHEN @IsAllVendor = 1 then '%%' else @VisitorId END ";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<BarChart> GetSummaryVisitorPerMonth(string Plant, string Month)
        {
            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            paramss.Add(new ctSqlVariable { Name = "Plant", Value = Plant, Type = "String" });
            paramss.Add(new ctSqlVariable { Name = "Month", Value = Month, Type = "String" });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "GetSummaryVisitorPerMonth", paramss);
            }
            return (from rw in dt.AsEnumerable()
                    select new BarChart
                    {
                        ParentColumn = rw["ParentColumn"].ToString(),
                        SubColumn = rw["SubColumn"].ToString(),
                        Summary = double.Parse(rw["Summary"].ToString()),
                    }).ToList();
        }
        public List<LogHistory> GetSummaryVisitorPerDay(string Plant, string Month, string Day)
        {
            DataTable dt = new DataTable();
            string query = @"
DECLARE @Plant nvarchar(20) = '" + Plant + @"'
DECLARE @month nvarchar(20) = '" + Month + @"'
DECLARE @Day nvarchar(20) = '" + Day + @"'

select isnull(UPPER(FullName), 'has been deleted') + ' ('  +  isnull(UPPER(Company),'has been deleted') +')' as Visitor,
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  as Duration

, VT.VisitorType, U.UseNam+' ('+U.UseDep+')' + ' - Ext. ' + U.UseTel as Host, (select distinct(AreaName) + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) as Area, NeedApprove, U1.UseNam 
	from VisitLog VL 
	INNER JOIN VisitLogDet VLD on 
		VL.LogId=VLD.LogId
		and VL.VisitorId = VLD.VisitorId
	INNER JOIN VisitorType VT on
		VL.VisitType = VT.Id 
	LEFt JOIN Usr U on
		U.UseID = VL.HostId 
	LEFt JOIN Usr U1 on
		U1.UseID = VL.CreUser
	LEFT JOIN Visitor V on
		VLD.VisitorId = V.id where plant=@Plant 
		and UPPER(left(datename(mm,cast(RTRIM(cast(format([Date],'yyyyMM') as int))+'01' as date)),3))
		+substring(RTRIM(cast(format([Date],'yyyyMM') as int)),3,2) = @month and DAY([Date])=@Day 
		order by Visitor";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogHistory
                          {
                              FullName = rw["Visitor"].ToString(),
                              VisitorType = rw["VisitorType"].ToString(),
                              HostName = rw["Host"].ToString(),
                              Area = rw["Area"].ToString(),
                              Duration = rw["Duration"].ToString(),
                              NeedApprove = rw["NeedApprove"].ToString(),
                              CreUser = rw["UseNam"].ToString(),

                          }).ToList();
            return _datas;
        }
        public List<BarChart> GetVisitorSummaryPerMonthDept(string Plant)
        {
            string query = @"DECLARE @Plant nvarchar(20) = '" + Plant + @"'
DECLARE @DateStart date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
DECLARE @dateEnd date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, -1))
select top 5 U.UseDep as ParentColumn, count(*) as [Summary] from VisitLog VL 
	INNER JOIN VisitLogDet VLD on 
		VL.LogId=VLD.LogId
		and VL.VisitorId = VLD.VisitorId
	INNER JOIN VisitorType VT on
		VL.VisitType = VT.Id 
	INNER JOIN Usr U on VL.HostId = u.UseID
	where plant=@Plant and CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
	GROUP BY U.UseDep order by summary desc";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return (from rw in dt.AsEnumerable()
                    select new BarChart
                    {
                        ParentColumn = rw["ParentColumn"].ToString(),
                        Summary = double.Parse(rw["Summary"].ToString()),
                    }).ToList();
        }
        public List<LogHistory> GetHistoryVisitorById(string VisitorId)
        {
            DataTable dt = new DataTable();
            string query = @"select convert(nvarchar(10),VLD.[Date], 121) as [Date], 
		UH.UseNam  + ' (' + UH.UseDep + ')<br/>Ext.' + UH.UseTel as HostId,
		VL.Area,
		VL.Remark,
		isnull(UA.UseNam, 'By System') as ApprovedBy,
VLD.TimeIn, isnull(VLD.[TimeOut],  '00:00:00') as [TimeOut], VLD.Remark  as 'Goods'
	from VisitLogDet VLD 
	INNER JOIN VisitLog VL on VLD.VisitorId = VL.VisitorId and VLD.LogId = VL.LogId
	INNER JOIN Usr UH on VL.HostId = UH.UseID
	LEFT JOIN Usr UA on VL.ApprovedBy = UA.UseID
	where VLD.VisitorId='" + VisitorId + @"' order by [Date] desc";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogHistory
                          {
                              DateStart = rw["Date"].ToString(),
                              HostId = rw["HostId"].ToString(),
                              Area = rw["Area"].ToString(),
                              ApprovedBy = rw["ApprovedBy"].ToString(),
                              TimeStart = rw["TimeIn"].ToString(),
                              TimeEnd = rw["TimeOut"].ToString(),
                              Remark = rw["Remark"].ToString(),
                              RemarkVisitor = rw["Goods"].ToString(),

                          }).ToList();
            return _datas;
        }
        public List<BarChart> GetSummaryVisitorPerYear(string Plant)
        {
            List<ctSqlVariable> paramss = new List<ctSqlVariable>();
            paramss.Add(new ctSqlVariable { Name = "Plant", Value = Plant, Type = "String" });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "GetSummaryVisitorPerYear", paramss);
            }
            return (from rw in dt.AsEnumerable()
                    select new BarChart
                    {
                        ParentColumn = rw["ParentColumn"].ToString(),
                        SubColumn = rw["SubColumn"].ToString(),
                        Summary = double.Parse(rw["Summary"].ToString()),
                    }).ToList();
        }
        public VMSRes<string> PostChangeStatusAll(string LogId, string VisitorId, string VisitorType)
        {
            string Querystatus = $"SELECT NeedApprove from VisitorType where [Id] = '{VisitorType}'";
            string VitId = (VisitorId == "" || VisitorId.ToUpper() == "ALL") ? "" : $"AND VisitorId = '{VisitorId}'";
            string query = "";
            using (var sql = new MSSQL())
            {
                scalar = sql.ExecuteScalar(ConnectionDB, Querystatus, null, null, false);
                //var Status = scalar.ToBoolean() ? "PENDING" : "APPROVED";
                var Status = scalar.ToBoolean() ? "WAITING APPROVAL" : "APPROVED"; //Modify by suharjo ng 2021-11-09
                query = $@"update VL set VisitType='{VisitorType}', status='{Status}' from VisitLog VL where LogId='{LogId}' {VitId} and Status not in ('CANCELLED', 'REJECTED')
                            IF EXISTS(SELECT * FROM VisitLog where LogId='{LogId}' and status='WAITING APPROVAL ')--status='PENDING')
	                            begin
	                            exec [PopulateEmailApproval] '{LogId}', '{Sessions.GetUsePlant()}'
	                            end
	                            ELSE
	                            BEGIN
	                            exec [Email_exeProc] '{LogId}'
	                            END";
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> PostCancelAll(string LogId, string VisitorId)
        {
            string CancelUsr = Sessions.GetUseID();
            //Modified by CANICE 20210801
            string VitId = (VisitorId == "" || VisitorId.ToUpper() == "ALL") ? "" : $"AND VisitorId = '{VisitorId}'";
            //string query = $@"update VL set  status='CANCELLED', ChgUser='{CancelUsr}', ChgDate = GETDATE() from VisitLog VL where LogId='{LogId}' {VitId} and Status not in ('CANCELLED', 'REJECTED');";
            string checkquery = $@"select status from VisitLog VL where LogId='{LogId}' {VitId} and Status in ('PENDING', 'APPROVED', 'WAITING APPROVAL ');";//add Waiting Approval by suharjo ng 2021-11-08
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, checkquery, null, null, false);
            }
            if (!dt.dtHavingRow())
            {
                return new VMSRes<string>
                {
                    Success = false,
                    Message = "No appointment to be cancelled. Kindly check the appointment status again"
                };
            }
            string Visitor = (VisitorId == "" || VisitorId.ToUpper() == "ALL") ? "0" : $"{VisitorId}";

            List<ctSqlVariable> param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "LogID", Type = "String", Value = LogId });
            param.Add(new ctSqlVariable { Name = "CancelUser", Type = "String", Value = CancelUsr });
            param.Add(new ctSqlVariable { Name = "VisitorID", Type = "String", Value = Visitor });
            //string query = $@"DECLARE @RC int = 0; EXEC @RC = [spCancelAppointment] @LogID = {LogId}, @CancelUser = '{CancelUsr}', @VisitorID = {Visitor}; SELECT @RC";
            //End Modified 20210801
            //string num2 = "";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "[spCancelAppointment]", param);
            }
            return new VMSRes<string>
            {
                Success = true,
            };
        }
        public VMSRes<List<LogHistory>> GetHistoryVisitorByIdStoreProc(string VisitorId)
        {
            List<ctSqlVariable> param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name= "VisitorId", Type="String", Value=VisitorId });
            using(var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, "[spGetHistoryVisitorById]", param);
            }
            var data = (from rw in dt.AsEnumerable()
                        select new LogHistory
                        {
                            HostId = rw["HostId"].ToString(),
                            DateIn = rw["DateIn"].ToString(),
                            TimeStart = rw["TimeIn"].ToString(),
                            TimeEnd = rw["TimeOut"].ToString(),
                            UserIn = rw["UserIn"].ToString(),
                            UserOut = rw["UserOut"].ToString(),
                            Remark = rw["RemarkIn"].ToString(),
                            RemarkOut = rw["RemarkOut"].ToString(),
                            VisitorType = rw["VisitorType"].ToString(),
                        }).ToList();
            return new VMSRes<List<LogHistory>>
            {
                Success = dt.dtHavingRow(),
                Message = dt.dtHavingRowMsg(),
                data = data
            };
            
        }

        //ADD HIDEF 09-03-2022
        public List<DisclaimerArea> GetDisclaimerDetail(string LogId, string FullName, string Company, string VisitorType, string Plant)
        {
            List<DisclaimerArea> datas = new List<DisclaimerArea>();
            try
            {
                string query = @"SELECT * FROM DisclaimerActivity WHERE LogId='" + LogId + @"' AND Plant='" + Plant + @"' AND VisitorName='" + FullName + @"' AND Company='" + Company + @"' AND VisitorType='" + VisitorType + @"'";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new DisclaimerArea()
                    {
                        DisclaimerCategory = dr["DisclaimerCategory"].ToString(),
                        AreaName = dr["Area"].ToString(),
                        UpdateAt = dr["ReadDate"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }

    }
    public class DashboardAction
    {
        DataTable dt;
        //long num;
        //string scalar;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public List<LogHistory> GetListVisitorToday(string Name="", string Plant = "2300", string Status = "", string Host = "")
        {
            var param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "Name", Type = "String", Value = Name });
            param.Add(new ctSqlVariable { Name = "Plant", Type = "String", Value = Plant });
            param.Add(new ctSqlVariable { Name = "Host", Type = "String", Value = Host });
            param.Add(new ctSqlVariable { Name = "Status", Type = "String", Value = Status });  

            DataTable dt = new DataTable();
            string query = "[spGetDashboardVisitor]";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            bool isAuth = Class.Global.IsAuth(Sessions.GetBusFunc());
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = rw["LogId"].ToString(), //Added by CANICE 20210801
                              TimeStart = TimeSpan.Parse(rw["TimeIn"].ToString()).ToString(),
                              DateStart = rw["Date"].ToString().DateStd(),
                              FullName = rw["FullName"].ToString(),
                              Company = rw["Company"].ToString(),
                              VisitorType = rw["VisitorType"].ToString(),
                              HostName = rw["Host"].ToString(),
                              HostTel = rw["HostTel"].ToString(),
                              Area = rw["Area"].ToString(),
                              Status = rw["Status"].ToString(),
                              ApprovedBy = rw["ApprovedBy"].ToString(),
                              Duration = rw["Duration"].ToString(),
                              Phone = isAuth ? rw["Phone"].ToString() : "---",
                              NeedApprove = rw["NeedApprove"].ToString(),
                              VisitorCardNo = rw["VisitorCardNo"].ToString(),
                              VisitorId = rw["VisitorId"].ToString(),
                              VehicleNo = isAuth ? rw["VehicleNo"].ToString() : "---",
                              ShimanoBadge = isAuth ? rw["ShimanoBadge"].ToString() : "---",
                          }).ToList();
            return _datas;
        }
        public dynamic GetVisitorPieChartMonth()
        {
            string query = @"
DECLARE @DateStart date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
DECLARE @dateEnd date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, -1))

select 
plantName as Name, 
count(*) as y,Plant as drilldown, null as parent
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Plant pl on vl.Plant = pl.plantId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by PlantName,Plant
union
select 
UseDep as Name, 
count(*) as y,UseDep+plantName as drilldown, plantName as Parent
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Plant pl on vl.Plant = pl.plantId
INNER JOIN Usr U on U.UseID = vl.HostId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by UseDep,plantName 
UNION
select 
UseNam as Name, 
count(*) as y,null as drilldown, UseDep+plantName as Parent
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Plant pl on vl.Plant = pl.plantId
INNER JOIN Usr U on U.UseID = vl.HostId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by plantName,UseDep,UseNam order by drilldown desc, Parent
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var data = (from rw in dt.AsEnumerable()
                        select new
                        {
                            Name = rw["Name"].ToString(),
                            y = int.Parse(rw["y"].ToString()),
                            drilldown = rw["drilldown"].ToString(),
                            parent = rw["parent"].ToString(),
                        }).ToList();
            var dataParent = (from dt in data
                              where dt.parent == ""
                              select new
                              {
                                  Name = dt.Name,
                                  y = dt.y,
                                  drilldown = dt.Name
                              });
            var ParentDrill = data.Where(d => d.parent != "").Select(d => d.parent).Distinct().ToList();
            List<dynamic> child = new List<dynamic>();
            foreach (var par in ParentDrill)
            {
                child.Add(new
                {

                    Name = par,
                    id = par,
                    data = (from dt in data
                            where dt.parent == par
                            select new
                            {
                                Name = dt.Name,
                                y = dt.y,
                                drilldown = dt.drilldown == "" ? null: dt.drilldown
                            })

                });
            }
            return new 
            {
                parent = dataParent,
                child = child
            };
        }
        public dynamic GetDashboardRank()
        {
            DataSet ds = new DataSet();
            #region query
            string query = @"
DECLARE @DateStart date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
DECLARE @dateEnd date= (SELECT DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, -1))

--Department
select top 5
UseDep +' - '+ plantName as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Plant pl on vl.Plant = pl.plantId
INNER JOIN Usr U on U.UseID = vl.HostId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by UseDep,plantName order by y desc
--User
select top 5
UseNam + ' - ' + UseDep as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Plant pl on vl.Plant = pl.plantId
INNER JOIN Usr U on U.UseID = vl.HostId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by UseNam,UseDep order by y desc
--Visitor
select top 5
V.fullName + ' - ' + Company as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Visitor V on V.id = vl.VisitorId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by fullName,Company order by y desc
--Date
select top 5
VLD.Date as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Visitor V on V.id = vl.VisitorId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by VLD.Date order by y desc
--Company
select top 5
Company as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN Visitor V on V.id = vl.VisitorId
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by Company order by y desc
--Visitor Type
select top 5
VT.VisitorType as Name, 
count(*) as y
FROM VisitLogDet VLD
INNER JOIN VisitLog VL on VLD.LogId = vl.LogId and vld.VisitorId = vl.VisitorId 
INNER JOIN VisitorType VT on VT.Id = vl.VisitType
where CONVERT(nvarchar(10), [DATE],121)
	between CONVERT(nvarchar(10), @DateStart,121) and CONVERT(nvarchar(10),@dateEnd,121)
group by VT.VisitorType order by y desc";
            #endregion
            using (var sql = new MSSQL())
            {
                ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
            }
            #region Populate
            var RankDepartment = (from rw in ds.Tables[0].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            var RankUser = (from rw in ds.Tables[1].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            var RankVisitor = (from rw in ds.Tables[2].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            var RankDate = (from rw in ds.Tables[3].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString().DateStd(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            var RankCompany = (from rw in ds.Tables[4].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            var RankVisitorType = (from rw in ds.Tables[5].AsEnumerable()
                                  select new
                                  {
                                      name = rw["Name"].ToString(),
                                      y = int.Parse(rw["y"].ToString())
                                  });
            #endregion
            return new
            {
                RankDepartment = RankDepartment,
                RankUser = RankUser,
                RankVisitor = RankVisitor,
                RankDate = RankDate,
                RankCompany = RankCompany,
                RankVisitorType = RankVisitorType,
            };
        }
    }

    public class VisitorLog
    {
    }
    public class VisitLog
    {
        public int LogID { get; set; }
        public string VisitorId { get; set; }
        public string HostId { get; set; }
        public string HostDept { get; set; }
        public string passCardNo { get; set; }
        public string VisitType { get; set; }
        public string Plant { get; set; }
        public string Area { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime CreDate { get; set; }
        public string CreUser { get; set; }
        public DateTime ChgDate { get; set; }
        public string ChgUser { get; set; }
        public string CCMail { get; set; }
    }
    public class VisitLogDet
    {
        public int LogId { get; set; }
        public int LogIdDet { get; set; }
        public int Days { get; set; }
        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }
        public string UserIn { get; set; }
        public string UserOut { get; set; }
        public string Status { get; set; }

    }
    public class BarChart
    {
        public string ParentColumn { get; set; }
        public string SubColumn { get; set; }
        public string SubColumn_1 { get; set; }
        public double Count { get; set; }
        public double Summary { get; set; }
        public double Summary_1 { get; set; }
        public double Summary_2 { get; set; }
    }
    public class StockBarChart
    {
        public string name { get; set; }
        public List<double> data { get; set; }
        public string Color { get; set; }
    }
    public class PieChart
    {
        public string name { get; set; }
        public double y { get; set; }
        public string drilldown { get; set; }
    }
    public class RegisterWifi
    {
        public int Id { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string VisitorId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan TimeExpired { get; set; }
        public string sTimeExpired { get; set; }
        public string CreDate { get; set; }
        public string CreUser { get; set; }
        public string CreName { get; set; }
    }
    public class LogHistory
    {
        public string VisitorId { get; set; }
        public string LogId { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string HostDept { get; set; }
        public string HostTel { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public string VisitorType { get; set; }
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
        public string RemarkVisitor { get; set; }
        public string ApprovedBy { get; set; }
        public string VisitorCardNo { get; set; }
        public string CreUser { get; set; }
        public string ShimanoBadge { get; set; }
        public string VehicleNo { get; set; }
        public string JobDesc { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string RemarkOut { get; set; }
        public bool IsAbleCancel { get; set; }
        public string UserIn { get; set; }
        public string UserOut { get; set; }
        public double Number { get; set; } //Added by Suharjo Ng 2021-11-08
        public string ReadStatus { get; set; } //Added by Hidef 2022-03-09
    }
    public class ApprovalStatus
    {
        public string ApprovalLevel { get; set; }
        public string Approval { get; set; }
        public string DelegateTo { get; set; }
        public string ApprovedBy { get; set; }
        public string Department { get; set; }
        public string VisitorType { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
    }
    public class UrgentReport
    {
        public string TimeStart { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string VisitorType { get; set; }
        public string HostName { get; set; }
        public string HostDept { get; set; }
        public string HostTel { get; set; }
        public string Area { get; set; }
        public string Duration { get; set; }
    }
}