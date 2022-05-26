using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Constants;
using VMS.Library.Models;

namespace VMS.Library.Actions
{
    public class LogBookAction
    {
        Database SQLCon = new Database();

        public DataTable ExportLogBook(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT TypeName, NameGrant, InfoGrant, DateGrant, TimeGrant, NameReceive,DateReceive,TimeReceive,Remark
FROM LogBook lb inner join LogType lt on lb.typeId=lt.TypeId Where lb.Status='DELIVERED' and dateGrant between '" + dateFrom + "' and '" + dateTo + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            return dt;
        }

        public MessageNonQuery SaveReceivebySecurity(LogBook LogBook)
        {
            string query = @"INSERT INTO logbook(LogId, TypeID, InfoGrant, NameGrant, DateGrant, TimeGrant, PhotoGrant, Remark, [Status], Plant)
              select isnull(MAX(LogId),'0')+1, '" + LogBook.TypeId + @"','" + LogBook.InfoGrant + @"' ,'" + LogBook.NameGrant + @"', convert(varchar(10), GETDATE(), 120),  convert(varchar(10), GETDATE(), 108),'" + LogBook.PhotoGrant + @"', '" + LogBook.Remark + @"', 'RECEIVED', '" + LogBook.Plant + @"' from logbook";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }

        public MessageNonQuery SaveReceivebyUser(string LogId, string Photo, string NameReceive)
        {
            string query = @"Update Logbook set NameReceive = '" + NameReceive + @"', DateReceive = convert(varchar(10),GETDATE(), 120), TimeReceive = convert(varchar(10), GETDATE(), 108), PhotoReceive = '" + Photo + @"', status = 'DELIVERED' where LogId = '" + LogId + @"'";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }

        public PhotoLogBook SeePhotoes(string LogId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [PhotoGrant], [PhotoReceive] from Logbook where LogId='" + LogId + @"'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var data = (from rw in dt.AsEnumerable()
                        select new PhotoLogBook
                        {
                            PhotoItem = rw["PhotoGrant"].ToString(),
                            PhotoUser = rw["PhotoReceive"].ToString()
                        }).FirstOrDefault();
            return data;
        }

        public List<LogBook> ShowAllLogBook(string dateFrom, string dateTo, string KeyWord, string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT LogId, TypeName, NameGrant, InfoGrant, DateGrant, TimeGrant, NameReceive,DateReceive,TimeReceive,Remark
FROM LogBook lb inner join LogType lt on lb.typeId=lt.TypeId Where lb.Status='DELIVERED' and dateGrant between '" + dateFrom + "' and '" + dateTo + "' and (Plant='" + Plant + @"' or plant is null)";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogBook
                          {
                              LogId = int.Parse(rw["LogId"].ToString()),
                              TypeName = rw["TypeName"].ToString(),
                              NameGrant = rw["NameGrant"].ToString(),
                              InfoGrant = rw["InfoGrant"].ToString(),
                              DateGrant = DateTime.Parse(rw["DateGrant"].ToString()).ToString("yyyy-MM-dd"),
                              TimeGrant = TimeSpan.Parse(rw["TimeGrant"].ToString()).ToString(@"hh\:mm"),
                              NameReceive = rw["NameReceive"].ToString() != "" ? rw["NameReceive"].ToString() : "",
                              DateReceive = rw["DateReceive"].ToString() != "" ? DateTime.Parse(rw["DateReceive"].ToString()).ToString("yyyy-MM-dd") : "",
                              TimeReceive = rw["TimeReceive"].ToString() != "" ? TimeSpan.Parse(rw["TimeReceive"].ToString()).ToString(@"hh\:mm") : "",
                              Remark = rw["Remark"].ToString()
                          }).ToList();
            return _datas;
        }

        public List<LogBook> ShowLogBook(string TypeId, string dateFrom, string dateTo, string KeyWord, string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"select LogId, InfoGrant, NameGrant, DateGrant, TimeGrant, Remark, LB.Status
from LogBook LB WHERE LB.TypeId='" + TypeId + "' and dateGrant between '" + dateFrom + "' and '" + dateTo + "' and (NameGrant Like '%" + KeyWord + "%' or infoGrant Like '%" + KeyWord + "%') and Status = 'RECEIVED' and (Plant='" + Plant + @"' or plant is null)";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogBook {
                              LogId = int.Parse(rw["LogId"].ToString()),
                              InfoGrant = rw["InfoGrant"].ToString(),
                              NameGrant = rw["NameGrant"].ToString(),
                              DateGrant = DateTime.Parse(rw["DateGrant"].ToString()).ToString("yyyy-MM-dd"),
                              TimeGrant = TimeSpan.Parse(rw["TimeGrant"].ToString()).ToString(@"hh\:mm"),
                              Remark = rw["Remark"].ToString(),
                              Status = rw["Status"].ToString()
                          }).ToList();
            return _datas;
        }
    }

}
