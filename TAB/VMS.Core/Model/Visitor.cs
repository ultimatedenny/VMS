using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Core.Model
{
    public class Visitor
    {
        public int Id { get; set; }
        public string VisitorCardNo { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Company { get; set; }
        public string JobDesc { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class WifiAccount
    {
        public string WifiName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
