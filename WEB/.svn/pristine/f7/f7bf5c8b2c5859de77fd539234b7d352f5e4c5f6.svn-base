using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{

    public class ReportController : SController
    {
        VisitorAction _visitorAction = new VisitorAction();
        VisitLogAction _visitLogAction = new VisitLogAction();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SecurityReport()
        {
            return View();
        }
        public JsonResult GetSecurityReportHistory(string dateFrom, string dateTo, string plant, string SearchVisitor, string SearchHost)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                //Creating instance of DatabaseContext class

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                
                var _visitors = _visitLogAction.GetSecurityReportHistory(dateFrom, dateTo, plant, SearchVisitor, SearchHost);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult VisitorLog()
        {
            return View();
        }
        public JsonResult GetVisitorLog([DataSourceRequest] DataSourceRequest request, string dateFrom, string dateTo, string plant, string SearchVisitor, string SearchHost)
        {
            return Json(_visitLogAction.GetSecurityReportHistory(dateFrom, dateTo, plant, SearchVisitor, SearchHost).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPendingVisitor()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                //Creating instance of DatabaseContext class

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                string _UseDept = Session["UseDep"].ToString(); //change to session username
                string _UseID = Session["UseID"].ToString(); //Added by CANICE 20210930
                var _visitors = _visitLogAction.GetPendingVisitLog(_UseDept, _UseID); //Modified by CANICE 20210930 //var _visitors = _visitLogAction.GetPendingVisitLog(_UseDept);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult getListVisitLogHistory(string DateFrom, string DateTo)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                //Creating instance of DatabaseContext class

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                string _userName = Session["UseID"].ToString(); //change to session username
                var _visitors = _visitLogAction.GetHistoryVisitLog(_userName, DateFrom, DateTo);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public FileResult ExportSecurityHis(string DateFromHis, string DateToHis, string Plant)
        {
            DataTable table = new DataTable("HistoryReport");
            table = _visitLogAction.ExportSecurityHis(DateFromHis, DateToHis, Plant);
            table.TableName = "HistoryReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_HistoryVisitLog" + DateFromHis + "_" + DateToHis + ".xlsx");
                }
            }
        }
        public JsonResult GetChangeReqVisit(string VisitorId, string LogId, string IsAllVendor)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                //Creating instance of DatabaseContext class

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                string _userName = Session["UseID"].ToString(); //change to session username
                var _visitors = _visitLogAction.GetChangeReqVisit(VisitorId, LogId, IsAllVendor);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult PostChangeReqVisit(string VisitorId, string LogId, string IsAllVendor, string Method)
        {
            if (Method.ToUpper() == "CHANGE")
            {
                return Json(_visitLogAction.PostChangeReqVisit(VisitorId, LogId, IsAllVendor), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_visitLogAction.PostCancelReqVisit(VisitorId, LogId, IsAllVendor), JsonRequestBehavior.AllowGet);
            }
            
        }

        //
        //
        public ActionResult RunningVisitorLog()
        {
            return View();
            
        }
        public JsonResult GetRunningVisitorLog([DataSourceRequest] DataSourceRequest request)
        {
            string _UseDept = Session["UseDep"].ToString();
            string _UseID = Session["UseID"].ToString(); //Added by CANICE 20210930
            //return Json(_visitLogAction.GetPendingVisitLog(_UseDept).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return Json(_visitLogAction.GetPendingVisitLog(_UseDept, _UseID).ToDataSourceResult(request), JsonRequestBehavior.AllowGet); //Modified by CANICE 20210930
        }
        public JsonResult GetRunningVisitorLogDet([DataSourceRequest] DataSourceRequest request, string LogId)
        {
            return Json(_visitLogAction.GetPendingVisitLogDet(LogId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostChangeStatusAll(string LogId, string VisitorId, string VisitorType)
        {
            return Json(_visitLogAction.PostChangeStatusAll(LogId,VisitorId,VisitorType), JsonRequestBehavior.DenyGet);
        }
        public JsonResult PostCancelAll(string LogId, string VisitorId)
        {
            return Json(_visitLogAction.PostCancelAll(LogId, VisitorId), JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetDisclaimerDetail(string LogId, string FullName, string Company, string VisitorType, string Plant)
        {
            List<DisclaimerArea> _Data = new List<DisclaimerArea>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                _Data = _visitLogAction.GetDisclaimerDetail(LogId, FullName, Company, VisitorType, Plant);
                j.Data.Add(_Data);
                j.Result = true;

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
    }
}