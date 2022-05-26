using System.Web.Mvc;
using System.Web;
using System.Data;
using VMS.Library;
using VMS.Library.Constants;

namespace VMS.Web.Attribute
{
    public class ShimanoCustomAttribute : ActionFilterAttribute
    {
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Sessions.GetUseID() == null)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Home/Index.cshtml"
                };

            }
            else
            {
                string BusFunc = Sessions.GetBusFunc();
                string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
                string query = $@"select MnuNam from menulist lst
 INNER JOIN levelmenu mnu on lst.mnucod = mnu.mnucod 
 where busfunc='{ BusFunc }' and frmnam='{controllerName}_{actionName} '";

                DataTable dt = new DataTable();
                using (var sql = new MSSQL())
                {
                    dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
                }

                if (dt.Rows.Count == 0)
                {
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Forbidden.cshtml"
                    };
                    
                }
            }
        }
    }

    //public class ShimanoSessionAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        base.OnActionExecuting(filterContext);
    //        if (HttpContext.Current.Session["UseID"] == null)
    //        {
    //            filterContext.Result = new ViewResult
    //            {
    //                ViewName = "~/Views/Home/Index.cshtml"
    //            };

    //        }
    //    }
    //}


}
