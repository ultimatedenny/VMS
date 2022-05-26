using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class NewVisitorController : SController
    {
        NewVisitor _Visitor = new NewVisitor();
        // GET: NewVisitor
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetVisitorOrLogByCard(string CardId)
        {
            return Json(_Visitor.GetVisitorOrLogByCard(CardId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVisitorDetailById(string VisitorId)
        {
            return Json(_Visitor.GetVisitorDetailById(VisitorId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHostAppointment(string VisitorId, string LogId)
        {
            return Json(_Visitor.GetHostAppointment(LogId, VisitorId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVisitorsForAppointment([DataSourceRequest] DataSourceRequest request,  string NameorCompany)
        {
            return Json(_Visitor.GetVisitorsForAppointment(NameorCompany).data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostUpdateVisitorWhileCheckin(Visitor Visitor)
        {
            return Json(_Visitor.PostUpdateVisitorWhileCheckin(Visitor), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVisitorToCheckout(string VisitorId, string LogId)
        {
            var data = _Visitor.GetVisitorToCheckout(VisitorId, LogId);
            return Json(data.data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostVisitorCheckinOut(string VisitorId, string logId, string CardId, string Method, string Remark)
        {
            var data = _Visitor.PostVisitorCheckinOut(VisitorId, logId, CardId, "", Method, Remark);
            TempData["SUCCESS"] = data.Success;
            TempData["MESSAGE"] = data.Message;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostSaveAppointment(VisitLog VisitLog)
        {

            VisitLog.CreUser = Sessions.GetUseID();
            VisitLog.DateStart = DateTime.Now;
            VisitLog.DateEnd = DateTime.Now;
            VisitLog.Area = ConfigurationManager.AppSettings["OthersRoomCode"].ToString();
            VisitLog.Plant = Sessions.GetUsePlant();
            VisitLog.TimeStart = DateTime.Now.TimeOfDay;
            VisitLog.TimeEnd = DateTime.Now.AddHours(4).TimeOfDay;
            var datas = new VMSRes<string>();


            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHistoryVisitorByIdStoreProc([DataSourceRequest] DataSourceRequest request, string VisitorId)
        {
            VisitLogAction vl = new VisitLogAction();
            return Json(vl.GetHistoryVisitorByIdStoreProc(VisitorId).data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetisUnknown(string LogID)
        {
            return Json(_Visitor.GetisUnknown(LogID), JsonRequestBehavior.AllowGet);
            //try
            //{
            //    List<VisitType> _VisitorType = new List<VisitType>();
            //    _VisitorType = _Visitor.GetisUnknown(LogID);
            //    var _retData =
            //       new
            //       {
            //           xdataTable = _VisitorType
            //       };
            //    return Json(_retData, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

    }
}