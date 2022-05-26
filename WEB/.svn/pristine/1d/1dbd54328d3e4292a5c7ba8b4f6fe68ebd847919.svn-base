using System;
using System.Linq;
using System.Web.Mvc;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class ReceiptionistController : Controller
    {
        UserAction _userAct = new UserAction();

        // GET: Receiptionist
        public ActionResult Index()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult DelegateApproval()
        {
            return View();
        }
        [ShimanoCustom]
        public ActionResult DelegationLog()
        {
            return View();
        }
        public JsonResult GetListApprover()
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
                var _visitors = _userAct.GetListApprover();
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
        public JsonResult ChangeDelegateByRec(string UseID)
        {
            return Json(_userAct.ChangeDelegateByRec(UseID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult StopDelegation(string UseID)
        {
            return Json(_userAct.StopDelegation(UseID), JsonRequestBehavior.AllowGet);
        }
    }
}