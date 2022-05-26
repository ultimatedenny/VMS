using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Api.Models
{
    public class User
    {
        public string UseID { get; set; }
        public string UseNam { get; set; }
        public string UsePass { get; set; }
        public string UseDep { get; set; }
        public string UseLev { get; set; }
        public string UseEmail { get; set; }
        public string UseComCod { get; set; }
        public string UseIC { get; set; }
        public string UseTel { get; set; }
        public string BusFunc { get; set; }
        public DateTime CreDate { get; set; }
        public string CreUser { get; set; }
        public DateTime ChgDate { get; set; }
        public string ChgUser { get; set; }
        public bool isDelegate { get; set; }
        public string UsePlant { get; set; }
        public string StartPage { get; set; }

    }
}