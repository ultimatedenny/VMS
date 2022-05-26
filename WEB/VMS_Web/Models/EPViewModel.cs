using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Web.Models
{
    public class EPViewModel
    {
        public System.Guid MasterId { get; set; }
        public int SENo { get; set; }
        public string EPNo { get; set; }
        public string UseDep { get; set; }
        public string PlantID { get; set; }
        public string Destination { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Out { get; set; }
        public System.TimeSpan In { get; set; }
        public bool CompTrans { get; set; }
        public Nullable<System.TimeSpan> CompTransTime { get; set; }
        public string Status { get; set; }
        public System.TimeSpan ActOut { get; set; }
        public System.TimeSpan ActIn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}