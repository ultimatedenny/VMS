using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class MLogBook
    {
        public int LogId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string InfoGrant { get; set; }
        public string NameGrant { get; set; }
        public string DateGrant { get; set; }
        public string TimeGrant { get; set; }
        public string PhotoGrant { get; set; }
        public string NameReceive { get; set; }
        public string DateReceive { get; set; }
        public string TimeReceive { get; set; }
        public string PhotoReceive { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Plant { get; set; }
    }
    public class LogBookAction
    {
        DataTable dt;
        long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public DataTable ExportLogBook(string dateFrom, string dateTo, string KeyWord)
        {
            string query = @"SELECT TypeName, NameGrant, InfoGrant, DateGrant, TimeGrant, NameReceive,DateReceive,TimeReceive,Remark
FROM LogBook lb inner join LogType lt on lb.typeId=lt.TypeId Where lb.Status='DELIVERED' and dateGrant between '" + dateFrom + "' and '" + dateTo + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            return dt;
        }

        public VMSRes<string> SaveReceivebySecurity(LogBook LogBook)
        {
            string query = @"INSERT INTO logbook(LogId, TypeID, InfoGrant, NameGrant, DateGrant, TimeGrant, PhotoGrant, Remark, [Status], Plant)
              select isnull(MAX(LogId),'0')+1, '" + LogBook.TypeId + @"','" + LogBook.InfoGrant + @"' ,'" + LogBook.NameGrant + @"', convert(varchar(10), GETDATE(), 120),  convert(varchar(10), GETDATE(), 108),'" + LogBook.PhotoGrant + @"', '" + LogBook.Remark + @"', 'RECEIVED', '" + LogBook.Plant + @"' from logbook";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }

        public VMSRes<string> SaveReceivebyUser(string LogId, string Photo, string NameReceive)
        {
            string query = @"Update Logbook set NameReceive = '" + NameReceive + @"', DateReceive = convert(varchar(10),GETDATE(), 120), TimeReceive = convert(varchar(10), GETDATE(), 108), PhotoReceive = '" + Photo + @"', status = 'DELIVERED' where LogId = '" + LogId + @"'";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();   
        }

        public PhotoLogBook SeePhotoes(string LogId)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [PhotoGrant], [PhotoReceive] from Logbook where LogId='" + LogId + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
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
            string query = $@"SELECT LogId, TypeName, NameGrant, InfoGrant, DateGrant, TimeGrant, NameReceive,DateReceive,TimeReceive,Remark
FROM LogBook lb inner join LogType lt on lb.typeId=lt.TypeId 
Where lb.Status='DELIVERED' and dateGrant between '{dateFrom}' and '{dateTo}' and (Plant='{Plant}' or plant is null) and (NameGrant like '%{KeyWord}%')";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
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
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new LogBook
                          {
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