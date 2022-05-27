using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class ApprovalController : SController
    {
        ApprovalAction _Approval = new ApprovalAction();
        // GET: Approval
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        //public JsonResult ShowRequestApproval()
        //{
        //    try
        //    {
        //        var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //        var start = Request.Form.GetValues("start").FirstOrDefault();
        //        var length = Request.Form.GetValues("length").FirstOrDefault();
        //        var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
        //        var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
        //        var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
        //        //Creating instance of DatabaseContext class

        //        //Paging Size (10,20,50,100)  
        //        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //        int skip = start != null ? Convert.ToInt32(start) : 0;
        //        int recordsTotal = 0;
        //        var _visitors = _Approval.ShowRequestApproval();

        //        //total number of rows count   
        //        recordsTotal = _visitors.Count();
        //        //Paging   
        //        var data = _visitors.Skip(skip).Take(pageSize).ToList();

        //        return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}
        public JsonResult ShowRequestApproval()
        {
            try
            {
                List<LogHistory> _ApprovalLevel = new List<LogHistory>();
                _ApprovalLevel = _Approval.ShowRequestApproval();
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
        public JsonResult ShowHistoryApproval()
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

                var _username = Session["UseID"].ToString(); //change to session username
                var _visitors = _Approval.ShowHistoryApproval(_username);

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
        public JsonResult CheckDelegate()
        {
            var _userName = Session["UseID"].ToString();
            return Json(_Approval.CheckDelegate(_userName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangeDelegate(string isDelegate)
        {
            var _userName = Session["UseID"].ToString();
            return Json(_Approval.ChangeDelegate(isDelegate,_userName), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckAuthorize()
        {
            var ApprovalGroup = Session["ApprovalGroup"].ToString();
            var isDelegate = Session["Uselev"].ToString();
            return Json(_Approval.CheckAuthorize(ApprovalGroup, isDelegate), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApprovalAction(string LogId, string Status, string Remark="")
        {
            var res = _Approval.ApproveAction(Session["UseID"].ToString(), LogId, Status, Remark);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DetailVisitor(string LogId)
        {
            try
            {
                List<LogHistory> _ApprovalLevel = new List<LogHistory>();
                _ApprovalLevel = _Approval.DetailVisitor(LogId);
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
        public JsonResult ShowApprovalStatus(string LogId)
        {
            try
            {
                List<ApprovalStatus> _ApprovalLevel = new List<ApprovalStatus>();
                _ApprovalLevel = _Approval.ShowApprovalStatus(LogId);
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
        public JsonResult ShowMyRequest()
        {
            try
            {
                List<LogHistory> _ApprovalLevel = new List<LogHistory>();
                _ApprovalLevel = _Approval.ShowMyRequest();
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
        public JsonResult ShowHistoryMyTask()
        {
            try
            {
                List<LogHistory> _ApprovalLevel = new List<LogHistory>();
                _ApprovalLevel = _Approval.ShowHistoryMyTask();
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
    }
}