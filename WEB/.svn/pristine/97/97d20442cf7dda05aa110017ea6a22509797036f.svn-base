using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Class;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{

    public class VisitorController : SController
    {
        VisitorAction _visitorAct = new VisitorAction();
        UserAction _userAct = new UserAction();
        MasterDataAction _mdAction = new MasterDataAction();
        // GET: Visitor
        [AllowAnonymous]
        public ActionResult NewVisitor()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewVisitor(Visitor Visitor)
        {
            Visitor.UpdateBy = Sessions.GetUseID() ?? "Update By self";
            var _datas = _visitorAct.PostAddEditVisitor(Visitor);
            TempData["SUCCESS"] = _datas.Success;
            TempData["MESSAGE"] = _datas.Message;
            return RedirectToAction("NewVisitor");
        }
        public ActionResult SaveVisitor(Visitor Visitor)
        {
            Visitor.UpdateBy = Sessions.GetUseID() ?? "Update By self";
            var _datas = _visitorAct.PostAddEditVisitor(Visitor);
            return Json(_datas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckVisitorName(string Name, string Company, string JobDesc)
        {
            return Json(_visitorAct.GetCheckVisitorName(Name, Company, JobDesc), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PopulateCompanyVendor(string text)
        {
            return Json(_mdAction.PopulateVisitorCompany(text), JsonRequestBehavior.AllowGet);
        }
        public JsonResult PopulateVisitoryByCompany(string text, string company)
        {
            return Json(_mdAction.PopulateVisitoryByCompany(text, company), JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult VisitorActivity()
        {
            return View();
        }
        public JsonResult GetCheckMandatory()
        {
            try
            {
                return Json(new VMSRes<dynamic>
                {
                    Success = true,
                    data = new
                {
                    Card = Global.GetVisitorConfigMandatory("CARDMANDATORY"),
                    Photo = Global.GetVisitorConfigMandatory("PHOTOMANDATORY")
                }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

            
        }
        public JsonResult GetsearchVisitorbyBadge(string CardId)
        {
            var session = Sessions.GetUsePlant() ?? @ConfigurationManager.AppSettings["PlantCode"];
            var _visitors = _visitorAct.GetVisitorInArea(CardId, session, "ShimanoBadge");
            return Json(_visitors, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetsearchVisitorbyVisitorID(string VisitorID)
        {
            var session = Sessions.GetUsePlant() ?? @ConfigurationManager.AppSettings["PlantCode"];
            var _visitors = _visitorAct.GetVisitorInArea(VisitorID, session, "VisitorId");
            return Json(_visitors, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetsearchByIDCard(string CardId)
        {
            var _Plant = Sessions.GetUsePlant() ?? @ConfigurationManager.AppSettings["PlantCode"];
            var _visitors = _visitorAct.GetVisitorByPassCard(CardId, "VisitorCardNo");
            var _host = _visitorAct.GetHostAppointment(CardId, _Plant, "VisitorCardNo");
            var _datas = new
            {
                visitor = _visitors,
                host = _host
            };
            return Json(_datas);
        }
        public ActionResult GetsearchByIDVisitor(string VisitorID)
        {
            var _Plant = Sessions.GetUsePlant() ?? @ConfigurationManager.AppSettings["PlantCode"];
            var _visitors = _visitorAct.GetVisitorByPassCard(VisitorID, "Id");
            var _host = _visitorAct.GetHostAppointment(VisitorID, _Plant, "Id");
            var _datas = new
            {
                visitor = _visitors,
                host = _host
            };
            return Json(_datas);
        }
        public JsonResult GetVisitorsForAppointment([DataSourceRequest] DataSourceRequest request, string Name)
        {
            return Json(_visitorAct.GetVisitorsForAppointment(Name).data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PostSaveAppointment(VisitLog VisitLog, string Method="ADD")
        {

            VisitLog.CreUser = Sessions.GetUseID();
            VisitLog.DateStart = DateTime.Now;
            VisitLog.DateEnd = DateTime.Now;
            VisitLog.Area = ConfigurationManager.AppSettings["OthersRoomCode"].ToString();
            VisitLog.Plant = Sessions.GetUsePlant();
            VisitLog.TimeStart = DateTime.Now.TimeOfDay;
            VisitLog.TimeEnd = DateTime.Now.AddHours(4).TimeOfDay;
            var datas = new VMSRes<string>();
            if (Method.ToUpper() == "ADD")
            {
                datas = _visitorAct.PostSaveAppointment(VisitLog, "InsertAppointmentBySecurity");
            }
            
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PostSaveCardRegister(string Id, string CardId)
        {
            var _datas = _visitorAct.GetVisitorByPassCard(CardId);
            if (_datas.Success)
            {
                _datas.Message = "Cannot Register this card! Name who has this card is :" + _datas.data;
                return Json(_datas, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _updated = _visitorAct.PostUpdateCardRegister(Convert.ToInt32(Id), CardId);
                return Json(_updated, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetHostDetail()
        {
            return Json(_userAct.GetUsersForAppointment(), JsonRequestBehavior.AllowGet);
        }
        //add by suharjo ng 2021-11-15
        public JsonResult GetDelegateTo()
        {
            return Json(_userAct.GetDelegateTo(), JsonRequestBehavior.AllowGet);
        }
        //end add
        public JsonResult GetHostDepartment(string Plant)
        {
            return Json(_userAct.GetUserDeptForAppointment(Plant), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAreaList(string plant, string visitType)
        {
            return Json(_mdAction.GetAreaforDDList(plant, visitType), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Capture(int id)
        {
            var stream = Request.InputStream;
            string dump;
            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();
                DateTime nm = DateTime.Now;
                string name = nm.ToString("yy_MM_dd_hh_mm") + "_" + id.ToString() + ".jpg";
                var path = Server.MapPath("~/Uploads/PhotoVisitor/") + name;
                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
                Session["PhotoFile"] = name;
            }
            return Json("Index");
        }
        /////////
        /////////
        /////////
        [HttpPost]
        public JsonResult Capture2(int id, HttpPostedFileBase webcamPhoto)
        {
            if (webcamPhoto != null && webcamPhoto.ContentLength > 0)
            {
                try
                {
                    DateTime nm = DateTime.Now;
                    string name = nm.ToString("yy_MM_dd_hh_mm") + "_" + id.ToString() + ".jpg";
                    var filePath = Path.Combine(Server.MapPath("~/Uploads/PhotoVisitor/"), name);
                    webcamPhoto.SaveAs(filePath);
                    Session["PhotoFile"] = name;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return Json("Index");
        }
        /////////
        /////////
        /////////
        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }
        public JsonResult SavePhotoVisitor(string id, string src)
        {
            return Json(_visitorAct.PostPhotoVisitor(Convert.ToInt16(id), src), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Rebind()
        {
            string path = Session["PhotoFile"].ToString();
            return Json(new VMSRes<string>
            {
                Success=true,
                Message="true",
                data=path
            }
            , JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveToCheckin(string Id, string LogId, string CardId, string status = "1", string Remark = "")
        {
            var _datas = _visitorAct.PostUpdateStatus(Id, LogId, CardId, "VisitorCheckIn", Sessions.GetUseID(), status, Remark);
            TempData["SUCCESS"] = _datas.Success;
            TempData["MESSAGE"] = _datas.Message;
            return Json(_datas);
        }
        public ActionResult VisitorActionChgStatus(string Id, string LogId, string CardId, string method)
        {
            var _datas = _visitorAct.PostUpdateStatus(Id, LogId, CardId, "VisitorCheckIn", Session["UseID"].ToString(), method);
            TempData["SUCCESS"] = _datas.Success;
            TempData["MESSAGE"] = _datas.Message;
            return Json(_datas);
        }
        public JsonResult GetVisitorDetail(string Id)
        {
            return Json(_visitorAct.GetVisitorDetail(Id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult List()
        {
            return View();
        }
        
        //
        

    }
}