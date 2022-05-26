using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Library.Constants
{
    public class ConnectionDB
    {
        public const string VMSConnection = "VMSConnection";
        public const string MDMConnection = "MDMConnection";
    }
    public class StoreProc
    {
        public const string ApprovalAction = "ApprovalAction";
        public const string VisitorCheckIn = "VisitorCheckIn";
        public const string InsertAppointment = "InsertAppointment";
        public const string ChangeStatusReq = "ChangeStatusReq";
        public const string GetSummaryVisitorPerYear = "GetSummaryVisitorPerYear";
        public const string GetSummaryVisitorPerMonth = "GetSummaryVisitorPerMonth";
        
    }
    public class cGrpCodLst
    {
        public const string WifiConfig_1 = "WifiConfig_1";
    }
    public class cWifiSetting
    {
        public const string SSID = "SSID";
        public const string Password = "Password";
        public const string isNeedPassword = "isNeedPassword";
        public const string Username = "Username";
    }
    public class cAppSetting
    {
        public const string PathErrLog = "PathErrLog";
    }
}
