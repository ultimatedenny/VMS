using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface IVisitLogResult
    {
        //GET
        List<LogHistory> getHistoryVisitLog(string UserName, string dateFrom, string dateTo);
        List<LogHistory> getPendingVisitLog(string UserName, string dateFrom, string dateTo);
        List<LogHistory> GetSecurityReportPending(string dateFrom, string dateTo);
        List<LogHistory> GetSecurityReportHistory(string dateFrom, string dateTo, string plant);
        LogHistory GetVisitorInArea(string CardId);
        List<LogHistory> ShowVisitorToday(string Name, string plant="2300");
        DataTable ExportSecurityPreHis(string dateFrom, string dateTo);
        DataTable ExportSecurityHis(string dateFrom, string dateTo, string plant);
        List<Chart> ShowVisitorOneMonth();
        List<Chart> ShowVisitorByHost();



        //POST
        MessageNonQuery PostUpdateStatus(string Id, string LogId, string CardId, string Query, string Username = "", string Method="", string Remark="");
        MessageNonQuery postSaveAppointment(VisitLog VisitLog, string query);

    }
}
