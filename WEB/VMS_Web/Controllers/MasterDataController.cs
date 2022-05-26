using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Web.Attribute;
using VMS.Web.Models;
using ClosedXML.Excel;
using System.IO;
using VMS.Library;
using System.Configuration;

namespace VMS.Web.Controllers
{
    
    public class MasterDataController : Controller
    {
        MasterDataAction _mdAction = new MasterDataAction();
        VisitorAction _visitorAction = new VisitorAction();

        // GET: MasterData
        [ShimanoCustom]
        public ActionResult Area()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult VisitorType()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult Visitor()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult ApprovalLevel(string VisitorType)
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult AutoApproval()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult Department()
        {
            return View();
        }

        //DISCLAIMER
        [ShimanoCustom]
        public ActionResult Disclaimer()
        {
            return View();
        }
        //[ShimanoCustom]
        public ActionResult DisclaimerArea()
        {
            return View();
        }

        public JsonResult getAreaList(string plant, string visitType)
        {
            var _visitorArea = _mdAction.GetAreaforDDList(plant, visitType);
            return Json(_visitorArea, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveArea(AreaVisitor AreaVisitor, string OldAreaId, string OldVisitorType)
        {
            AreaVisitor.UpdateBy = Session["UseID"].ToString();
            return Json(_mdAction.SaveArea(AreaVisitor, OldAreaId, OldVisitorType));
        }
        //public JsonResult SaveVisitorType(VisitType VisitType)
        //{
        //    VisitType.UpdateBy = Session["UseID"].ToString();
        //    return Json(_mdAction.SaveVisitorType(VisitType), JsonRequestBehavior.AllowGet);
        //}
        //add by suharjo ng 2021-11-15
        public JsonResult SaveVisitorType(string ID, string VisitorType, string NeedApprove, string NeedMailAlert, string AllowUnknown, string IsView)
        {
            var UseID = Session["UseID"].ToString();
            
            var datas = _mdAction.SaveVisitorType(ID, VisitorType, NeedApprove, NeedMailAlert, AllowUnknown, IsView, UseID);
            if (datas.Success == true)
            {
                return Json(new VMSRes<string>
                {
                    Success = true,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new VMSRes<string>
                {
                    Success = false,
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult SaveDept(string Plant, string Department, string DepartmentName, string Action)
        {
            string UseID = Session["UseID"].ToString();
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");
            try
            {
                _Data = _mdAction.SaveDept(Plant, Department, DepartmentName, UseID, Action);
                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        //end add
        public JsonResult SaveAutoApproval(mmAutoApproval AutoApproval)
        {
            AutoApproval.ChangeUser = Session["UseID"].ToString();
            return Json(_mdAction.SaveAutoApproval(AutoApproval), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEditVisitor(string LogId, string id, string FullName, string Company, string JobDesc, string Email, string Phone, string VehicleNo)
        {
            string UpdateBy = Session["UseID"].ToString();
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");
            try
            {

                _Data = _mdAction.SaveEditVisitor(LogId, id, FullName, Company, JobDesc, Email, Phone, VehicleNo, UpdateBy);

                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAutoApprovalByID(mmAutoApproval AutoApproval)
        {
            AutoApproval.ChangeUser = Session["UseID"].ToString();
            return Json(_mdAction.DeleteAutoApprovalByID(AutoApproval), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDept(string Plant, string Dept)
        {
            return Json(_mdAction.DeleteDept(Plant, Dept), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPlantList()
        {
            return Json(_mdAction.GetPlantforDDList(""), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPlantMDM()
        {
            return Json(_mdAction.GetPlantMDMList(""), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getVisitTypeList(string text = "")
        {
            return Json(_mdAction.GetVisitTypeforDDList(text), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getVisitTypeListPublicView(string text = "")
        {
            return Json(_mdAction.GetVisitTypeforDDListPublicView(text), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ChangeStatus(string TableName, string ColumnName, string Id)
        {
            return Json(_mdAction.ChangeStatusMasterData(TableName, ColumnName, Id, Session["UseID"].ToString()), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAreaDatatables(string AreaName)
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

                var _Areas = _mdAction.GetAreaDatatables(AreaName);

                //total number of rows count   
                recordsTotal = _Areas.Count();
                //Paging   
                var data = _Areas.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public JsonResult getmmApprovalLevel(string VisitorType, string Plant, string Department)
        //{
        //    return Json(_mdAction.GetmmApprovalLevel(VisitorType, Plant, Department), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetmmApprovalLevel(string VisitorType, string Plant, string Department)
        {
            try
            {
                List<mmApprovalLevel> _ApprovalLevel = new List<mmApprovalLevel>();
                _ApprovalLevel = _mdAction.GetmmApprovalLevel(VisitorType, Plant, Department);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet );
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetmmDepartment(string Plant)
        {
            try
            {
                List<mmDepartment> _ApprovalLevel = new List<mmDepartment>();
                _ApprovalLevel = _mdAction.GetmmDepartment(Plant);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult ViewApproverAppointment(string Plant, string Department, string VisitorType)
        {
            try
            {
                List<mmApprovalLevel> _ApprovalLevel = new List<mmApprovalLevel>();
                _ApprovalLevel = _mdAction.ViewApproverAppointment(Plant, Department, VisitorType);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult ViewApproverAppointmentRecord(string Plant, string Department, string VisitorType)
        {
            try
            {
                List<mmApprovalLevel> _ApprovalLevel = new List<mmApprovalLevel>();
                _ApprovalLevel = _mdAction.ViewApproverAppointmentRecord(Plant, Department, VisitorType);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetmmApprovalLevelConfirm()
        {
            try
            {
                List<TmpmmApprovalLevel> _ApprovalLevelConfirm = new List<TmpmmApprovalLevel>();
                _ApprovalLevelConfirm = _mdAction.GetmmApprovalLevelConfirm(Session["UseID"].ToString());
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevelConfirm
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetmmAutoApprovalConfirm()
        {
            try
            {
                List<TmpmmAutoApproval> _ApprovalLevelConfirm = new List<TmpmmAutoApproval>();
                _ApprovalLevelConfirm = _mdAction.GetmmAutoApprovalConfirm(Session["UseID"].ToString());
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevelConfirm
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetmmAutoApproval(string Plant, string Department)
        {
            try
            {
                List<mmAutoApproval> _AutoApproval = new List<mmAutoApproval>();
                _AutoApproval = _mdAction.GetmmAutoApproval(Plant, Department);
                var _retData =
                   new
                   {
                       xdataTable = _AutoApproval
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetmmDelegationLog(string Plant, string Department, string StartDate, string EndDate)
        {
            try
            {
                List<User> _AutoApproval = new List<User>();
                _AutoApproval = _mdAction.GetmmDelegationLog(Plant, Department, StartDate, EndDate);
                var _retData =
                   new
                   {
                       xdataTable = _AutoApproval
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetViewApproverLevel(string Plant, string Department, string VisitorType)
        {
            try
            {
                List<mmApprovalLevel> _ApprovalLevel = new List<mmApprovalLevel>();
                _ApprovalLevel = _mdAction.GetViewApproverLevel(Plant, Department, VisitorType);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult DetailApprovalLevel(string VisitorType, string Plant, string Department)
        {
            try
            {
                List<mmApprovalLevel> _ApprovalLevel = new List<mmApprovalLevel>();
                _ApprovalLevel = _mdAction.GetmmApprovalLevelDetail(VisitorType, Plant, Department);
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult DetailApprovalLevelConfirm(string VisitorType, string Plant, string Department)
        {
            try
            {
                List<TmpmmApprovalLevel> _ApprovalLevel = new List<TmpmmApprovalLevel>();
                _ApprovalLevel = _mdAction.GetmmApprovalLevelDetailConfirm(VisitorType, Plant, Department, Session["UseID"].ToString());
                var _retData =
                   new
                   {
                       xdataTable = _ApprovalLevel
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetAVisitorTypeDatatables()
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

                var _visitorType = _mdAction.GetVisitorTypeDatatables();

                //total number of rows count   
                recordsTotal = _visitorType.Count();
                //Paging   
                var data = _visitorType.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetVisitorTypeDetail(string Id)
        {
            return Json(_mdAction.GetVisitorTypeDetail(Id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDeptDetail(string Plant, string Dept)
        {
            return Json(_mdAction.GetDeptDetail(Plant, Dept), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAreaDetail(string AreaId, string VisitorType)
        {
            return Json(_mdAction.GetAreaDetail(AreaId, VisitorType), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserLevelCodLst()
        {
            return Json(_mdAction.GetCodLst("VisitorMSUseLev"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentforDDList(string Plant)
        {
            return Json(_mdAction.GetDepartmentforDDList(Plant), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentApprovalLevelDDList(string Plant)
        {
            return Json(_mdAction.GetDepartmentApprovalLevelDDList(Plant), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovalCategoryDDList()
        {
            return Json(_mdAction.GetApprovalCategoryDDList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWindowsIDDDList()
        {
            return Json(_mdAction.GetWindowsIDDDList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovalLevel(string Plant, string VisitorType, string Department, string Category, string WindowsID)
        {
            return Json(_mdAction.GetApprovalLevel(Plant, VisitorType, Department, Category, WindowsID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVisitorDatatables(string Name)
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

                var _Visitor = _visitorAction.GetAllVisitor(Name);

                //total number of rows count   
                recordsTotal = _Visitor.Count();
                //Paging   
                var data = _Visitor.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetVisitorMasterData([DataSourceRequest] DataSourceRequest request, string Name)
        {
            return Json(_visitorAction.GetVisitorMasterData(Name).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PostUploadUploadMasterData(UploadMasterData FilesCertDoc)
        {
            
            string ExtFile;
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");
            var xMasterData = FilesCertDoc.MasterData;
            var User = Session["UseID"].ToString();
            try
            {
                var fileName = "";
                var physicalPath = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    ExtFile = Path.GetExtension(file.FileName);
                    if(xMasterData =="AutoApproval")
                    {
                        fileName = "AutoApproval_" + DateTime.Now.ToString("yyMMddHHmm") + "_" + Session["UseID"].ToString() + Path.GetExtension(file.FileName);
                        physicalPath = Path.Combine(Class.Global.GetLocationPath("VISITORCONFIG", "VISITORPATH", "SBM"), fileName);
                    }
                    else if (xMasterData == "ApprovalLevel")
                    {
                        fileName = "ApprovalLevel_" + DateTime.Now.ToString("yyMMddHHmm") + "_" + Session["UseID"].ToString() + Path.GetExtension(file.FileName);
                        physicalPath = Path.Combine(Class.Global.GetLocationPath("VISITORCONFIG", "VISITORPATH", "SBM"), fileName);
                    }
                    
                    file.SaveAs(physicalPath);
                    _Data = _mdAction.MasterDataUpload(physicalPath, xMasterData, User);

                    j.Data.Add(_Data);
                    j.Result = true;
                    j.Msg = _Data[0].Message;
                }

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public JsonResult PostUploadApprovalLevel(UploadMasterData FilesCertDoc)
        //{

        //    string ExtFile;
        //    List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
        //    JRes j = new JRes(new List<object>(), false, "");
        //    var xMasterData = FilesCertDoc.MasterData;
        //    var User = Session["UseID"].ToString();
        //    try
        //    {
        //        HttpFileCollectionBase files = Request.Files;
        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            HttpPostedFileBase file = files[i];
        //            ExtFile = Path.GetExtension(file.FileName);

        //            var fileName = "ApprovalLevel_" + DateTime.Now.ToString("yyMMddHHmm") + "_" + Session["UseID"].ToString() + Path.GetExtension(file.FileName);
        //            var physicalPath = Path.Combine(Class.Global.GetLocationPath("VISITORCONFIG", "VISITORPATH", "SPL"), fileName);
        //            file.SaveAs(physicalPath);
        //            _Data = _mdAction.MasterDataUpload(physicalPath, xMasterData, User);

        //            j.Data.Add(_Data);
        //            j.Result = true;
        //            j.Msg = _Data[0].Message;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        j.Result = false;
        //        j.Msg = ex.Message;
        //    }
        //    return Json(j, JsonRequestBehavior.AllowGet);
        //}
        public FileResult Export(string Plant, string Department)
        {
            DataTable table = new DataTable("AutoApproval");
            table = _mdAction.GetListForExport(Plant, Department);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "mmAutoApproval_" + Plant + "_" + Department + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx");
                }
            }
        }
        public FileResult ExportFunction(string MasterData, string Plant, string Department)
        {
            DataTable table = new DataTable(MasterData);
            table = _mdAction.ExportFunction(MasterData, Plant, Department);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        MasterData+"_" + Plant + "_" + Department + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx");
                }
            }
        }
        public FileResult ExportApprovalLevel(string Plant, string Department, string VisitorType)
        {
            DataTable table = new DataTable("ApprovalLevel");
            table = _mdAction.GetListForExportApprovalLevel(Plant, Department, VisitorType);
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "mmApprovalLevel_" + Plant + "_" + Department + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx");
                }
            }
        }
        public JsonResult GetAllowUnknown(string VisitorType)
        {
            try
            {
                List<Visitor> _VisitorType = new List<Visitor>();
                _VisitorType = _mdAction.GetAllowUnknown(VisitorType);
                var _retData =
                   new
                   {
                       xdataTable = _VisitorType
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetLogo()
        {
            try
            {
                List<Visitor> _VisitorType = new List<Visitor>();
                _VisitorType = _mdAction.GetLogo();
                var _retData =
                   new
                   {
                       xdataTable = _VisitorType
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult GetResultNotOK()
        {
            try
            {
                List<Visitor> _VisitorType = new List<Visitor>();
                _VisitorType = _mdAction.GetResultNotOK(Session["UseID"].ToString());
                var _retData =
                   new
                   {
                       xdataTable = _VisitorType
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult CheckVisitorTypeName(string visitortype)
        {
            try
            {
                List<Visitor> _VisitorType = new List<Visitor>();
                _VisitorType = _mdAction.CheckVisitorTypeName(visitortype);
                var _retData =
                   new
                   {
                       xdataTable = _VisitorType
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult UploadAutoApprovalNext(string MasterData)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");
            
            var User = Session["UseID"].ToString();
            try
            {
                _Data = _mdAction.UploadAutoApprovalNext(User, MasterData);
                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteApprovalLevel(string Plant, string VisitorType, string Department)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");
            
            try
            {
                _Data = _mdAction.DeleteApprovalLevel(Plant, VisitorType, Department);
                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;

            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadModalMessage()
        {
            var useID = Session["UseID"].ToString();
            try
            {
                List<ModalMessage> _AutoApproval = new List<ModalMessage>();
                _AutoApproval = _mdAction.LoadModalMessage(useID);
                var _retData =
                   new
                   {
                       xdataTable = _AutoApproval
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //ADD HIDEF DISCLAIMER
        public class ItemDisclaimer
        {
            public string ID { get; set; }
            public string Plant { get; set; }
            public string DisclaimerCategory { get; set; }
            public string Type { get; set; }
            public string ValidFrom { get; set; }
            public string ValidTo { get; set; }
            public string DisclaimerDesc { get; set; }
            public HttpPostedFileBase File { get; set; }

        }

        public JsonResult GetDisclaimerDatatables(string Search)
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

                var _Disclaimer = _mdAction.GetDisclaimerDatatables(Search);

                //total number of rows count   
                recordsTotal = _Disclaimer.Count();
                //Paging   
                var data = _Disclaimer.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult getTypeDisclaimer()
        {
            return Json(_mdAction.GetTypeDisclaimer(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getArea(string Plant)
        {
            return Json(_mdAction.getArea(Plant), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getDisclaimerCategory(string Plant)
        {
            return Json(_mdAction.getDisclaimerCategory(Plant), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SaveDisclaimer(ItemDisclaimer Item)
        {
            string NameFile = "";
            string ExtFile = ".pdf";
            string PathDisclaimer = ConfigurationManager.AppSettings["StorePathDisclaimer"];
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                //Random rand = new Random();
                //int RandomText = rand.Next();
                var UseID = Session["UseID"].ToString();
                HttpFileCollectionBase files = Request.Files;
                //NameFile = "DISCLAIMER-" + Item.Plant + "-" + Item.DisclaimerCategory.Replace(" ","_")+"-"+ RandomText + ExtFile;

                for (int i = 0; i < files.Count; i++)
                {
                    Random rand = new Random();
                    int RandomText = rand.Next();
                    NameFile = "DISCLAIMER-" + Item.Plant + "-" + Item.DisclaimerCategory.Replace(" ", "_") + "-" + RandomText + ExtFile;
                }

                _Data = _mdAction.SaveDisclaimer(Item, NameFile, UseID);

                if (_Data[0].Status == "success")
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        file.SaveAs(Path.Combine(PathDisclaimer, NameFile));
                    }
                }

                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;
            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SaveDisclaimerOnly(ItemDisclaimer Item)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                var UseID = Session["UseID"].ToString();

                _Data = _mdAction.SaveDisclaimerOnly(Item, UseID);

                j.Data.Add(_Data);
                j.Result = true;
                j.Msg = _Data[0].Message;
            }
            catch (Exception ex)
            {
                j.Result = false;
                j.Msg = ex.Message;
            }
            return Json(j, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDisclaimer(string ID)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                _Data = _mdAction.DeleteDisclaimer(ID);
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

        public JsonResult GetDisclaimerAreaDatatables(string Search)
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

                var _Disclaimer = _mdAction.GetDisclaimerAreaDatatables(Search);

                //total number of rows count   
                recordsTotal = _Disclaimer.Count();
                //Paging   
                var data = _Disclaimer.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult SaveDisclaimerArea(string ID, string Plant, string Category, string Area)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                var UseID = Session["UseID"].ToString();
                _Data = _mdAction.SaveDisclaimerArea(ID, Plant, Category, Area, UseID);
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

        public JsonResult DeleteDisclaimerArea(string ID)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                _Data = _mdAction.DeleteDisclaimerArea(ID);
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
        public JsonResult ChangeStatusDisclaimer(string Plant, string Area, string Type, string Value)
        {
            List<ModelKnowledgeMsg> _Data = new List<ModelKnowledgeMsg>();
            JRes j = new JRes(new List<object>(), false, "");

            try
            {
                _Data = _mdAction.ChangeStatusDisclaimer(Plant, Area, Type, Value);
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