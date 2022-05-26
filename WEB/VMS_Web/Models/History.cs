using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class History
    {
        DataTable dt;
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public List<EPMaster2> GetExitPermitDatatablesHistory(string ExitPermitNo, string UseId, string DateFrom, string DateTo)
        {
            string query = @"SELECT	EM.MasterId as Id, 
                                EM.SENo as No,
		                        EM.EPNo as ExitPermit,
		                        EM.UseDep as Departement,
		                        P.PlantName as Section,
		                        EM.Destination as Destination,
								EM.CreatedBy as Requestor,
		                        Convert(nvarchar(10),EM.[Date],121) as [Date],
		                        EM.[Out] as [Out],
		                        EM.[In] as [In],
		                        EM.CompTrans as CompanyTransport,
		                        EM.CompTransTime as CompanyTransportTime,
		                        EM.[Status] as [Status]
                                from EPMASTER EM
                                INNER JOIN Plant P on EM.PlantID = P.plantId where EM.EPNo like '%" + ExitPermitNo + @"%' and EM.CreatedBy = '" + UseId + @"' and
                                                                                   EM.[Date] between '" + DateFrom + @"' and '" + DateTo + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _ExitPermit = (from rw in dt.AsEnumerable()
                               select new EPMaster2
                               {
                                   MasterId = rw["Id"].ToString(),
                                   SENo = int.Parse(rw["No"].ToString()),
                                   EPNo = rw["ExitPermit"].ToString(),
                                   UseDep = rw["Departement"].ToString(),
                                   PlantID = rw["Section"].ToString(),
                                   Destination = rw["Destination"].ToString(),
                                   Date = Convert.ToDateTime(rw["Date"].ToString()).ToString("yyyy-MM-dd"),
                                   In = TimeSpan.Parse(rw["In"].ToString()).ToString(),
                                   Out = TimeSpan.Parse(rw["Out"].ToString()).ToString(),
                                   CompTrans = bool.Parse(rw["CompanyTransport"].ToString()).ToString(),
                                   CompTransTime = TimeSpan.Parse(rw["CompanyTransportTime"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                                   CreatedBy = rw["Requestor"].ToString(),
                               }).ToList();
            return _ExitPermit;
        }
        public List<Attendance> GetAttendanceDatatableHistory(string VisitorId1, string DateFrom, string DateTo)
        {
            string query = @"SELECT LOGID, TSVISITORID, HOSTNAME, HOSTDEPARTMENT, TIMEIN, TIMEOUT = ISNULL(TimeOut, '00:00:00' ), DATEVISIT, NEEDLUNCH, STATUS, PLANT, REMARK = ISNULL(REMARK, '-' ), NEEDSTAY, DATEOFEND, PREMISES = ISNULL(PREMISES, 'SBM' ) FROM VisitLog_TS
WHERE [TSVisitorId] LIKE  '%" + VisitorId1 + @"%' and DateVisit between '" + DateFrom + @"' and '" + DateTo + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _Attendance = (from rw in dt.AsEnumerable()
                               select new Attendance
                               {
                                   LogId = int.Parse(rw["LogId"].ToString()),
                                   TSVisitorId = int.Parse(rw["TSVisitorId"].ToString()),
                                   HostName = rw["HostName"].ToString(),
                                   HostDepartment = rw["HostDepartment"].ToString(),
                                   TimeIn = TimeSpan.Parse(rw["TimeIn"].ToString()).ToString(),
                                   TimeOut = TimeSpan.Parse(rw["TimeOut"].ToString()).ToString(),
                                   DateVisit = DateTime.Parse(rw["DateVisit"].ToString()).ToString("yyyy-MM-dd"),
                                   NeedLunch = bool.Parse(rw["NeedLunch"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                                   //Plant = rw["Plant"].ToString(),
                                   //Remark = rw["Remark"].ToString(),
                                   //NeedStay = bool.Parse(rw["NeedStay"].ToString()).ToString(),
                                   DateofEnd = DateTime.Parse(rw["DateVisit"].ToString()).ToString("yyyy-MM-dd"),
                                   Premises = rw["Premises"].ToString(),
                               }).ToList();
            return _Attendance;
        }
        public List<MDeliveryOrder> GetDeliveryOrderDatatable(string dateFrom, string dateTo, string KeyWord, string UseId)
        {
            DataTable dt = new DataTable();
            string query = @"select MasterId, SENo, DONo, UseDep, UseID, ReqDate, [Address] as Address , DelVia, 
                             DriName, VechNo, [TimeOut] as TimeOut, ContainerNo, SealNo, ReceiverName, ReceivedDate, 
                             ReceivedPic, SecurityCheck, SecurityPic, ManagerApprove, [Status] as Status from DOMaster 
                             WHERE DONo like '%DO%' and UseID like '%" + UseId + @"%'and ReqDate between '" + dateFrom + "' and '" + dateTo + "'";
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

    }
}