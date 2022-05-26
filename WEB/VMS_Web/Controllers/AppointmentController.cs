using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{

    public class AppointmentController : SController
    {
        // GET: Appointment
        VisitorAction _visitorAction = new VisitorAction();
        VisitLogAction _visitLogAction = new VisitLogAction();

        #region Make Appointment
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult getListVisitor(string Name, string Company, string dateFrom, string dateTo, string Plant)
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

                string _plant = Session["UsePlant"].ToString(); //change to session username
                var _visitors = _visitorAction.GetListVisitorBeforeInvite(Name, Company, Plant, dateFrom, dateTo);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult getListVisitorInvite(string username)
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

                string _userName = username; //Modified by CANICE 20210407 //string _userName = Session["UseID"].ToString(); //change to session username
                var _visitors = _visitorAction.GetListVisitorAfterInvite(_userName);

                //total number of rows count   
                recordsTotal = _visitors.Count();
                //Paging   
                var data = _visitors.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult addVisitorInvite(string hostid, int id)
        {
            string _userName = hostid; //Modified by CANICE 20210407 //string _userName = Session["UseID"].ToString();
            var _data = _visitorAction.PostAddVisitorAppointment(id, _userName);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeVisitorInvite(string hostid, int id)
        {
            string _userName = hostid; //Modified by CANICE 20210407 //string _userName = Session["UseID"].ToString();
            var _data = _visitorAction.DeleteVisitorAppointment(id, _userName);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult saveAppointment(VisitLog VisitLog)
        {
            //VisitLog.HostId = Session["UseID"].ToString(); //Commented by CANICE 20210407 Value by user selection on screen
            VisitLog.CreUser = Session["UseID"].ToString();
            if(VisitLog.CCMail == null) { VisitLog.CCMail = ""; }
            var datas = _visitorAction.PostSaveAppointment(VisitLog, "InsertAppointment");
            if(datas.Success == true)
            {
                TempData["SUCCESS"] = datas.Message;
            }
            else
            {
                TempData["ERROR"] = datas.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult SaveNewVisitor(Visitor Visitor)
        {
            Visitor.UpdateBy = Session["UseID"].ToString();
            return Json(_visitorAction.PostAddEditVisitor(Visitor), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDateMinMax()
        {
            return Json(_visitorAction.GetDateMinMax(), JsonRequestBehavior.AllowGet);
        }
        public FileResult GetTemplateVisitor()
        {
            try
            {
                var table = _visitorAction.GetTemplateVisitor();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(table, "VisitorList");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "VisitorList_Template.xlsx");
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public JsonResult PostUploadVisitor(HttpPostedFileBase FilesCertDoc, string DateStart, string DateEnd, string HostId)
        {
            var SheetName = "VisitorList";
            var fileName = "Visitor_" + DateTime.Now.ToString("yyMMddHHmm") +"_"+ Cookies.GetUseID() + Path.GetExtension(FilesCertDoc.FileName);
            var physicalPath = Path.Combine(Class.Global.GetLocationPath("VISITORCONFIG", "VISITORPATH","SPL"), fileName);
            FilesCertDoc.SaveAs(physicalPath);
            var data = _visitorAction.PostUploadVisitor(physicalPath, SheetName, DateStart, DateEnd, HostId);
            if (!data.data.ToBoolean())
            {
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            return Json(data, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}