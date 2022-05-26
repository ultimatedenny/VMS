using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VMS.Web.Models;

namespace VMS.Web.ViewModels
{
    public class CRUDModels
    {
    }
    public class CRUDUser
    {
        public User OldData { get; set; }
        public User NewData { get; set; }
    }
}