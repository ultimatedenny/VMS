using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace VMS.Library
{
    public static class Cookies
    {
        public static string GetUseID()
        {
            return GetCookies("UseID");
        }
        public static string GetBusFunc()
        {
            return GetCookies("BusFunc");
        }
        public static string GetCookies(string Key)
        {
            var Value = HttpContext.Current.Request.Cookies[Key] != null
                ? HttpContext.Current.Request.Cookies[Key].Value.Decrypt()
                : null;
            return Value;
        }
        public static void PostCookies(string Key, string Value)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(Key, Value.Encrypt()));
        }
        public static string GetCookiesWithoutEnc(string Key)
        {
            var Value = HttpContext.Current.Request.Cookies[Key] != null
                ? HttpContext.Current.Request.Cookies[Key].Value
                : null;
            return Value;
        }
        public static void PostCookiesWithoutEnc(string Key, string Value)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(Key, Value));
        }
        public static void DeleteCookies(string Key)
        {
            HttpContext.Current.Response.Cookies[Key].Expires = DateTime.Now.AddDays(-1);
        }
    }
    public static class Sessions
    {
        public static string GetUseID()
        {
            return (HttpContext.Current.Session["UseID"] != null ? HttpContext.Current.Session["UseID"].ToString() : null);
        }
        public static string GetUserName()
        {
            return (HttpContext.Current.Session["UseNam"] != null ? HttpContext.Current.Session["UseNam"].ToString() : null);
        }
        public static string GetBusFunc()
        {
            return (HttpContext.Current.Session["BusFunc"] != null ? HttpContext.Current.Session["BusFunc"].ToString() : null);
        }
        public static string GetUseLev()
        {
            return (HttpContext.Current.Session["UseLev"] != null ? HttpContext.Current.Session["UseLev"].ToString() : null);
        }
        public static string GetApprovalGroup()
        {
            return (HttpContext.Current.Session["ApprovalGroup"] != null ? HttpContext.Current.Session["ApprovalGroup"].ToString() : null);
        }
        public static string GetUsePlant()
        {
            return (HttpContext.Current.Session["UsePlant"] != null ? HttpContext.Current.Session["UsePlant"].ToString() : null);
        }
        public static string GetAuthorisedDelegate()
        {
            return (HttpContext.Current.Session["IsAuthorisedDelegate"] != null ? HttpContext.Current.Session["IsAuthorisedDelegate"].ToString() : null);
        }
        public static string GetDelegatePeriod()
        {
            return (HttpContext.Current.Session["DelegatePeriod"] != null ? HttpContext.Current.Session["DelegatePeriod"].ToString() : null);
        }
        public static string GetDelegateUseID()
        {
            return (HttpContext.Current.Session["DelegateUseID"] != null ? HttpContext.Current.Session["DelegateUseID"].ToString() : null);
        }
    }
}
