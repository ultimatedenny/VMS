using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VMS.Web.Models;

namespace VMS.Api.Controllers
{
    public class VisitLogController : ApiController
    {
        VisitLogAction _visitLog = new VisitLogAction();
        [HttpPost]
        public IEnumerable<LogHistory> ShowVisitorToday(string Name)
        {
            return _visitLog.ShowVisitorToday(Name);
        }
        
        
    }
}
