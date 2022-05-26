using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class ToolBoxController : Controller
    {
        VisitorAction _visitorAction = new VisitorAction();
        // GET: ToolBox
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        [ShimanoCustom]

        public ActionResult WifiAccount()
        {
            return View();
        }
        public JsonResult GetListRegister(string Name, string DateFrom, string DateTo)
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

                var _visitors = _visitorAction.GetListRegister(Name, DateFrom, DateTo);

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
        public JsonResult PostRegisterWifi(RegisterWifi RegisterWifi)
        {
            VMSRes<string> _jsonReturn = new VMSRes<string>
            {
                Success = false,
                Message = "Failed to Create Wifi"
            };
            var _wifiUser = GetTheWifi(RegisterWifi.TimeExpired);
            if(_wifiUser != null)
            {
                RegisterWifi.Username = _wifiUser.Username;
                RegisterWifi.Password = _wifiUser.Password;
                RegisterWifi.CreUser ="SBM_ADMINDP";
                _jsonReturn = _visitorAction.PostRegisterWifi(RegisterWifi);
            }
            return Json(_jsonReturn, JsonRequestBehavior.AllowGet);
        }
        public RegisterWifi GetTheWifi(TimeSpan Time)
        {
            RegisterWifi _req = null;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string fileName = @"\\sbm-vmiis03\WifiAccount\command.txt";
            var Password = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var UseNam = "GUEST".ToUpper().Replace(" ", "").Substring(0, 5) + new string(Enumerable.Repeat(chars, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Create a new file     
                using (StreamWriter sw = System.IO.File.CreateText(fileName))
                {
                    sw.WriteLine("local-userdb add username {1} password {2} expiry time {0} ", DateTime.Now.ToString("MM/dd/yyyy") + " " + Time.ToString(), UseNam, Password);
                    sw.WriteLine("exit");
                }

                // Write file contents on console.     
                using (StreamReader sr = System.IO.File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
                _req = new RegisterWifi
                {
                    Username = UseNam,
                    Password = Password,
                };
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
            return _req;
        }
        public JsonResult GetVisitorForWifi(string HostId)
        {
            return Json(_visitorAction.GetVisitorForWifi(HostId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHostForWifi()
        {
            return Json(_visitorAction.GetHostForWifi(), JsonRequestBehavior.AllowGet);
        }

    }
}