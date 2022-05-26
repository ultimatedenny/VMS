using Kendo.Mvc.Extensions;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class BlogController : Controller
    {
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        //string _UseId, _token;
        string fname;
        DBVisitorMSEntities db = new DBVisitorMSEntities();

        public ActionResult Index()
        {
            var news = from m in db.NewsMasters where m.CreatedDate > new DateTime(1984, 6, 1)
                       select m;
            return View(news.ToList());
        }
        public ActionResult AddNews(string Header, string BodyTEXT, string BodyHTML, string FeaturedImage)
        {
            string result = "Error!";
            if (Header != null && (BodyTEXT != null || BodyHTML != null))
            {
                try
                {
                    var query = "Declare @MaxNo varchar(50), @UniqNo varchar(10) select @MaxNo = isnull(MAX(convert(bigint, RIGHT(NewsId, 3))), 0) + 1 from NewsMaster where CONVERT(date, CreatedDate) = convert(date, getdate()) select @UniqNo = right(convert(varchar, 1000 + @MaxNo), 3) SELECT @UniqNo";
                    var query_ = db.Database.SqlQuery<string>(query).First();
                    var masterId = Guid.NewGuid();
                    string NEWSNO = "NW" + DateTime.Now.ToString("yyMMdd") + "-" + query_.ToString();

                    var scheme = Request.Url.Scheme; // will get http, https, etc.
                    var host = Request.Url.Host; // will get www.mywebsite.com
                    var port = Request.Url.Port; // will get the port
                    var path = Request.Url.AbsolutePath; // should get the /pages/page1.aspx part, can't remember if it only get pages/page1.aspx

                    NewsMaster model = new NewsMaster
                    {
                        MasterId = masterId,
                        NewsId = NEWSNO,
                        Header = Header,
                        BodyTEXT = BodyTEXT,
                        BodyHTML = BodyHTML,
                        FeaturedImage = scheme + "://" + host + "/VisitorMSQA/Uploads/" + FeaturedImage,
                        //Path.Combine(Server.MapPath("~/Uploads/"), FeaturedImage);
                        //model.CreatedBy = Session["UseID"].ToString();
                        CreatedBy = "sbm_deni",
                        CreatedDate = DateTime.Now,
                        Viewer = 1
                    };

                    db.NewsMasters.Add(model);
                    db.SaveChanges();

                    result = "OK!";
                }
                catch (Exception e)
                {
                    result = e.ToString();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadImage(HttpPostedFileBase file, HttpPostedFileBase FileName)
        {
            var fileName = file.FileName.ToString();
            var filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
            if (file != null && file.ContentLength > 0)
            {
                file.SaveAs(filePath);
            }
            return Json(new { location = fileName });
        }
        public ActionResult Details(Guid masterId)
        {
            if (masterId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsMaster News = db.NewsMasters.Find(masterId);
            if (News == null)
            {
                return HttpNotFound();
            }
            return View(News);
        }
        public ActionResult UploadFeaturedImage()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            DateTime date = DateTime.Now;
            var fileName = "BLG" + date.ToString("yyyyMMdd") + "-" + finalString;

            string FileName = "";
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    if (file.FileName.Contains(".png"))
                    {
                        fname = fileName + ".png";
                        FileName = fileName + ".png";
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                    else if (file.FileName.Contains(".jpg") || file.FileName.Contains(".jpeg"))
                    {
                        fname = fileName + ".jpg";
                        FileName = fileName + ".jpg";
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                    else if (file.FileName.Contains(".jpeg"))
                    {
                        fname = fileName + ".jpeg";
                        FileName = fileName + ".jpeg";
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                }
            }
            return Json(FileName, JsonRequestBehavior.AllowGet);
        }
    }
}