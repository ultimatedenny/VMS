using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class DeliveryAction
    {
        //DataTable dt;
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();

        public List<MDeliveryOrder> _Show_DO_Outstanding_Manager(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster WHERE DONo like '%" + KeyWord + @"%' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' and ManagerApprove = '0' and Status = 'WAITING HOD APPROVAL' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_Receptionist(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster WHERE DONo like '%" + KeyWord + @"%' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' and ManagerApprove = '1' and Status = 'WAITING DRIVER' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_Security(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster WHERE DONo like '%" + KeyWord + @"%' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' and ManagerApprove = '1' and Status = 'OUTPOST CHECK' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_Admin(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster WHERE DONo like '%" + KeyWord + @"%' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_User(string dateFrom, string dateTo, string KeyWord, string _User)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, 
                             ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster 
                             WHERE DONo like '%" + KeyWord + @"%' and UseID like '%" + _User + @"%' and Status = 'WAITING HOD APPROVAL' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_Driver(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster 
                            WHERE DONo like '%" + KeyWord + @"%' and ReqDate between '" + dateFrom + "' and '" + dateTo + "' and ManagerApprove = '1' and DelVia = 'SBM_DRIVER' and Status = 'WAITING DRIVER' or Status = 'OUTPOST CHECK' or Status = 'DELIVERING' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDeliveryOrder> _Show_DO_Outstanding_Driver_Delivered(string dateFrom, string Requestor, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster 
                            WHERE DONo like '%" + KeyWord + @"%' and UseID like '%" + Requestor + @"%' and ReqDate = '" + dateFrom + "' and ManagerApprove = '1' and DelVia = 'SBM_DRIVER' and Status = 'DELIVERED' or Status = 'CANCELED' order by DONo";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDeliveryOrder
                          {
                              MasterId = rw["MasterId"].ToString(),
                              SENo = int.Parse(rw["SENo"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseID = rw["UseID"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              Address = rw["Address"].ToString(),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              ReceivedPic = rw["ReceivedPic"].ToString(),
                              SecurityCheck = rw["SecurityCheck"].ToString(),
                              SecurityPic = rw["SecurityPic"].ToString(),
                              ManagerApprove = rw["ManagerApprove"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDDeliveryOrder> _Show_DO_Returned(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select DD.[Id] as [Id], DM.DONo as DONo, DM.UseID as Requestor, DM.[Address] as [Address], DD.Product as Product, DD.Quantity as Quantity, DD.IsReturned as IsReturned, DD.ReturnedBy as ReturnedBy from DOMaster DM inner join DODetails DD on DM.MasterId = DD.MasterId 
                             where DD.IsReturned = '1' and DM.[Status] = 'DELIVERED' and DONo like '%" + KeyWord + "%' and DD.ReturnedDate is null order by DM.DONo";
            
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDDeliveryOrder
                          {
                              Id = int.Parse(rw["Id"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              UseID = rw["Requestor"].ToString(),
                              Address = rw["Address"].ToString(),
                              Product = rw["Product"].ToString(),
                              Quantity = int.Parse(rw["Quantity"].ToString()),
                              ReturnedBy = DateTime.Parse(rw["ReturnedBy"].ToString()).ToString("yyyy-MM-dd"),
                              IsReturned = rw["IsReturned"].ToString(),
                          }).ToList();
            return _datas;
        }
        public List<MDDeliveryOrder> _Show_DO_AllItem(string dateFrom, string dateTo, string KeyWord)
        {
            DataTable dt = new DataTable();
            string query = @"select DD.[Id] as [Id], DM.DONo as DONo, DM.ReqDate as ReqDate,DM.UseID as Requestor, DM.UseDep as UseDep, DM.[Address] as [Address], DD.Product as Product, DD.Quantity as Quantity, DM.DelVia as DelVia,DM.DriName as DriName,DM.VechNo as VechNo,DM.[TimeOut] as [TimeOut],DM.SealNo as SealNo,DM.ContainerNo as ContainerNo,DM.ReceiverName as ReceiverName,DM.ReceivedDate as ReceivedDate,DD.IsReturned as IsReturned, DD.ReturnedBy as ReturnedBy, DM.[Status] as [Status] from DOMaster DM inner join DODetails DD on DM.MasterId = DD.MasterId where DONo like '%" + KeyWord + "%' and DM.[Status] = 'DELIVERED' and  DM.ReqDate between '" + dateFrom + "' and '" + dateTo + "' order by DM.DONo";

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _datas = (from rw in dt.AsEnumerable()
                          select new MDDeliveryOrder
                          {
                              Id = int.Parse(rw["Id"].ToString()),
                              DONo = rw["DONo"].ToString(),
                              ReqDate = DateTime.Parse(rw["ReqDate"].ToString()).ToString("yyyy-MM-dd"),
                              UseID = rw["Requestor"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              Address = rw["Address"].ToString(),
                              Product = rw["Product"].ToString(),
                              Quantity = int.Parse(rw["Quantity"].ToString()),
                              DelVia = rw["DelVia"].ToString(),
                              DriName = rw["DriName"].ToString(),
                              VechNo = rw["VechNo"].ToString(),
                              TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                              SealNo = rw["SealNo"].ToString(),
                              ContainerNo = rw["ContainerNo"].ToString(),
                              ReceiverName = rw["ReceiverName"].ToString(),
                              ReceivedDate = rw["ReceivedDate"].ToString(),
                              IsReturned = rw["IsReturned"].ToString(),
                              ReturnedBy = rw["ReturnedBy"].ToString(),
                              Status = rw["Status"].ToString(),
                          }).ToList();
            return _datas;
        }
        public DataTable ExportDeliveryOrder(string DateStart, string DateEnd, string DONo)
        {
            DataTable dt = new DataTable();
            string query = $@"spGetDeliveryOrder";
            var param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "DateStart", Type = "string", Value = DateStart });
            param.Add(new ctSqlVariable { Name = "DateEnd", Type = "string", Value = DateEnd });
            param.Add(new ctSqlVariable { Name = "DONo", Type = "string", Value = DONo });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            return dt;
        }
    }
}