using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VMS.Api.Models;
using VMS.Library.Constants;

namespace VMS.Api.Controllers
{
    [RoutePrefix("api/TSVisitor")]
    public class TSVisitorController : ApiController
    {
        const string Command = @"/C @echo off
""\\sbm-vmiis03\Uploads\WifiAccount\plink.exe"" cmd2cli@172.18.102.200 -pw ""admin12345"" <\\sbm-vmiis03\Uploads\WifiAccount\command.txt
exit
";
        TSVisitorAction _visitor = new TSVisitorAction();
        [HttpGet]
        [Route("GetVisitor")]
        public Visitor_TS GetVisitor(string ShimanoBadge)
        {
            return _visitor.GetVisitorFromBadge(ShimanoBadge);
        }
        //====
        [HttpGet]
        [Route("GetListVisitorTemp")]
        public IEnumerable<Temp_VisitorTS> GetListVisitorTemp(string Name)
        {
            return _visitor.GetTemp_VisitorList(Name);
        }

        [HttpGet]
        [Route("GetListVisitorTS")]
        public IEnumerable<Visitor_TS> GetListVisitorTS(string Name)
        {
            return _visitor.GetListVisitorTS(Name);
        }
        [HttpGet]
        [Route("GetVisitorTSForCheck")]
        public Visitor_TS GetVisitorTSForCheck(string ShimanoBadge)
        {
            return _visitor.GetVisitorTSForCheck(ShimanoBadge);
        }

        [HttpPost]
        [Route("PostNewWIFI")]
        public HttpResponseMessage PostNewWIFI(ShimanoWIFI wifi)
        {
            var _data = _visitor.PostNewWIFI(wifi.Host, wifi.Visitor, wifi.UserName, wifi.Password, wifi.CreBy);
            return Request.CreateResponse(_data.isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, _data);
        }
        
        //====
        [HttpPost]
        [Route("PostNewShimanoBadge")]
        public HttpResponseMessage PostNewShimanoBadge(ShimanoBadgeModel postData)
        {
            var _data = _visitor.PostNewShimanoBadge(postData);
            return Request.CreateResponse(_data.isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError,_data);
        }
        [HttpPost]
        [Route("PostChangeShimanoBadge")]
        public HttpResponseMessage PostChangeShimanoBadge(ShimanoBadgeModel postData)
        {
            var _data = _visitor.PostChangeShimanoBadge(postData);
            return Request.CreateResponse(_data.isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, _data);
        }
        //====
        [HttpGet]
        [Route("GetCodLst")]
        public IEnumerable<CodLst> GetCodLst(string GrpCod)
        {
            return _visitor.GetCodLst(GrpCod);
        }
        [HttpGet]
        [Route("GetWifiAccount")]
        public WifiAccount GetWifiAccount(string Username,string wifitime="")
        {
            if(wifitime=="")
            {
                wifitime= DateTime.Now.AddHours(2).ToString(@"HH\:mm");
            }
            WifiAccount WifiAccount;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string fileName = ConfigurationManager.AppSettings["GenerateWifi_FilePath"].ToString(); //@"\\sbm-vmiis03\WifiAccount\command.txt";
            var Password = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var UseNam = Username.ToUpper().Replace(" ", "").Substring(0, 4) + new string(Enumerable.Repeat(chars, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            var WifiConfig_1 = _visitor.GetCodLst(cGrpCodLst.WifiConfig_1);
            string SSID = WifiConfig_1.Where(w => w.Cod == cWifiSetting.SSID)
                                    .Select(m => m.CodAbb)
                                    .FirstOrDefault();
            try
            {
                WifiAccount = new WifiAccount
                {
                    Username = UseNam,
                    Password = Password,
                    WifiName = SSID
                };
                // Check if file already exists. If yes, delete it.     
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                // Create a new file     
                using (StreamWriter sw = System.IO.File.CreateText(fileName))
                {
                    sw.WriteLine("local-userdb add username {1} password {2} expiry time {0} {3}", DateTime.Now.ToString("MM/dd/yyyy"), UseNam, Password, wifitime);
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

            }
            catch (Exception Ex)
            {
                WifiAccount = new WifiAccount
                {
                    Username = WifiConfig_1.Where(w => w.Cod == cWifiSetting.Username)
                                    .Select(m => m.CodAbb)
                                    .FirstOrDefault(),
                    Password = WifiConfig_1.Where(w => w.Cod == cWifiSetting.Password)
                                    .Select(m => m.CodAbb)
                                    .FirstOrDefault(),
                    WifiName = SSID
                };
                Console.WriteLine(Ex.ToString());
            }
            return WifiAccount;
        }
        //====
        [HttpGet]
        [Route("GetorPostVisitorTS")]
        public VisitorJoinLog GetorPostVisitorTS(string ShimanoBadge)
        {
            return _visitor.GetorPostVisitorTS(ShimanoBadge);
        }
        //====
        [HttpPost]
        [Route("PostVisitorCheckIn")]
        public HttpResponseMessage PostVisitorCheckIn(ShimanoBadgeModel postData)
        {
            var _data = _visitor.PostVisitorCheckIn(postData);
            return Request.CreateResponse(_data.isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, _data);
        }
        [HttpPost]
        [Route("PostVisitorCheckOut")]
        public HttpResponseMessage PostVisitorCheckOut(ShimanoBadgeModel postData)
        {
            var _data = _visitor.PostVisitorCheckOut(postData.Temp_Visitor);
            return Request.CreateResponse(_data.isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, _data.message);
        }
        
        [HttpPost]
        [Route("PostVisitorPhoto")]
        public HttpResponseMessage PostVisitorPhoto(PhotoAndroid postData)
        {
            string targetpath = @"\\SBM-NB-IT0277\Files\";
            try
            {
                using (var fs = new FileStream(targetpath + postData.photoName, FileMode.Create,FileAccess.Write))
                {
                    fs.Write(postData.photoData, 0, postData.photoData.Length);

                    return Request.CreateResponse(HttpStatusCode.OK, new MessageNonQuery { isSuccess = true, message = "success" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new MessageNonQuery { isSuccess = false, message = ex.ToString() });
            }
        }
        //===========================================================================================





    }
}
