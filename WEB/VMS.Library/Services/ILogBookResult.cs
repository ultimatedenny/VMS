using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface ILogBookResult
    {
        MessageNonQuery SaveReceivebySecurity(LogBook LogBook);
        MessageNonQuery SaveReceivebyUser(string LogId, string Photo, string NameReceive);
        List<LogBook> ShowLogBook(string TypeId, string dateFrom, string dateTo, string KeyWord);
        List<LogBook> ShowAllLogBook(string dateFrom, string dateTo, string KeyWord);
        DataTable ExportLogBook(string dateFrom, string dateTo, string KeyWord);

        PhotoLogBook SeePhotoes(string LogId);
    }
}
