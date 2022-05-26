
﻿using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;

using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class TSVisitorController : Controller
    {
        const string Command = @"/C @echo off
""\\sbm-vmiis03\Uploads\WifiAccount\plink.exe"" cmd2cli@172.18.102.200 -pw ""admin12345"" <\\sbm-vmiis03\Uploads\WifiAccount\command.txt
exit
";
        TSVisitorAction _tsVisit = new TSVisitorAction();
        // GET: TSVisitor
        public void GetTheWifi(string Username, string ExpTime= "23:00")
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string fileName = @"\\sbm-vmiis03\WifiAccount\command.txt";
            var Password = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            var UseNam = Username.ToUpper().Replace(" ", "").Substring(0, 4) + new string(Enumerable.Repeat(chars, 2)
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
                    sw.WriteLine("local-userdb add username {1} password {2} expiry time {0} {3}", DateTime.Now.ToString("dd/MM/yyyy"), UseNam, Password,ExpTime);
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
                Console.WriteLine(Ex.ToString());
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetVisitorTSToday([DataSourceRequest] DataSourceRequest request, string Name)
        {
            try
            {
                return Json(_tsVisit.GetVisitorTSToday(Name).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //================
        public ActionResult TeamShimanoArrival()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadAppointment(HttpPostedFileBase FileUpload)
        {
            string filename = "";
            string pathToExcelFile = "";
            try
            {

                if (FileUpload != null)
                {
                    filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Uploads/VisitorLog/");
                    pathToExcelFile = targetpath + filename;
                    if (System.IO.File.Exists(pathToExcelFile))
                    {
                        TempData["isSuccess"] = false;
                        TempData["message"] = "This File has been uploaded before!!";
                        return RedirectToAction("TeamShimanoArrival");
                    }
                    FileUpload.SaveAs(pathToExcelFile);
                    var data = _tsVisit.PostTempVisitor(pathToExcelFile, filename);
                    if (!data.Success)
                    {
                        if (pathToExcelFile != null || pathToExcelFile != string.Empty)
                        {
                            if (System.IO.File.Exists(pathToExcelFile))
                            {
                                System.IO.File.Delete(pathToExcelFile);
                            }
                        }
                    }
                    TempData["isSuccess"] = data.Success;
                    TempData["message"] = data.Message;
                    return RedirectToAction("TeamShimanoArrival");

                }
                else
                {
                    TempData["isSuccess"] = false;
                    TempData["message"] = "Please Upload the file";
                    return RedirectToAction("TeamShimanoArrival");
                }
            }
            catch (Exception ex)
            {
                if (pathToExcelFile != null || pathToExcelFile != string.Empty)
                {
                    if (System.IO.File.Exists(pathToExcelFile))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                }
                TempData["isSuccess"] = false;
                TempData["message"] = ex.Message.ToString(); //"Failed upload the File!\n"
                return RedirectToAction("TeamShimanoArrival");
            }
            
        }
        public FileResult DownloadTemplate()
        {
            string path = "~/Uploads/VisitorList.xlsx";
            return File(path, "application/vnd.ms-excel", "VisitorList"+ DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
        }
        public ActionResult ReportTeamShimano()
        {
            return View();
        }
        public JsonResult GetTSReport([DataSourceRequest] DataSourceRequest request, string dateFrom, string dateTo)
        {
            return Json(_tsVisit.GetTSReport(dateFrom, dateTo).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public FileResult ExportTSReport(string DateFromHis, string DateToHis)
        {
            DataTable table = new DataTable("HistoryReport");
            table = _tsVisit.GetDtTSReport(DateFromHis, DateToHis);


            table.TableName = "HistoryReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_HistoryVisitLog" + DateFromHis + "_" + DateToHis + ".xlsx");
                }
            }
        }


        public JsonResult GetTempVisitor(string dateFrom, string dateTo, string name)
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

                var _visitors = _tsVisit.GetTempVisitor(dateFrom, dateTo, name);

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
        public JsonResult GetTSVisitorTempDet(string Id)
        {
            return Json(_tsVisit.GetTSVisitorTempDet(Id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveVisitorTemp(Temp_VisitorTS tempvisitor)
        {
            return Json(_tsVisit.SaveVisitorTemp(tempvisitor), JsonRequestBehavior.AllowGet);
        }
        public FileResult GetExportExcel(string dateFrom, string dateTo)
        {
            DataTable table = new DataTable("HistoryReport");
            table = _tsVisit.GetExportExcel(dateFrom, dateTo);
            table.TableName = "HistoryReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_HistoryTSVisitLog" + dateFrom + "_" + dateTo + ".xlsx");
                }
            }
        }
        public JsonResult DeleteTempVisitor(string id)
        {
            return Json(_tsVisit.DeleteTempVisitor(id), JsonRequestBehavior.AllowGet);
        }
    }
}