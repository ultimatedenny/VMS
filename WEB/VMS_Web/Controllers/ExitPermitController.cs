using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{

    public class ExitPermitController : Controller
    {
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        string _UseId;
        //ExitPermit1 _exitpermit = new ExitPermit1();
        ExitPermit _epAction = new ExitPermit();
        Notifications _Nofif = new Notifications();
        private DBVisitorMSEntities db = new DBVisitorMSEntities();
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Update(EPViewModel model)
        {
            if (model.SENo > 0)
            {
                EPMaster or = db.EPMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.MasterId = model.MasterId;
                or.SENo = model.SENo;
                or.EPNo = model.EPNo;
                or.UseDep = model.UseDep;
                or.PlantID = model.PlantID;
                or.Destination = model.Destination;
                or.Date = model.Date;
                or.Out = model.Out;
                or.ActOut = model.ActOut;
                or.In = model.In;
                or.ActIn = model.ActIn;
                or.CompTrans = model.CompTrans;
                or.CompTransTime = model.CompTransTime;
                or.Status = model.Status;
                or.UpdateBy = Session["UseID"].ToString();
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateMobile(EPViewModel model)
        {
            HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_manager");
            _UseId = cookie2.Value.ToString();
            if (model.SENo > 0)
            {
                EPMaster or = db.EPMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.MasterId = model.MasterId;
                or.SENo = model.SENo;
                or.EPNo = model.EPNo;
                or.UseDep = model.UseDep;
                or.PlantID = model.PlantID;
                or.Destination = model.Destination;
                or.Date = model.Date;
                or.Out = model.Out;
                or.ActOut = model.ActOut;
                or.In = model.In;
                or.ActIn = model.ActIn;
                or.CompTrans = model.CompTrans;
                or.CompTransTime = model.CompTransTime;
                or.Status = model.Status;
                or.UpdateBy = _UseId;
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        public ActionResult Mobile(string UseId)
        {
            HttpCookie cookie = new HttpCookie("cookie_useid", UseId);
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
            return View();
        }
        public ActionResult MobileConfirm(Guid MasterId, string token, string UseId)
        {
            EPMaster or = db.EPMasters.SingleOrDefault(a => a.MasterId == MasterId);

            HttpCookie cookie = new HttpCookie("cookie_manager", UseId);
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);

            ViewBag.Token = token;

            return View("MobileConfirm", or);
        }
        public ActionResult ThankYou()
        {
            return View();
        }
        public ActionResult SaveOrder(string UseDep, string PlantID, string Destination, string Out, string In,string ActOut, string ActIn, string Date, string CompTrans, string CompTransTime, string Status, EPDetail[] EPDetailss)
        {
            string result = "Error!";
            if (UseDep != null && PlantID != null && Destination != null && EPDetailss != null)
            {
                var query = "Declare @MaxNo varchar(50), @UniqNo varchar(10) select @MaxNo = isnull(MAX(convert(bigint, RIGHT(SENo, 3))), 0) + 1 from EPMaster where CONVERT(date, CreatedDate) = convert(date, getdate()) select @UniqNo = right(convert(varchar, 1000 + @MaxNo), 3) SELECT @UniqNo";
                var query_ = db.Database.SqlQuery<string>(query).First();
                var masterId = Guid.NewGuid();
                string EPNo_ = "EP" + DateTime.Now.ToString("yyMMdd") + "-" + query_.ToString();

                EPMaster model = new EPMaster();
                model.MasterId = masterId;
                model.SENo = Convert.ToInt32(query_);
                model.EPNo = EPNo_;
                model.UseDep = UseDep;
                model.PlantID = PlantID;
                model.Destination = Destination;
                model.Date = Convert.ToDateTime(Date);
                model.Out = TimeSpan.Parse(Out);
                model.ActOut = TimeSpan.Parse(ActOut);
                model.In = TimeSpan.Parse(In);
                model.ActIn = TimeSpan.Parse(ActIn);
                model.CompTrans = Convert.ToBoolean(CompTrans);
                model.CompTransTime = TimeSpan.Parse(CompTransTime);
                model.Status = Status;
                model.CreatedBy = Session["UseID"].ToString();
                model.CreatedDate = DateTime.Now;
                model.UpdateBy = Session["UseID"].ToString();
                model.UpdateDate = DateTime.Now;
                db.EPMasters.Add(model);
                
                foreach (var item in EPDetailss)
                {
                    var detailId = Guid.NewGuid();
                    var EPDetails = new EPDetail();
                    EPDetail O = new EPDetail();
                    O.DetailId = detailId;
                    O.MasterId = masterId;
                    O.UseID = item.UseID;
                    db.EPDetails.Add(O);
                }
                db.SaveChanges();

                result = "Saved Success !";
                if (result == "Saved Success !")
                {
                    var _User = _Nofif.GetUserApprovalEP(masterId, EPNo_);
                    //string queryMAIL = $@"EMAIL_EXITPERMIT '{masterId}'";
                    //using (var sql = new MSSQL())
                    //{
                    //    num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                    //}
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveOrderMobile(string UseDep, string PlantID, string Destination, string Out, string In, string ActOut, string ActIn, string Date, string CompTrans, string CompTransTime, string Status, EPDetail[] EPDetailss)
        {
            HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_useid");
            _UseId = cookie2.Value.ToString();
            string result = "Error!";
            if (UseDep != null && PlantID != null && Destination != null && EPDetailss != null)
            {
                var query = "Declare @MaxNo varchar(50), @UniqNo varchar(10) select @MaxNo = isnull(MAX(convert(bigint, RIGHT(SENo, 3))), 0) + 1 from EPMaster where CONVERT(date, CreatedDate) = convert(date, getdate()) select @UniqNo = right(convert(varchar, 1000 + @MaxNo), 3) SELECT @UniqNo";
                var query_ = db.Database.SqlQuery<string>(query).First();
                var masterId = Guid.NewGuid();
                string EPNo_ = "EP" + DateTime.Now.ToString("yyMMdd") + "-" + query_.ToString();

                EPMaster model = new EPMaster();
                model.MasterId = masterId;
                model.SENo = Convert.ToInt32(query_);
                model.EPNo = EPNo_;
                model.UseDep = UseDep;
                model.PlantID = PlantID;
                model.Destination = Destination;
                model.Date = Convert.ToDateTime(Date);
                model.Out = TimeSpan.Parse(Out);
                model.ActOut = TimeSpan.Parse(ActOut);
                model.In = TimeSpan.Parse(In);
                model.ActIn = TimeSpan.Parse(ActIn);
                model.CompTrans = Convert.ToBoolean(CompTrans);
                model.CompTransTime = TimeSpan.Parse(CompTransTime);
                model.Status = Status;
                model.CreatedBy = _UseId;
                model.CreatedDate = DateTime.Now;
                model.UpdateBy = _UseId;
                model.UpdateDate = DateTime.Now;
                db.EPMasters.Add(model);

                foreach (var item in EPDetailss)
                {
                    var detailId = Guid.NewGuid();
                    var EPDetails = new EPDetail();
                    EPDetail O = new EPDetail();
                    O.DetailId = detailId;
                    O.MasterId = masterId;
                    O.UseID = item.UseID;
                    db.EPDetails.Add(O);
                }
                db.SaveChanges();

                result = "Saved Success !";
                if (result == "Saved Success !")
                {
                    var _User = _Nofif.GetUserApprovalEP(masterId, EPNo_);
                    //string queryMAIL = $@"EMAIL_EXITPERMIT '{masterId}'";
                    //using (var sql = new MSSQL())
                    //{
                    //    num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                    //}
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditMaster(int SENo)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-ADMIN" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
            {
                EPViewModel model = new EPViewModel();
                EPMaster or = db.EPMasters.SingleOrDefault(c => c.SENo == SENo);
                model.MasterId = or.MasterId;
                model.SENo = or.SENo;
                model.EPNo = or.EPNo;
                model.UseDep = or.UseDep;
                model.PlantID = or.PlantID;
                model.Destination = or.Destination;
                model.Date = or.Date;
                model.Out = or.Out;
                model.ActOut = (TimeSpan)or.ActOut;
                model.In = or.In;
                model.ActIn = (TimeSpan)or.ActIn;
                model.CompTrans = or.CompTrans;
                model.CompTransTime = or.CompTransTime;
                model.Status = or.Status;
                return PartialView("Partial1", model);
            }
            else
            {
                return PartialView("Error");
            }
        }
        public ActionResult EditTime(int SENo)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-SECURITY" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN" )
            {
                EPViewModel model = new EPViewModel();
                EPMaster or = db.EPMasters.SingleOrDefault(c => c.SENo == SENo);
                model.MasterId = or.MasterId;
                model.SENo = or.SENo;
                model.EPNo = or.EPNo;
                model.UseDep = or.UseDep;
                model.PlantID = or.PlantID;
                model.Destination = or.Destination;
                model.Date = or.Date;
                model.Out = or.Out;
                model.ActOut = (TimeSpan)or.ActOut;
                model.In = or.In;
                model.ActIn = (TimeSpan)or.ActIn;
                model.CompTrans = or.CompTrans;
                model.CompTransTime = or.CompTransTime;
                model.Status = or.Status;
                return PartialView("Partial2", model);
            }
            else
            {
                return PartialView("Error");
            }
        }

        public JsonResult GetExitPermitDatatables(string ExitPermitNo, string DateFrom, string DateTo)
        {
            try
            {
                //Creating instance of DatabaseContext class
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (Session["BusFunc"].ToString() == "SYSTEM-SECURITY")
                {
                    var _Permit = _epAction.GetExitPermitDatatablesSecurity(ExitPermitNo, DateFrom, DateTo);
                    recordsTotal = _Permit.Count();
                    var data = _Permit.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else
                {
                    var _Permit = _epAction.GetExitPermitDatatables(ExitPermitNo, DateFrom, DateTo);
                    recordsTotal = _Permit.Count();
                    var data = _Permit.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetExitPermitDatatables2(string ExitPermitNo, string DateFrom, string DateTo)
        {
            try
            {
                //Creating instance of DatabaseContext class
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var _Permit = _epAction.GetExitPermitDatatables2(ExitPermitNo, DateFrom, DateTo);

                //total number of rows count   
                recordsTotal = _Permit.Count();
                //Paging   
                var data = _Permit.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public FileResult ExportExitPermit(string start2, string end2, string EPNo)
        {
            DataTable table = new DataTable("ExitPermitReport");
            table = _epAction.ExportExitPermit(start2, end2, EPNo);
            table.TableName = "ExitPermitReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_EPLog" + start2 + "_" + end2 + ".xlsx");
                }
            }
        }
        
    }
}