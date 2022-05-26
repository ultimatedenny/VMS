using System;
using System.Linq;
using System.Web.Mvc;
using VMS.Web.Models;
using VMS.Library;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ClosedXML.Excel;
using System.IO;
using System.Data;

namespace VMS.Web.Controllers
{

    public class UserController : SController
    {
        // GET: User
        WindowsAuth winAuth = new WindowsAuth();
        UserAction userAct = new UserAction();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserList()
        {
            return View();
        }
        public JsonResult GetUsersList([DataSourceRequest] DataSourceRequest request, string Plant)
        {
            return Json(userAct.GetUsersList(Plant).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserDetail(string UseID)
        {
            return Json(userAct.GetUserDetail(UseID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBusfuncsDL()
        {
            return Json(userAct.GetBusfunc(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApprovalGroupsDL()
        {
            return Json(userAct.GetAprrovalGroupsDL(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostUsers(User NewData, User OldData)
        {
            return Json(userAct.PostNewUser(NewData, OldData), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostChangeUserStatus(string UseID)
        {
            return Json(userAct.PostChangeUserStatus(UseID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDetailDelegation(string UserID)
        {
            try
            {
                VMSRes<User> _VisitorType = new VMSRes<User>();
                _VisitorType = userAct.GetDetailDelegation(UserID);
                var _retData =
                   new
                   {
                       xdataTable = _VisitorType.data
                   };
                return Json(_retData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult UserAccessRight()
        {
            return View();
        }

        public JsonResult UpdateUserAccess()
        {
            return Json("");
        }

        public JsonResult ShowListofAccess(string BusFunc)
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
                var _menuList = userAct.GetMenuListEdit(BusFunc);

                //total number of rows count   
                recordsTotal = _menuList.Count();
                //Paging   
                var data = _menuList.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public FileResult GetComparDiffDept()
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dt = new DataTable("CompareDepartment");
                dt = userAct.GetComparDiffDept().data;
                wb.Worksheets.Add(dt, "CompareDepartment");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_CompareDeparment" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                }
            }
        }
        public JsonResult GetUserFromAD(string Plant = "2300")
        {

            return Json(new VMSRes<string>
            {
                Success = true,
                Message = "Total Insert: " + userAct.GetUserFromAD(Plant).data + " Person(s)"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCheckUserGroup()
        {
            var UserDept = Session["UseDep"].ToString();
            return Json(userAct.GetCheckUserGroup(UserDept), JsonRequestBehavior.AllowGet);
        }
        public ActionResult SelfRegister()
        {
            return View();
        }

        public JsonResult RemoveDelegation()
        {
            var UseID = Session["UseID"].ToString();
            return Json(userAct.RemoveDelegation(UseID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveDelegation2(string UseID)
        {
            return Json(userAct.RemoveDelegation(UseID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult StopDelegation()
        {
            var UseID = Session["UseID"].ToString();
            return Json(userAct.StopDelegation(UseID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelegation()
        {
            var UseID = Session["UseID"].ToString();
            return Json(userAct.CheckDelegation(UseID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDelegation(string UseID, string DelegateUseID, string DelegateEffectiveFrom, string DelegateEffectiveTo, string DelegationRemarks)
        {
            if (UseID == DelegateUseID)
            {
                return Json(new VMSRes<string>
                {
                    Success = false,
                    Message = "You cant delegate to your own ID."
                }, JsonRequestBehavior.AllowGet);
            }
           if (DelegateEffectiveFrom == "")
            {
                DelegateEffectiveFrom = null;
            }
            if (DelegateEffectiveTo == "")
            {
                DelegateEffectiveTo = null;
            }
            var datas = userAct.SaveDelegation(UseID, DelegateUseID, DelegateEffectiveFrom, DelegateEffectiveTo, DelegationRemarks);
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
    }
}