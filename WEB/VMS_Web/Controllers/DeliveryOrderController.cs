using ClosedXML.Excel;
using Kendo.Mvc.Extensions;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VMS.Web.Attribute;
using VMS.Web.Models;

namespace VMS.Web.Controllers
{
    public class DeliveryOrderController : Controller
    {
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        string _UseId;
        private DBVisitorMSEntities db = new DBVisitorMSEntities();
        DeliveryAction _actDeliv = new DeliveryAction();
        Notifications _Nofif = new Notifications();
        // GET: DeliveryOrder
        [ShimanoCustom]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MobileDriver()
        {
            return View();
        }
        public ActionResult MobileDriver_DriverHistory()
        {
            return View();
        }
        public ActionResult MobileUser(string UseId)
        {
            HttpCookie cookie = new HttpCookie("cookie_useid", UseId)
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Add(cookie);
            return View();
        }
        public ActionResult SaveOrder(string UseDep, string UseID, string Address, string DelVia, string DriName, string VechNo, string SealNo, string ContNo, DODetail[] DODetail_)
        {
            string result = "Error!";
            if (UseDep != null && Address != null && DODetail_ != null)
            {
                var query = "Declare @MaxNo varchar(50), @UniqNo varchar(10) select @MaxNo = isnull(MAX(convert(bigint, RIGHT(SENo, 3))), 0) + 1 from DOMaster where CONVERT(date, ReqDate) = convert(date, getdate()) select @UniqNo = right(convert(varchar, 1000 + @MaxNo), 3) SELECT @UniqNo";
                var query_ = db.Database.SqlQuery<string>(query).First();
                var masterId = Guid.NewGuid();
                string DONo_ = "DO" + DateTime.Now.ToString("yyMMdd") + "-" + query_.ToString();

                if (DelVia == "SBM_DRIVER")
                {
                    DOMaster master = new DOMaster
                    {
                        MasterId = masterId,
                        SENo = Convert.ToInt32(query_),
                        DONo = DONo_,
                        UseDep = UseDep,
                        UseID = UseID,
                        ReqDate = DateTime.Now,
                        Address = Address,
                        DelVia = DelVia,
                        TimeOut = TimeSpan.Parse("00:00:00"),
                        //master.ManagerApprove = false;
                        Status = "WAITING HOD APPROVAL",
                        UpdateBy = Session["UseID"].ToString(),
                        UpdateDate = DateTime.Now
                    };

                    db.DOMasters.Add(master);

                    foreach (var item in DODetail_)
                    {
                        var detailId = Guid.NewGuid();
                        //var DODetails = new DODetail();
                        DODetail detail = new DODetail
                        {
                            DetailId = detailId,
                            MasterId = masterId,
                            Product = item.Product,
                            Quantity = item.Quantity,
                            Remark = item.Remark,
                            IsReturned = item.IsReturned,
                            ReturnedBy = item.ReturnedBy,
                            Photo = item.Photo,
                            UpdateBy = Session["UseID"].ToString(),
                            UpdateDate = DateTime.Now
                        };
                        db.DODetails.Add(detail);
                    }
                    db.SaveChanges();
                    result = "Saved Success !";
                    //if (result == "Saved Success !")
                    //{
                    //    var _User = _Nofif.GetUserApprovalDO(masterId, DONo_);
                    //    string queryMAIL = $@"EMAIL_DELIVERY_ORDER '{masterId}'";
                    //    using (var sql = new MSSQL())
                    //    {
                    //        num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                    //    }
                    //}
                }
                else if (DelVia == "OTHERS")
                {
                    DOMaster master = new DOMaster
                    {
                        MasterId = masterId,
                        SENo = Convert.ToInt32(query_),
                        DONo = DONo_,
                        UseDep = UseDep,
                        UseID = UseID,
                        ReqDate = DateTime.Now,
                        Address = Address,
                        DelVia = DelVia,
                        DriName = DriName,
                        //master.VechNo = VechNo;
                        //master.SealNo = SealNo;
                        // master.ContainerNo = ContNo;
                        TimeOut = TimeSpan.Parse("00:00:00"),
                        //master.ManagerApprove = false;
                        Status = "WAITING HOD APPROVAL",
                        UpdateBy = Session["UseID"].ToString(),
                        UpdateDate = DateTime.Now
                    };

                    db.DOMasters.Add(master);

                    foreach (var item in DODetail_)
                    {
                        var detailId = Guid.NewGuid();
                        //var DODetails = new DODetail();
                        DODetail detail = new DODetail
                        {
                            DetailId = detailId,
                            MasterId = masterId,
                            Product = item.Product,
                            Quantity = item.Quantity,
                            Remark = item.Remark,
                            IsReturned = item.IsReturned,
                            ReturnedBy = item.ReturnedBy,
                            Photo = item.Photo,
                            UpdateBy = Session["UseID"].ToString(),
                            UpdateDate = DateTime.Now
                        };
                        db.DODetails.Add(detail);
                    }
                    db.SaveChanges();
                    result = "Saved Success !";
                    if (result == "Saved Success !")
                    {
                        var _User = _Nofif.GetUserApprovalDO(masterId, DONo_);
                        //string queryMAIL = $@"EMAIL_DELIVERY_ORDER '{masterId}'";
                        //using (var sql = new MSSQL())
                        //{
                        //    num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                        //}
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveOrderMobile(string UseDep, string UseID, string Address, string DelVia, string DriName, string VechNo, string SealNo, string ContNo,  DODetail[] DODetail_)
        {
            HttpCookie cookie2 = HttpContext.Request.Cookies.Get("cookie_useid");
            _UseId = cookie2.Value.ToString();
            string result = "Error!";
            if (UseDep != null && Address != null && DODetail_ != null)
            {
                var query = "Declare @MaxNo varchar(50), @UniqNo varchar(10) select @MaxNo = isnull(MAX(convert(bigint, RIGHT(SENo, 3))), 0) + 1 from DOMaster where CONVERT(date, ReqDate) = convert(date, getdate()) select @UniqNo = right(convert(varchar, 1000 + @MaxNo), 3) SELECT @UniqNo";
                var query_ = db.Database.SqlQuery<string>(query).First();
                var masterId = Guid.NewGuid();
                string DONo_ = "DO" + DateTime.Now.ToString("yyMMdd") + "-" + query_.ToString();

                if (DelVia == "SBM_DRIVER")
                {
                    DOMaster master = new DOMaster
                    {
                        MasterId = masterId,
                        SENo = Convert.ToInt32(query_),
                        DONo = DONo_,
                        UseDep = UseDep,
                        UseID = UseID,
                        ReqDate = DateTime.Now,
                        Address = Address,
                        DelVia = DelVia,
                        TimeOut = TimeSpan.Parse("00:00:00"),
                        //master.ManagerApprove = false;
                        Status = "WAITING HOD APPROVAL",
                        UpdateBy = _UseId,
                        UpdateDate = DateTime.Now
                    };

                    db.DOMasters.Add(master);

                    foreach (var item in DODetail_)
                    {
                        var detailId = Guid.NewGuid();
                        //var DODetails = new DODetail();
                        DODetail detail = new DODetail
                        {
                            DetailId = detailId,
                            MasterId = masterId,
                            Product = item.Product,
                            Quantity = item.Quantity,
                            Remark = item.Remark,
                            IsReturned = item.IsReturned,
                            ReturnedBy = item.ReturnedBy,
                            Photo = item.Photo,
                            UpdateBy = _UseId,
                            UpdateDate = DateTime.Now
                        };
                        db.DODetails.Add(detail);
                    }
                    db.SaveChanges();
                    result = "Saved Success !";
                    if (result == "Saved Success !")
                    {
                        var _User = _Nofif.GetUserApprovalDO(masterId, DONo_);
                        //string queryMAIL = $@"EMAIL_DELIVERY_ORDER '{masterId}'";
                        //using (var sql = new MSSQL())
                        //{
                        //    num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                        //}
                    }
                }
                else if (DelVia == "OTHERS")
                {
                    DOMaster master = new DOMaster();
                    master.MasterId = masterId;
                    master.SENo = Convert.ToInt32(query_);
                    master.DONo = DONo_;
                    master.UseDep = UseDep;
                    master.UseID = UseID;
                    master.ReqDate = DateTime.Now;
                    master.Address = Address;
                    master.DelVia = DelVia;
                    master.DriName = DriName;
                    //master.VechNo = VechNo;
                    //master.SealNo = SealNo;
                    // master.ContainerNo = ContNo;
                    master.TimeOut = TimeSpan.Parse("00:00:00");
                    //master.ManagerApprove = false;
                    master.Status = "WAITING HOD APPROVAL";
                    master.UpdateBy = _UseId;
                    master.UpdateDate = DateTime.Now;

                    db.DOMasters.Add(master);

                    foreach (var item in DODetail_)
                    {
                        var detailId = Guid.NewGuid();
                        //var DODetails = new DODetail();
                        DODetail detail = new DODetail
                        {
                            DetailId = detailId,
                            MasterId = masterId,
                            Product = item.Product,
                            Quantity = item.Quantity,
                            Remark = item.Remark,
                            IsReturned = item.IsReturned,
                            ReturnedBy = item.ReturnedBy,
                            Photo = item.Photo,
                            UpdateBy = _UseId,
                            UpdateDate = DateTime.Now
                        };
                        db.DODetails.Add(detail);
                    }
                    db.SaveChanges();
                    result = "Saved Success !";
                    //if (result == "Saved Success !")
                    //{
                    //    var _User = _Nofif.GetUserApprovalDO(masterId, DONo_);
                    //    string queryMAIL = $@"EMAIL_DELIVERY_ORDER '{masterId}'";
                    //    using (var sql = new MSSQL())
                    //    {
                    //        num = sql.ExecNonQuery(ConnectionDB, queryMAIL, null, null, false);
                    //    }
                    //}
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Manager(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                if (or.DelVia == "SBM_DRIVER")
                {
                    if (model.ManagerApprove == false)
                    {
                        or.ManagerApprove = model.ManagerApprove;
                        or.Status = "REJECTED";
                        or.UpdateBy = Session["UseID"].ToString();
                        or.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        or.ManagerApprove = model.ManagerApprove;
                        or.Status = "WAITING DRIVER";
                        or.UpdateBy = Session["UseID"].ToString();
                        or.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                else if (or.DelVia == "OTHERS")
                {
                    if (model.ManagerApprove == false)
                    {
                        or.ManagerApprove = model.ManagerApprove;
                        or.Status = "REJECTED";
                        or.UpdateBy = Session["UseID"].ToString();
                        or.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        or.ManagerApprove = model.ManagerApprove;
                        or.Status = "OUTPOST CHECK";
                        or.UpdateBy = Session["UseID"].ToString();
                        or.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Receptionist(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.VechNo = model.VechNo;
                or.DriName = model.DriName;
                or.SealNo = model.SealNo;
                or.ContainerNo = model.ContainerNo;
                or.Status = "OUTPOST CHECK";
                or.UpdateBy = Session["UseID"].ToString();
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Security(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                if (or.DelVia == "SBM_DRIVER")
                {
                    or.TimeOut = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                    or.SecurityCheck = Session["UseID"].ToString();
                    or.Status = "DELIVERING";
                    or.SecurityPic = model.SecurityPic;
                    or.UpdateBy = Session["UseID"].ToString();
                    or.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
                else if (or.DelVia == "OTHERS")
                {
                    or.TimeOut = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                    or.SecurityCheck = Session["UseID"].ToString();
                    or.Status = "DELIVERED";
                    or.SecurityPic = model.SecurityPic;
                    or.ReceiverName = model.ReceiverName;
                    or.ReceivedDate = DateTime.Now;
                    or.ReceivedPic = model.SecurityPic;
                    or.UpdateBy = Session["UseID"].ToString();
                    or.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
                //DODetail or1 = db.DODetails.SingleOrDefault(x => x.MasterId == or.MasterId);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DriverPick(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.VechNo = model.VechNo;
                or.DriName = model.DriName;
                or.SealNo = model.SealNo;
                or.ContainerNo = model.ContainerNo;
                or.Status = "OUTPOST CHECK";
                or.UpdateBy = model.DriName;
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DriverReceived(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.ReceiverName = model.ReceiverName;
                or.ReceivedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                or.ReceivedPic = model.ReceivedPic;
                or.Status = "DELIVERED";
                or.UpdateBy = model.DriName;
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DriverCancel(DOMaster model)
        {
            if (model.SENo > 0)
            {
                DOMaster or = db.DOMasters.SingleOrDefault(x => x.SENo == model.SENo);
                or.Status = "CANCELED";
                or.UpdateBy = model.DriName;
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ItemReturn(DODetail model)
        {
            if (model.Id > 0)
            {
                DODetail or = db.DODetails.SingleOrDefault(x => x.Id == model.Id);
                or.ReturnedDate = model.ReturnedDate;
                or.UpdateBy = Session["UseID"].ToString();
                or.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            return View(model);
        }
        public JsonResult Show_DO_Outstanding(string dateFrom, string dateTo, string KeyWord, string _User)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (Session["BusFunc"].ToString() == "SYSTEM-MANAGER" || Session["BusFunc"].ToString() == "SYSTEM-DIRECTOR")
                {
                    var _DO = _actDeliv._Show_DO_Outstanding_Manager(dateFrom, dateTo, KeyWord);
                    recordsTotal = _DO.Count();
                    var data = _DO.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else if (Session["BusFunc"].ToString() == "SYSTEM-RECEIPTIONIST")
                {
                    var _DO = _actDeliv._Show_DO_Outstanding_Receptionist(dateFrom, dateTo, KeyWord);
                    recordsTotal = _DO.Count();
                    var data = _DO.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else if (Session["BusFunc"].ToString() == "SYSTEM-SECURITY")
                {
                    var _DO = _actDeliv._Show_DO_Outstanding_Security(dateFrom, dateTo, KeyWord);
                    recordsTotal = _DO.Count();
                    var data = _DO.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else if (Session["BusFunc"].ToString() == "SYSTEM-USER")
                {
                    _User = Session["UseID"].ToString();
                    var _DO = _actDeliv._Show_DO_Outstanding_User(dateFrom, dateTo, KeyWord, _User);
                    recordsTotal = _DO.Count();
                    var data = _DO.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else if (Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
                {
                    var _DO = _actDeliv._Show_DO_Outstanding_Admin(dateFrom, dateTo, KeyWord);
                    recordsTotal = _DO.Count();
                    var data = _DO.Skip(skip).Take(pageSize).ToList();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult Show_DO_Outstanding_Driver(string dateFrom, string dateTo, string KeyWord)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = 50;
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int pageSize = Convert.ToInt32(length);
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var _DO = _actDeliv._Show_DO_Outstanding_Driver(dateFrom, dateTo, KeyWord);

                recordsTotal = _DO.Count();
                var data = _DO.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult Show_DO_Outstanding_Driver_Delivered(string dateFrom, string Requestor, string KeyWord)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = 50;
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int pageSize = Convert.ToInt32(length);
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var _DO = _actDeliv._Show_DO_Outstanding_Driver_Delivered(dateFrom, Requestor, KeyWord);

                recordsTotal = _DO.Count();
                var data = _DO.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult Show_DO_Returned(string dateFrom, string dateTo, string KeyWord)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var _DO = _actDeliv._Show_DO_Returned(dateFrom, dateTo, KeyWord);
 
                recordsTotal = _DO.Count(); 
                var data = _DO.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult Show_DO_Report(string dateFrom, string dateTo, string KeyWord)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var _DO = _actDeliv._Show_DO_AllItem(dateFrom, dateTo, KeyWord);

                recordsTotal = _DO.Count();
                var data = _DO.Skip(skip).Take(pageSize).ToList();

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
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);

                dump = reader.ReadToEnd();
                DateTime nm = DateTime.Now;
                string name = nm.ToString("yyyy-MM-dd") + "-" + finalString + ".jpg";
                var path = Server.MapPath("~/Uploads/DOItem/") + name;
                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
                Session["PhotoFile"] = name;
            }
            return Json("Index");
        }
        [HttpPost]
        public void SecurityCapture(HttpPostedFileBase webcamPhoto, string FileName)
        {
            if (webcamPhoto != null && webcamPhoto.ContentLength > 0)
            {
                var fileName = FileName;
                var filePath = Path.Combine(Server.MapPath("~/Uploads/DOItem/"), fileName);
                webcamPhoto.SaveAs(filePath);
            }
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
            string path = Session["PhotoFile"].ToString();
            return Json(path, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFileName1()
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
            var fileName = "ITM" + date.ToString("yyyyMMdd") + "-" + finalString + ".jpg";
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFileName2()
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
            var fileName = "OPC" + date.ToString("yyyyMMdd") + "-" + finalString + ".jpg";
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManagerApprove(int SENo)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-MANAGER" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
            {
                DOViewModel model = new DOViewModel();
                DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
                //var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
                return PartialView("PartialManagerApprove", or);
            }
            else
            {
                return PartialView("Error");
            }
        }
        public ActionResult PickDriver(int SENo, Guid MasterId)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-RECEIPTIONIST" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
            {
                DOViewModel model = new DOViewModel();
                DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
                var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
                return PartialView("PartialDriverPick", or);
            }
            else
            {
                return PartialView("Error");
            }
        }
        public ActionResult MobilePickDriver(int SENo, Guid MasterId)
        {
            DOViewModel model = new DOViewModel();
            DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
            var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
            return PartialView("MobileDriver_DriverPick", or);
        }
        public ActionResult MobileDelivered(int SENo, Guid MasterId)
        {
            DOViewModel model = new DOViewModel();
            DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
            var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
            return PartialView("MobileDriver_DriverDelivered", or);
        }
        public ActionResult MobileDetails(int SENo, Guid MasterId)
        {
            DOViewModel model = new DOViewModel();
            DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
            var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
            return PartialView("MobileDriver_DriverDetails", or);
        }
        public ActionResult SecurityCheck(int SENo)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-SECURITY" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
            {
                DOViewModel model = new DOViewModel();
                DOMaster or = db.DOMasters.SingleOrDefault(a => a.SENo == SENo);
                //var or1 = db.DODetails.Where(b => b.MasterId == MasterId).ToList();
                if (or.DelVia == "SBM_DRIVER")
                {
                    return View("PartialSecurityCheck", or);
                }
                else if (or.DelVia == "OTHERS")
                {
                    return View("PartialSecurityCheck1", or);
                }
                return null;
            }
            else
            {
                return PartialView("Error");
            }
        }
        public ActionResult ReturnItem (int Id)
        {
            if (Session["BusFunc"].ToString() == "SYSTEM-SECURITY" || Session["BusFunc"].ToString() == "SYSTEM-ADMIN")
            {
                DODetail model = new DODetail();
                DODetail or = db.DODetails.SingleOrDefault(c => c.Id == Id);
                model.DetailId = or.DetailId;
                model.MasterId = or.MasterId;
                model.Id = or.Id;
                model.Product = or.Product;
                model.Quantity = or.Quantity;
                model.Remark = or.Remark;
                model.IsReturned = or.IsReturned;
                model.ReturnedBy = or.ReturnedBy;
                model.ReturnedDate = or.ReturnedDate;
                model.Photo = or.Photo;
                return PartialView("PartialViewReturnItem", model);
            }
            else
            {
                return PartialView("Error");
            }
        }
        public ActionResult SeeDetails (int Id)
        {
            if (Session["BusFunc"].ToString() != "")
            {
                DODetail model = new DODetail();
                DODetail or = db.DODetails.SingleOrDefault(c => c.Id == Id);
                model.DetailId = or.DetailId;
                model.MasterId = or.MasterId;
                model.Id = or.Id;
                model.Product = or.Product;
                model.Quantity = or.Quantity;
                model.Remark = or.Remark;
                model.IsReturned = or.IsReturned;
                model.ReturnedBy = or.ReturnedBy;
                model.ReturnedDate = or.ReturnedDate;
                model.Photo = or.Photo;
                return PartialView("PartialViewDetails", model);
            }
            else
            {
                return PartialView("Error");
            }
        }
        public FileResult ExportDeliveryOrder(string DateFromDA, string DateToDA, string DONo)
        {
            DataTable table = new DataTable("DeliveryOrderReport");
            table = _actDeliv.ExportDeliveryOrder(DateFromDA, DateToDA, DONo);
            table.TableName = "DeliveryOrderReport";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       DateTime.Now.ToString("yyMMdd") + "_DOLog" + DateFromDA + "_" + DateToDA + ".xlsx");
                }
            }
        }
        public ActionResult UploadFiles()
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
            var fileName = "ITM" + date.ToString("yyyyMMdd") + "-" + finalString;

            string FileName = "";
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname;
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
                        fname = Path.Combine(Server.MapPath("~/Uploads/DOItem/"), fname);
                        file.SaveAs(fname);
                    }
                    else if (file.FileName.Contains(".jpg") || file.FileName.Contains(".jpeg"))
                    {
                        fname = fileName + ".jpg";
                        FileName = fileName + ".jpg";
                        fname = Path.Combine(Server.MapPath("~/Uploads/DOItem/"), fname);
                        file.SaveAs(fname);
                    }
                    else if (file.FileName.Contains(".jpeg"))
                    {
                        fname = fileName + ".jpeg";
                        FileName = fileName + ".jpeg";
                        fname = Path.Combine(Server.MapPath("~/Uploads/DOItem/"), fname);
                        file.SaveAs(fname);
                    }
                }
            }
            return Json(FileName, JsonRequestBehavior.AllowGet);
        }
    }
}
