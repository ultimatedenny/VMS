using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class Notifications
    {
        //DataTable dt;
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        //public string ConnectionDB1 = System.Configuration.ConfigurationManager.ConnectionStrings["ApprovalConnection"].ToString();
        public string GetUserApprovalEP(Guid MasterId, string EPNo_)
        {
            string query = @"
            BEGIN
            DECLARE 
            @MasterId uniqueidentifier = '" + MasterId + @"',
            @EPNumber nvarchar(MAX)= '',
            @User nvarchar(MAX),
            @Plant nvarchar(MAX),
            @Department nvarchar(MAX),
            @Approver nvarchar(MAX),
            @Destination nvarchar(MAX)= '',
            @Status nvarchar(MAX) = ''

            SELECT
            @Plant = EM.PlantID,
            @User  = U.UseNam 
            FROM EPMaster EM
            INNER JOIN EPDetails ED on EM.MasterId = ED.MasterId
            INNER JOIN Plant P on EM.PlantID = P.plantId
            INNER JOIN Usr U on ED.UseID = U.UseID
            WHERE EM.MasterId = CONVERT(uniqueidentifier, @MasterId)
            order by  EM.EPNo

            select @EPNumber = EPNo,
	               @Status  = [Status],
	               @Department = [UseDep],
	               @Destination   = [Destination] from EPMaster where MasterId = CONVERT(uniqueidentifier, @MasterId)

             SELECT @Approver = [ApprovalGroup] FROM ApprovalGroup 
            WHERE	Dept = @Department and 
		            Plant = @Plant and 
		            ApprovalGroup = 'MGT'

            IF @Department = 'HR' OR @Department = 'BPR' OR @Department = 'IT'
            BEGIN 
	            SELECT UseId, UseNam FROM USR WHERE UseDep ='IT' AND BusFunc = 'SYSTEM-MANAGER'
            END
            ELSE
	        SELECT	UseId, UseNam FROM USR WHERE 
			        ApprovalGroup = @Approver and 
			        UseDep = @Department and
			        BusFunc LIKE '%MANAGER%'
            END
            ";
            var sql = new MSSQL();
            var GetUseID = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            string No = EPNo_;
            string UseId = "";
            string UseNam = "";
            if (GetUseID != null || GetUseID.Rows.Count > 0)
            {
                foreach (DataRow item1 in GetUseID.Rows)
                {
                    UseId = item1["UseId"].ToString();
                    UseNam = item1["UseNam"].ToString();
                    GetUserToken(No, UseId, UseNam);
                }
            }
            return null;
        }
        public string GetUserApprovalDO(Guid MasterId, string DONo_)
        {
            string query = @"
            BEGIN
            DECLARE 
            @MasterId uniqueidentifier = '" + MasterId + @"',
            @DONumber    nvarchar(MAX)= '',
            @Requestor   nvarchar(MAX),
            @RequestDate nvarchar(MAX),
            @Department  nvarchar(MAX),
            @DelVia	     nvarchar(MAX),
            @Address     nvarchar(MAX),
            @Approver	 nvarchar(MAX),
            @Status      nvarchar(MAX) = ''

            SELECT
            @Department = DM.UseDep,
            @Requestor  = U.UseNam 
            FROM DOMaster DM
            INNER JOIN DODetails DD on DM.MasterId = DD.MasterId
            INNER JOIN Usr U on DM.UseID = U.UseID
            WHERE DM.MasterId = CONVERT(uniqueidentifier, @MasterId)
            order by  DM.DONo

            select @DONumber   = DONo,
	        @Status     = [Status],
	        @Department = [UseDep],
	        @Address    = [Address] from DOMaster where MasterId = CONVERT(uniqueidentifier, @MasterId)

             SELECT @Approver = [ApprovalGroup] FROM ApprovalGroup 
            WHERE	Dept = @Department and
		            ApprovalGroup = 'MGT'

            IF @Department = 'HR' OR @Department = 'BPR' OR @Department = 'IT'
            BEGIN 
	            SELECT UseId, UseNam FROM USR WHERE UseDep ='IT' AND BusFunc = 'SYSTEM-MANAGER'
            END
            ELSE
	        SELECT	UseId, UseNam FROM USR WHERE 
			        ApprovalGroup = @Approver and 
			        UseDep = @Department and
			        BusFunc LIKE '%MANAGER%'
            END
            ";
            var sql = new MSSQL();
            var GetUseID = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            string No = DONo_;
            string UseId = "";
            string UseNam = "";
            if (GetUseID != null || GetUseID.Rows.Count > 0)
            {
                foreach (DataRow item1 in GetUseID.Rows)
                {
                    UseId = item1["UseId"].ToString();
                    UseNam = item1["UseNam"].ToString();
                    GetUserToken(No, UseId, UseNam);
                }
            }
            return null;
        }
        public string GetUserToken(string No, string UseId, string UseNam)
        {
            var deviceId = "";
            var title = "VMS Approval";
            var body = "";
            if (No.Contains("DO"))
            {
                body = "Dear " + UseNam + "," + "\nYou have a pending a pending Delivery Order Approval for " + No;
            }
            else if(No.Contains("EP"))
            {
                body = "Dear " + UseNam + "," + "\nYou have a pending a pending Exit Permit Approval for " + No;
            }
            
            var click_action = "";
            string query = @"select Token from DBAPPROVAL.DBO.tblToken where useid = '" + UseId + "'";
            var sql = new MSSQL();
            var GetToken = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            if (GetToken != null || GetToken.Rows.Count > 0)
            {
                foreach (DataRow TokenId in GetToken.Rows)
                {
                    deviceId = TokenId["Token"].ToString();
                    SendNotificationJSON(deviceId, title, body, click_action);
                }
            }
            return null;
        }
        public string SendNotificationJSON(string deviceId, string title, string body, string click_action)
        {
            string SERVER_KEY_TOKEN = ConfigurationManager.AppSettings["FCMServerToken"].ToString();
            var SENDER_ID = ConfigurationManager.AppSettings["FCMSenderID"];

            WebProxy proxy = new WebProxy("http://172.18.100.54:80", true);
            proxy.Credentials = new NetworkCredential("sbm_admindino", "ojolali");
            WebRequest.DefaultWebProxy = proxy;

            WebRequest tRequest;
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = " application/json";

            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_KEY_TOKEN));
            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            var a = new
            {
                notification = new
                {
                    title,
                    body,
                    icon = "https://domain/path/to/logo.png",
                    click_action,
                    sound = "mySound"
                },
                to = deviceId
            };

            byte[] byteArray = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(a));
            tRequest.ContentLength = byteArray.Length;

            Stream dataStream = tRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse tResponse = tRequest.GetResponse();
            dataStream = tResponse.GetResponseStream();

            StreamReader tReader = new StreamReader(dataStream);
            string sResponseFromServer = tReader.ReadToEnd();

            tReader.Close();
            dataStream.Close();
            tResponse.Close();

            return sResponseFromServer;
        }
    }
}