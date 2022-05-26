using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Library;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{

    public class LogBookController : SController
    {
        LogBookAction _lbAct = new LogBookAction();
        // GET: LogBook
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult SaveReceivebySecurity(LogBook _logbook)
        {
            _logbook.Plant = Session["UsePlant"].ToString();
            return Json(_lbAct.SaveReceivebySecurity(_logbook), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveReceivebyUser (string LogIds, string Photoes, string NameReceives)
        {
            return Json(_lbAct.SaveReceivebyUser(LogIds, Photoes, NameReceives), JsonRequestBehavior.AllowGet);
        }
        public FileResult ExportLogtoExcel(string DateFromReport, string DateToReport, string KeyWordReport)
        {
            DataTable table = new DataTable("HistoryReport");
            table = _lbAct.ExportLogBook(DateFromReport, DateToReport, KeyWordReport);
            table.TableName = "HistoryReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_LogBookReport" + DateFromReport + "_" + DateToReport + ".xlsx");
                }
            }
        }
        public JsonResult ShowLostItem(string dateFrom, string dateTo, string KeyWord)
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
                var UsePlant = Session["UsePlant"].ToString();
                var _LogBook = _lbAct.ShowLogBook("2", dateFrom, dateTo, KeyWord, UsePlant);

                //total number of rows count   
                recordsTotal = _LogBook.Count();
                //Paging   
                var data = _LogBook.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult ShowReceivePackage(string dateFrom, string dateTo, string KeyWord)
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
                var UsePlant = Session["UsePlant"].ToString();
                var _LogBook = _lbAct.ShowLogBook("1", dateFrom, dateTo, KeyWord, UsePlant);

                //total number of rows count   
                recordsTotal = _LogBook.Count();
                //Paging   
                var data = _LogBook.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult ShowAllReport (string dateFrom, string dateTo, string KeyWord)
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
                var UsePlant = Session["UsePlant"].ToString();
                var _LogBook = _lbAct.ShowAllLogBook(dateFrom, dateTo, KeyWord, UsePlant);

                //total number of rows count   
                recordsTotal = _LogBook.Count();
                //Paging   
                var data = _LogBook.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult Capture(string Type)
        {
            var stream = Request.InputStream;
            string dump;
            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();
                DateTime nm = DateTime.Now;
                string name = nm.ToString("yy_MM_dd_hh_mm") + "_" + Type + ".jpg";
                var path = Server.MapPath("~/Uploads/LogBook/") + name;
                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
                Session["PhotoFile"] = name;
            }
            return Json("Index");
        }

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
        public JsonResult Rebind()
        {
            try
            {
                if (Session["PhotoFile"] != null)
                {
                    string path = Session["PhotoFile"].ToString();
                    return Json(path, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["PhotoFile"] = null;
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SeePhotoes(string LogId)
        {
            return Json(_lbAct.SeePhotoes(LogId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Capture2(string Type, string FileName, HttpPostedFileBase webcamPhoto)
        {
            if (webcamPhoto != null && webcamPhoto.ContentLength > 0)
            {
                try
                {
                    DateTime nm = DateTime.Now;
                    string name = FileName;
                    var filePath = Path.Combine(Server.MapPath("~/Uploads/LogBook/"), name);
                    webcamPhoto.SaveAs(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {

            }
            return Json("Index");
        }
        [HttpPost]
        public JsonResult Capture3(string Type, string FileName, HttpPostedFileBase webcamPhoto)
        {
            if (webcamPhoto != null && webcamPhoto.ContentLength > 0)
            {
                try
                {
                    DateTime nm = DateTime.Now;
                    string name = FileName;
                    var filePath = Path.Combine(Server.MapPath("~/Uploads/LogBook/"), name);
                    webcamPhoto.SaveAs(filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {

            }
            return Json("Index");
        }

        public JsonResult GetFileName(int GetReceive)
        {
            try
            {
                if (GetReceive == 1)
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();
                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }
                    var finalString = new String(stringChars);
                    DateTime nm = DateTime.Now;
                    string filename = nm.ToString("yy_MM_dd_hh_mm") + "_" + finalString + ".jpg";
                    return Json(filename, JsonRequestBehavior.AllowGet);
                }
                else if (GetReceive == 2)
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();
                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }
                    var finalString = new String(stringChars);
                    DateTime nm = DateTime.Now;
                    string filename = nm.ToString("yy_MM_dd_hh_mm") + "_" + finalString + ".jpg";
                    return Json(filename, JsonRequestBehavior.AllowGet);
                }
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
    }
}