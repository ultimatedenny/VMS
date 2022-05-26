using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure.Implementation;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class HistoryController : Controller
    {
        History _History = new History();
        
        // GET: History
        public string _UseId, _VisitorId;
        [HttpGet]
        public ActionResult Index(string UseId, string VisitorId)
        {
            HttpCookie cookie = new HttpCookie("cookie_useid", UseId);
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);

            HttpCookie cookie2 = new HttpCookie("cookie_visitorid", VisitorId);
            cookie2.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie2);
            return View();
        }
        [HttpGet]
        public ActionResult Index2(string UseId)
        {
            HttpCookie cookie = new HttpCookie("cookie_useid", UseId);
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
            return View();
        }
        public JsonResult GetExitPermitDatatables(string ExitPermitNo, string DateFrom, string DateTo)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_useid");
                _UseId = cookie2.Value.ToString();

                string UseId = _UseId;
                var _Permit = _History.GetExitPermitDatatablesHistory(ExitPermitNo, UseId, DateFrom, DateTo);
                recordsTotal = _Permit.Count();
                var data = _Permit.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetAttendanceDatatable(string DateFrom, string DateTo)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_visitorid");
                _VisitorId= cookie2.Value.ToString();

                string VisitorId1 = _VisitorId;
                var _Attendance = _History.GetAttendanceDatatableHistory(VisitorId1, DateFrom, DateTo);
                recordsTotal = _Attendance.Count();
                var data = _Attendance.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetDeliveryOrderDatatable(string dateFrom, string dateTo, string KeyWord)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_useid");
                _UseId = cookie2.Value.ToString();

                string UseId = _UseId;
                var _Attendance = _History.GetDeliveryOrderDatatable( dateFrom,  dateTo,  KeyWord,  UseId);
                recordsTotal = _Attendance.Count();
                var data = _Attendance.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}