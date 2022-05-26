using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VMS.Library
{
    public abstract class SController : Controller
    {
        public string ConnStr = ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        //public string appname = ConfigurationManager.AppSettings["ApplicationName"].ToString();
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");
            using (var sql = new MSSQL())
            {
                sql.SaveError(new auSystemLog
                {
                    SystemMessage = filterContext.Exception.ToString().Substring(0,
                filterContext.Exception.ToString().Length > 2000 ? 2000 : filterContext.Exception.ToString().Length),
                    ActionName = Request.RequestContext.RouteData.Values["controller"].ToString()
                    + "_"
                    + Request.RequestContext.RouteData.Values["action"].ToString(),
                }, false);
            }
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {

                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        Error = true,
                        msg = filterContext.Exception.ToString()
                    }
                };
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
            }
            else
            {
                filterContext.Result = new ViewResult()
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                    ViewData = new ViewDataDictionary(model),

                };
                Cookies.PostCookiesWithoutEnc("ERRORGLOBAL", model.Exception.Message);
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Sessions.GetUseID() == null)
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {

                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            Error = true,
                            msg = "SESSION TIMEOUT"
                        }
                    };
                    filterContext.HttpContext.Response.StatusCode = 500;
                }
                else
                {
                    filterContext.Result = new ViewResult()
                    {
                        ViewName = "~/Views/Home/Index.cshtml",
                    };
                }
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }
        }
    }
}
