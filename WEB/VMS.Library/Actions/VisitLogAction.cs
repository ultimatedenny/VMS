using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Constants;
using VMS.Library.Models;

namespace VMS.Library.Actions
{
    public class VisitLogAction 
    {
        Database SQLCon = new Database();

        public DataTable ExportSecurityHis(string dateFrom, string dateTo, string plant)
        {
            string query = @"SELECT 
Usr.UseNam+' ('+Usr.UseDep+')' as UseNam, 
V.FullName, 
V.Company,
VT.VisitorType, 
VL.Plant,
VL.Remark,
isnull(VLD.[Date],VL.DateStart) as [DateStart],
CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, 
CONVERT(varchar,isnull(VLD.[TimeOut] ,'00:00'), 108) as TimeEnd, 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  as Duration,
(select AreaName + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
	INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) 
	as Area, 
isnull(VLD.[Status], VL.[Status]) as [Status], 
VT.NeedApprove as NeedApprove,
isnull(VLD.Remark, '') as RemarkVisitor,
CASE WHEN VT.NeedApprove = '0' THEN 'By System'
ELSE isnull(A.UseNam, 'PENDING APPROVED') end as ApprovedBy
FROM Visitlog VL
LEFT OUTER JOIN Visitor V on VL.VisitorId = V.id
LEFT OUTER JOIN Usr on VL.HostId = Usr.UseID
LEFT OUTER JOIN Usr A on VL.ApprovedBy = A.UseID
LEFT OUTER JOIN VisitorType VT on VL.VisitType = VT.Id 
LEFT OUTER JOIN VisitLogDet VLD on VL.LogId = VLD.LogId and VL.VisitorId = VLD.VisitorId 
where CONVERT(nvarchar(10),isnull(VLD.[Date],VL.DateStart), 121) between '" + dateFrom + "' and '" + dateTo + @"' and plant like '%" + plant + @"%'
order by isnull(VLD.[Date],VL.DateStart) desc";
            return SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
        }

        public DataTable ExportSecurityPreHis(string dateFrom, string dateTo)
        {
            throw new NotImplementedException();
        }

        public List<LogHistory> getHistoryVisitLog(string UserName,string dateFrom, string dateTo)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT fullName, Company, VisitorType, Plant, t1.Remark, Area, DateStart, DateEnd,TimeStart, TimeEnd FROM VISITLOG t1
	INNER JOIN VISITOR t2 on t1.VisitorId = t2.id
	INNER JOIN VisitorType t3 on t1.VisitType = t3.Id where HostId = '" + UserName + @"' and t1.[status] !='PENDING' and dateStart between '" + dateFrom + @"' and '" + dateTo + @"'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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

        public List<LogHistory> getPendingVisitLog(string UserName)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 
	LogId, VisitorId,
	fullName, Company, 
	VL.[Status], Area, Plant,
	VisitorType, DateStart, DateEnd,  VL.Remark, NeedApprove
	FROM VISITLOG VL
	INNER JOIN VISITOR V on VL.VisitorId = V.id
	INNER JOIN VisitorType VT on VL.VisitType = VT.Id 
	where HostId = '" + UserName + @"'
	AND (CONVERT(nvarchar(10),GETDATE(),121)
		between CONVERT(nvarchar(10),DateStart,121) and CONVERT(nvarchar(10),DateEnd,121)
		or CONVERT(nvarchar(10),GETDATE(),121) < CONVERT(nvarchar(10),DateStart,121))";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              LogId = row["LogId"].ToString(),
                              VisitorId = row["VisitorId"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              Status= row["Status"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              NeedApprove = row["NeedApprove"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Area = row["Area"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              DateEnd = Convert.ToDateTime(row["DateEnd"].ToString()).ToString("yyyy-MM-dd"),
                          }).ToList();
            return _datas;
        }

        public List<LogHistory> GetSecurityReportHistory(string dateFrom, string dateTo, string plant,
            string SearchVisitor, string SearchHost)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 
Usr.UseNam+' ('+Usr.UseDep+')' as UseNam, 
V.FullName, 
V.Company,
VT.VisitorType, 
VL.Plant,
VL.Remark,
isnull(VLD.[Date],VL.DateStart) as [DateStart],
CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, 
CONVERT(varchar,isnull(VLD.[TimeOut] ,'00:00'), 108) as TimeEnd, 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  as Duration,
(select AreaName + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
	INNER JOIN AreaVisitor AV on val = AreaId FOR XML PATH ('')) 
	as Area, 
isnull(VLD.[Status], VL.[Status]) as [Status], 
VT.NeedApprove as NeedApprove,
isnull(VLD.Remark, '') as RemarkVisitor,
CASE WHEN VT.NeedApprove = '0' THEN 'By System'
ELSE isnull(A.UseNam, 'PENDING APPROVED') end as ApprovedBy
FROM Visitlog VL
LEFT OUTER JOIN Visitor V on VL.VisitorId = V.id
LEFT OUTER JOIN Usr on VL.HostId = Usr.UseID
LEFT OUTER JOIN Usr A on VL.ApprovedBy = A.UseID
LEFT OUTER JOIN VisitorType VT on VL.VisitType = VT.Id 
LEFT OUTER JOIN VisitLogDet VLD on VL.LogId = VLD.LogId and VL.VisitorId = VLD.VisitorId 
where CONVERT(nvarchar(10),isnull(VLD.[Date],VL.DateStart), 121) between '"+dateFrom+"' and '"+dateTo+@"' and plant like '%"+plant+ @"%' and (Usr.UseNam like '%"+ SearchHost+ @"%' or Usr.UseID like '%" + SearchHost + @"%') and (V.FullName like '%" + SearchVisitor + @"%'
 or V.Company like '%" + SearchVisitor + @"%')order by isnull(VLD.[Date],VL.DateStart) desc";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from row in dt.AsEnumerable()
                          select new LogHistory
                          {
                              HostName = row["UseNam"].ToString(),
                              FullName = row["fullName"].ToString(),
                              Company = row["Company"].ToString(),
                              VisitorType = row["VisitorType"].ToString(),
                              Plant = row["Plant"].ToString(),
                              Remark = row["Remark"].ToString(),
                              DateStart = Convert.ToDateTime(row["DateStart"].ToString()).ToString("yyyy-MM-dd"),
                              TimeStart = row["TimeIn"].ToString(),
                              TimeEnd = row["TimeEnd"].ToString(),
                              Duration = row["Duration"].ToString(),
                              Area = row["Area"].ToString(),
                              Status = row["Status"].ToString(),
                              NeedApprove = row["NeedApprove"].ToString(),
                              RemarkVisitor = row["RemarkVisitor"].ToString(),
                              ApprovedBy = row["ApprovedBy"].ToString(),
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
	dateStart between '"+ dateFrom + @"' and '" + dateTo + @"'
	and t1.[status] ='PENDING'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
	                            where ShimanoBadge ='"+CardId+@"' 
	                            and GETDATE() between t1.Datestart and DATEADD(DAY, 1, DateEnd)
	                            and t1.[Status] in ('BREAK', 'CHECKIN') and t1.Plant='"+ Plant + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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

        public MessageNonQuery postSaveAppointment(VisitLog VisitLog, string query)
        {
            
            List<SqlVariable> paramss = new List<SqlVariable>();
            Type type = typeof(VisitLog);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var field in properties)
            {
                
                Console.WriteLine("{0} = {1}", field.Name, field.GetValue(VisitLog, null));
                //field.GetValue(0);
                SqlVariable param = new SqlVariable();
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
            return SQLCon.executeStoreProcNonQuery(paramss, query, ConnectionDB.VMSConnection);
        }

        public MessageNonQuery PostUpdateStatus(string Id, string LogId, string CardId, string Query, string Username = "", string Method ="", string Remark="")
        {
            List<SqlVariable> paramss = new List<SqlVariable>();
            paramss.Add(new SqlVariable { Name = "Id", Value = Id, Type = "String" });
            paramss.Add(new SqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new SqlVariable { Name = "CardId", Value = CardId, Type = "String" });
            paramss.Add(new SqlVariable { Name = "Username", Value = Username, Type = "String" });
            paramss.Add(new SqlVariable { Name = "Method", Value = Method, Type = "String" });
            paramss.Add(new SqlVariable { Name = "Remark", Value = Remark, Type = "String" });
            return SQLCon.executeStoreProcNonQuery(paramss, StoreProc.VisitorCheckIn, ConnectionDB.VMSConnection);
        }

        public List<Chart> ShowVisitorByHost()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT top 5 COUNT(distinct LogId) as counting, UseNam from VisitLog VL
inner join Usr on VL.HostId = Usr.UseId
where datestart between  DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE()), -1)
group by UseNam order by COUNT(distinct LogId) desc";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new Chart {
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
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new Chart
                          {
                              Data = Int16.Parse(rw["counting"].ToString()),
                              Name = rw["date"].ToString()

                          }).ToList();
            return _datas;
        }

        public List<LogHistory> ShowVisitorToday(string Name, string plant="2300", string Status="", string Host="")
        {
            DataTable dt = new DataTable();
            string query = @"
DECLARE @Name varchar(MAX) = '"+ Name + @"'
DECLARE @Host varchar(MAX) = '" + Host + @"'
DECLARE @Plant varchar(10) = '" + plant + @"'
DECLARE @Status varchar(10) = '" + Status + @"'


SELECT VL.VisitorId, CONVERT(varchar,isnull(VLD.TimeIn ,'00:00'), 108) as TimeIn, 
V.FullName + '<br/> (' + V.Company + ')' as Visitor, V.Phone,
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) / 60 % 24 as nvarchar) + ' Hour(s) : ' + 
CAST (DATEDIFF(MINUTE,isnull(VLD.[TimeIn], CONVERT(TIME, GETDATE(),103)), 
isnull(VLD.[TIMEOUT], CONVERT(TIME, GETDATE(),103))) % 60 as nvarchar) + ' Minute(s)'  as Duration,
VT.VisitorType, UH.UseNam+' ('+UH.UseDep+')' + '<br/> - Ext. ' + UH.UseTel as Host, (select AreaName + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
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
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
@isAllVendor nvarchar(10) = '"+IsAllVendor+@"',
@LogId nvarchar(10) = '"+ LogId + @"',
@VisitorId nvarchar(10) = '"+ VisitorId + @"'

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
            return (from rw in SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
                    select new Visitor
                    {
                        FullName = rw["FullName"].ToString(),
                        Company = rw["Company"].ToString(),
                        JobDesc = rw["JobDesc"].ToString(),
                    }).ToList();

        }

        public MessageNonQuery PostChangeReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            List<SqlVariable> paramss = new List<SqlVariable>();
            paramss.Add(new SqlVariable { Name = "VisitorId", Value = VisitorId, Type = "String" });
            paramss.Add(new SqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new SqlVariable { Name = "IsAllVendor", Value = IsAllVendor, Type = "String" });
            return SQLCon.executeStoreProcNonQuery(paramss, StoreProc.ChangeStatusReq, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery PostCancelReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            string query = @"
DECLARE @LogId nvarchar(20) = '"+LogId+ @"',
		@IsAllVendor nvarchar(5) = '" + IsAllVendor + @"',
		@VisitorId nvarchar(20) = '" + VisitorId + @"'

UPDATE VL set [Status] = 'CANCELED'
	FROM VisitLog VL 
	WHERE LogId=@LogId and VisitorId like CASE WHEN @IsAllVendor = 1 then '%%' else @VisitorId END ";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }

        public List<BarChart> GetSummaryVisitorPerYear(string Plant)
        {
            List<SqlVariable> paramss = new List<SqlVariable>();
            paramss.Add(new SqlVariable { Name = "Plant", Value = Plant, Type = "String" });
            DataTable dt = SQLCon.executeStoreGetDataTable(paramss, StoreProc.GetSummaryVisitorPerYear, ConnectionDB.VMSConnection);
            return (from rw in dt.AsEnumerable()
                    select new BarChart
                    {
                        ParentColumn = rw["ParentColumn"].ToString(),
                        SubColumn = rw["SubColumn"].ToString(),
                        Summary = double.Parse(rw["Summary"].ToString()),
                    }).ToList();
        }
        public List<BarChart> GetSummaryVisitorPerMonth(string Plant, string Month)
        {
            List<SqlVariable> paramss = new List<SqlVariable>();
            paramss.Add(new SqlVariable { Name = "Plant", Value = Plant, Type = "String" });
            paramss.Add(new SqlVariable { Name = "Month", Value = Month, Type = "String" });
            DataTable dt = SQLCon.executeStoreGetDataTable(paramss, StoreProc.GetSummaryVisitorPerMonth, ConnectionDB.VMSConnection);
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

, VT.VisitorType, U.UseNam+' ('+U.UseDep+')' + ' - Ext. ' + U.UseTel as Host, (select AreaName + ', ' AS [text()] from ufn_String_To_Table (area,',',1) SN
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
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
            return (from rw in SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection).AsEnumerable()
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
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
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
    }
}
