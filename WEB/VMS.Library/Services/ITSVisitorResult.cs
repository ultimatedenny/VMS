using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface ITSVisitorResult
    {
        //GET DATA
        Visitor_TS GetVisitorFromBadge(string ShimanoBadge);
        List<Visitor_TS> GetVisitorList(string Name);
        List<Temp_VisitorTS> GetTemp_VisitorList(string Name);
        List<TSLogHistory> GetVisitorTSToday(string Name);
        List<TSLogHistory> GetTSReport(string dateFrom, string dateTo);
        List<Temp_VisitorTS> GetTempVisitor(string dateFrom, string dateTo, string name);
        VisitorJoinLog GetorPostVisitorTS(string ShimanoBadge);
        

        //POST DATA
        MessageNonQuery PostRegisterAndAutoVisitLog(string ShimanoBadge, string temp_Id);
        MessageNonQuery PostAutoVisitLog(string Visitor_Id);
        MessageNonQuery PostVisitorLunch(string VisitorId, string NeedLunch);
        MessageNonQuery PostManualVisitLog(string VisitorId, string Temp_Id);
        MessageNonQuery PostVisitorCheckOut(string ShimanoBadge);
        MessageNonQuery PostTempVisitor(string PathFile, string filename);
        Temp_VisitorTS GetTSVisitorTempDet(string Id);
        MessageNonQuery SaveVisitorTemp(Temp_VisitorTS tempvisitor);

    }
}
