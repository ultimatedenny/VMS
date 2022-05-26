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

    public class DashboardController : SController
    {
        VisitLogAction _visitLog = new VisitLogAction();
        DashboardAction _dash = new DashboardAction();
        // GET: Dashboard
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        #region New Dashboard
        public JsonResult GetListVisitorToday([DataSourceRequest] DataSourceRequest request, string Name, string Status = "", string Host = "", string Plant="")
        {
            return Json(_dash.GetListVisitorToday(Name, Plant, Status, Host).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public FileResult GetUrgentlyFileReport()
        {
            var data = _visitLog.GetFileUrgentReport(Sessions.GetUsePlant());
            using (XLWorkbook wb = new XLWorkbook())
            {
                if (data.Count > 0)
                {
                    List<string> Depts = data.Select(m => m.HostDept).Distinct().ToList();
                    foreach (var dep in Depts)
                    {
                        DataTable dt = new DataTable(dep);
                        dt = SysUtil.ConvertToDataTable((from rw in data
                                                         where rw.HostDept == dep
                                                         select rw).ToList());
                        wb.Worksheets.Add(dt, dep);
                    }
                }
                else
                {
                    wb.Worksheets.Add("none");
                }


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_urgentlyReport" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                }
            }
        }

        #endregion
        public JsonResult GetVisitorPieChartMonth()
        {
            return Json(_dash.GetVisitorPieChartMonth(), JsonRequestBehavior.AllowGet);
        }
        // TODO :
        public JsonResult GetDashboardRank()
        {
            return Json(_dash.GetDashboardRank(), JsonRequestBehavior.AllowGet);
        }
        #region Old Dashboard
        public JsonResult ShowListToday(string Name, string Status = "", string Host = "")
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
                var _visitors = _visitLog.ShowVisitorToday(Name, Session["UsePlant"].ToString(), Status, Host);

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
        public JsonResult GetSummaryVisitorPerDay(string month, string Day)
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
                var _visitors = _visitLog.GetSummaryVisitorPerDay(Session["UsePlant"].ToString(), month, Day);

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
        public JsonResult GetSummaryVisitorPerYear()
        {
            var Plant = Session["UsePlant"].ToString();
            var _data = _visitLog.GetSummaryVisitorPerYear(Plant);
            var _retData = ConvertDataToHighChartStack(_data);
            return Json(_retData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSummaryVisitorPerMonth(string month)
        {
            var Plant = Session["UsePlant"].ToString();
            var _data = _visitLog.GetSummaryVisitorPerMonth(Plant, month);
            var _retData = ConvertDataToHighChartStack(_data);
            return Json(_retData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVisitorSummaryPerMonthDept()
        {
            var UsePlant = Session["UsePlant"].ToString();
            var _data = _visitLog.GetVisitorSummaryPerMonthDept(UsePlant);
            List<PieChart> datas = new List<PieChart>();
            foreach (var x in _data)
            {
                PieChart data = new PieChart()
                {
                    name = x.ParentColumn,
                    y = x.Summary,
                    drilldown = x.ParentColumn,
                };
                datas.Add(data);
            }
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShowDatabyHost()
        {
            return Json(_visitLog.ShowVisitorByHost(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShowDataByVisitor()
        {
            return Json(_visitLog.ShowVisitorOneMonth(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHistoryVisitorById(string LogId)
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
                var _visitors = _visitLog.GetHistoryVisitorById(LogId);

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
        public dynamic ConvertDataToHighChartStack(List<BarChart> _data)
        {
            var categories = _data.Select(p => p.ParentColumn.Replace(" ", "")).Distinct().ToList();
            var _name = _data.Select(p => p.SubColumn).Distinct().ToList();

            List<StockBarChart> datas = new List<StockBarChart>();
            foreach (var name in _name)
            {
                datas.Add(new StockBarChart { name = name, data = _data.Where(p => p.SubColumn == name).Select(p => p.Summary).ToList() });
            }
            var _retData =
            new
            {
                DataX = categories,
                DataY = _name,
                data = datas
            };
            return _retData;
        }
        #endregion


        

        

    }
}