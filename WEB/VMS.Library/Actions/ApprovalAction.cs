using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;
using VMS.Library.Class;
using System.Data;
using VMS.Library.Constants;

namespace VMS.Library.Actions
{
    public class ApprovalAction
    {
        Database SQLCon = new Database();
        public MessageNonQuery ApproveAction(string Username, string LogId, string Status, string ApprovedRemark= "")
        {
            List<SqlVariable> paramss = new List<SqlVariable>();
            paramss.Add(new SqlVariable { Name = "LogId", Value = LogId, Type = "String" });
            paramss.Add(new SqlVariable { Name = "ApprovedBy", Value = Username, Type = "String" });
            paramss.Add(new SqlVariable { Name = "Status", Value = Status, Type = "String" });
            paramss.Add(new SqlVariable { Name = "ApprovedRemark", Value = ApprovedRemark, Type = "String" });
            return SQLCon.executeStoreProcNonQuery(paramss, StoreProc.ApprovalAction, ConnectionDB.VMSConnection);
        }

        public MessageNonQuery ChangeDelegate(string isDelegate, string UserName)
        {
            string query = @"UPDATE Usr set isDelegate='" + isDelegate + "' where UseID='"+UserName+"'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }

        public MessageNonQuery CheckAuthorize(string UsePlant, string Uselev)
        {
            DataTable dt = new DataTable();
            string query = @"DECLARE 
                            @UseLev nvarchar(1) = '" + Uselev + @"',
                            @Plant nvarchar (4) = '" + UsePlant + "'";
            query += @"IF @UseLev = '1'
                begin
	                select 1 as isSuccess,'You Have Access to Approve' as [Message]
                end
				else if (@UseLev = '2' AND NOT EXISTS(SELECT * FROM Usr where UseLev = cast (@UseLev as INT) - 1 and UsePlant=@Plant and isDelegate=0))
				begin
	                select 1 as isSuccess,'You Have Access to Approve' as [Message]
                end
				else if (@UseLev = '3' AND NOT EXISTS(SELECT * FROM Usr where UseLev = cast (@UseLev as INT) - 1 and UsePlant=@Plant and isDelegate=0))
				begin
	                select 1 as isSuccess,'You Have Access to Approve' as [Message]
                end
				else
				BEGIN
				select 0 as isSuccess,'You do not Have Access to Approve' as [Message]
				END";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitLogArea = (from rw in dt.AsEnumerable()
                                 select new MessageNonQuery
                                 {
                                     isSuccess = rw["isSuccess"].ToString() == "1" ? true : false,
                                     message = rw["Message"].ToString()
                                 }).FirstOrDefault();
            return _visitLogArea;
        }

        public MessageNonQuery CheckDelegate(string Username)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT isDelegate, CASE WHEN 
                            isDelegate = 0 then 'FALSE' else 'TRUE' end as [message]
                            FROM Usr where UseID='" + Username + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                         select new MessageNonQuery
                         {
                             isSuccess = (rw["isDelegate"].ToString().ToUpper() == "TRUE" ? true : false),
                             message = rw["message"].ToString()
                         }).FirstOrDefault();
            return _datas;
        }

        public List<LogHistory> ShowHistoryApproval(string Username)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 
            t1.LogId,
            t1.Area,
			UseNam,
            STUFF(
                    (SELECT '; ' + FullName + ' (' + Company + ')'
                    FROM VisitLog V1
                    INNER JOIN Visitor  V2
                    ON V2.id = V1.VisitorId  where logid=t1.logid
                    ORDER BY V2.Company
                    FOR XML PATH('')), 1, 1, ''
            ) 
            [VisitorName], Remark, datestart, dateend, timeStart, TimeEnd, t1.status
            FROM VisitLog t1 
			INNER JOIN Usr on t1.hostId = UseId where t1.ApprovedBy='"+ Username + @"'
and [ApprovedAt] between DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) and DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE()), -1)            
GROUP BY 
            t1.LogId, t1.Area, UseNam, datestart, dateend, Remark, timeStart, TimeEnd, t1.status";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitLog = (from rw in dt.AsEnumerable()
                             select new LogHistory
                             {
                                 Area = rw["Area"].ToString(),
                                 HostName = rw["UseNam"].ToString(),
                                 FullName = rw["VisitorName"].ToString(),
                                 Remark = rw["Remark"].ToString(),
                                 DateStart = Convert.ToDateTime(rw["datestart"].ToString()).ToString("yyyy-MM-dd"),
                                 DateEnd = Convert.ToDateTime(rw["dateend"].ToString()).ToString("yyyy-MM-dd"),
                                 TimeStart = rw["timeStart"].ToString(),
                                 TimeEnd = rw["TimeEnd"].ToString(),
                                 Status = rw["status"].ToString(),
                             }).ToList();
            return _visitLog;
        }
        public List<LogHistory> ShowRequestApproval()
        {
            
            DataTable dt = new DataTable();
            string query = $@"DECLARE @ApprovalGroup varchar(20) = '{Sessions.GetApprovalGroup() ?? ""}'
SELECT 
            t1.LogId,
            t1.Area,
			U.UseNam,
			U.UseDep,
            STUFF(
                    (SELECT '; ' + FullName + ' (' + Company + ')<br> '
                    FROM VisitLog VL2
                    INNER JOIN Visitor  V2
                    ON V2.id = VL2.VisitorId  where logid=t1.logid
                    ORDER BY V2.Company
                    FOR XML PATH('')), 1, 1, ''
            ) 
            [VisitorName], Remark, datestart, dateend, timeStart, TimeEnd, t1.status
            FROM VisitLog t1 
			INNER JOIN Usr U on t1.hostId = UseId 
			INNER JOIN ApprovalGroup AG on U.UseDep = AG.Dept
			where t1.status='PENDING' and t1.Plant='{Sessions.GetUsePlant()}' and AG.ApprovalGroup=@ApprovalGroup
            GROUP BY 
            t1.LogId, t1.Area, UseNam, U.UseDep,datestart, dateend, Remark, timeStart, TimeEnd, t1.status";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitLog = (from rw in dt.AsEnumerable()
                             select new LogHistory
                             {
                                 LogId = rw["LogId"].ToString(),
                                 Area = rw["Area"].ToString(),
                                 HostName = rw["UseNam"].ToString(),
                                 FullName = rw["VisitorName"].ToString(),
                                 Remark = rw["Remark"].ToString(),
                                 DateStart = Convert.ToDateTime(rw["datestart"].ToString()).ToString("yyyy-MM-dd"),
                                 DateEnd = Convert.ToDateTime(rw["dateend"].ToString()).ToString("yyyy-MM-dd"),
                                 TimeStart = rw["timeStart"].ToString(),
                                 TimeEnd = rw["TimeEnd"].ToString(),
                                 Status = rw["status"].ToString(),

                             }).ToList();
            return _visitLog;
        }
    }
}
