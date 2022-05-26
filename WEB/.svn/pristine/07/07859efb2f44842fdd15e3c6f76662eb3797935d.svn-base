using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Library.Models
{


    public enum Status
    {
        PENDING = 1,
        APPROVED = 2,
        REJECTED = 3,
        CHECKIN = 4,
        BREAK = 5,
        CHECKOUT = 6,
        DELETE = 7

    }
    public enum Method
    {
        Security = 1,
        User = 2
    }

    class MasterDataCommon
    {
    }
    public class Plant
    {
        public int plantId { get; set; }
        public string plantName { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class VisitType
    {
        public int Id { get; set; }
        public string VisitorType { get; set; }
        public string NeedApprove { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class AreaVisitor
    {
        public int Plant { get; set; }
        public string PlantName { get; set; }
        public string areaGroupId { get; set; }
        public string areaId { get; set; }
        public string areaName { get; set; }
        public int VisitorType { get; set; }
        public string VisitorTypeName { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }

    }
    public class SqlVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class HostAppointment
    {
        public string LogId { get; set; }
        public string UseID { get; set; }
        public string UseNam { get; set; }
        public string UseTel { get; set; }
        public string UseDep { get; set; }
        public string Area { get; set; }
        public string VisitorType { get; set;}
        public string Status { get; set; }
        public string Remark { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string NeedApprove { get; set; }
    }
    public class VisitLogArea
    {
        public string LogId { get; set; }
        public string AreaId { get; set; }

    }
    public class Chart
    {
        public int Data { get; set; }
        public string Name { get; set; }
    }

    public class MenulistDto
    {
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string MenuAction { get; set; }
        public string MenuParent { get; set; }
        public string MenuController { get; set; }
        public string MenuIcon { get; set; }
    }
    public class BusFunc
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Menulist
    {
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string MenuAction { get; set; }
        public string MenuParent { get; set; }
        public string MenuController { get; set; }
        public string MenuIcon { get; set; }
        public string MenuSeq { get; set; }
        public string isView { get; set; }
    }
    public class LogBook
    {
        public int LogId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string InfoGrant { get; set; }
        public string NameGrant { get; set; }
        public string DateGrant { get; set; }
        public string TimeGrant { get; set; }
        public string PhotoGrant { get; set; }
        public string NameReceive { get; set; }
        public string DateReceive { get; set; }
        public string TimeReceive { get; set; }
        public string PhotoReceive { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Plant { get; set; }
    }
    public class Department
    {
        public string Plant { get; set; }
        public string Dept { get; set; }
        public string DeptName { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateAt { get; set; }
    }
    public class CodLst
    {
        public string GrpCod { get; set; }
        public string Cod { get; set; }
        public string CodAbb { get; set; }
        public string CodDesc { get; set; }
        
    }
    public class PhotoLogBook
    {
        public string PhotoItem { get; set; }
        public string PhotoUser { get; set; }

    }


}
