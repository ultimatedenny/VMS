using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Library.Models
{
    public class TeamShimano
    {
    }
    public class Visitor_TS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string Department { get; set; }
        public string ShimanoBadge { get; set; }
        public string Plant { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Photoes { get; set; }
        public string CreUser { get; set; }
        public string CreDate { get; set; }
        public string ChgUser { get; set; }
        public string ChgDate { get; set; }
    }
    public class VisitLog_TS
    {
        public int LogId { get; set; }
        public int TSVisitorId { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string Plant { get; set; }
        public string DateVisit { get; set; }
        public string NeedLunch { get; set; }
        public string Status { get; set; }
    }
    public class Temp_VisitorTS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string EmployeeNo { get; set; }
        public string Plant { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string DateEnd { get; set; }
        public string UploadDate { get; set; }
        public DateTime DateManual { get; set; }

    }
    public class Template_UploadVsitor
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string Plant { get; set; }
        public DateTime DateofTravel { get; set; }
        public DateTime DateofEnd { get; set; }

    }
    public class VisitorJoinLog
    {
        public string VisitorId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Temp_VisitorId { get; set; }
        public string Status { get; set; }

    }
    public class ShimanoBadgeModel
    {
        public string ShimanoBadge { get; set; }
        public string Temp_Visitor { get; set; }
        public bool NeedLunch { get; set; }
        public string PhotoName { get; set; }
    }

    public class ShimanoWIFI
    {
        public string Host { get; set; }
        public string Visitor { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CreBy { get; set; }
    }

    public class PhotoAndroid {
        public byte[] photoData { get; set; }
        public string photoName { get; set; }
    }
    public class WifiAccount
    {
        public string WifiName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
