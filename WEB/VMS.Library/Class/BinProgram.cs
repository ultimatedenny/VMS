using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Constants;
using VMS.Library.Models;

namespace VMS.Library.Class
{
    class BinProgram
    {
        Database SQLCon = new Database();
        public MessageNonQuery PostAutoVisitLog(string Visitor_Id)
        {
            string query = @"

DECLARE 
@VisitorId nvarchar(5) = '" + Visitor_Id + @"'

INSERT INTO VisitLog_TS
(LogId,TSVisitorId,HostName,HostDepartment,TimeIn,DateVisit,Plant,[Status])
SELECT (SELECT ISNULL(MAX(LogId)+1,1) FROM VisitLog_TS), V.Id, TV.HostName, TV.HostDepartment, CONVERT(TIME, GETDATE(),103), CONVERT(DATE, GETDATE(),103), '2300', 'CHECKIN'  FROM VisitorTS V
INNER JOIN Temp_VisitorTS TV on V.Name = TV.Name 
where CONVERT(DATE, GETDATE(),103)
	between CONVERT(DATE, TV.UploadDate,103) and CONVERT(DATE, DateOfEnd,103) and V.Id=@VisitorId

";
            return SQLCon.executeNonQueryBeginTran(query, ConnectionDB.VMSConnection);

        }
        public MessageNonQuery PostManualVisitLog(string VisitorId, string Temp_Id)
        {
            string query = @"DECLARE 
@VisitorId nvarchar(5) = '" + VisitorId + @"',
@Temp_Id nvarchar (10) = '" + Temp_Id + @"'

INSERT INTO VisitLog_TS
(LogId,TSVisitorId,HostName,HostDepartment,TimeIn,DateVisit,Plant,[Status])
SELECT (SELECT ISNULL(MAX(LogId)+1,1) FROM VisitLog_TS), @VisitorId, HostName, HostDepartment, 
CONVERT(TIME, GETDATE(),103), CONVERT(DATE, GETDATE(),103), '2300', 'CHECKIN' from Temp_VisitorTS
where id=@Temp_Id";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery PostRegisterAndAutoVisitLog(string ShimanoBadge, string temp_Id)
        {
            string query = @"
DECLARE 
@ShimanoBadge nvarchar(30) = '" + ShimanoBadge + @"',
@temp_Id nvarchar(5) = '" + temp_Id + @"'


INSERT INTO VisitorTS
(Id, Name, Department, ShimanoBadge, CreDate, CreUser)
SELECT (SELECT ISNULL(MAX(ID)+1,1) from VisitorTS), Name, 
Department, @ShimanoBadge, GETDATE(), Name from Temp_VisitorTS where Id=@temp_Id

INSERT INTO VisitLog_TS
(LogId,TSVisitorId,HostName,HostDepartment,TimeIn,DateVisit,Plant,[Status])
SELECT (SELECT ISNULL(MAX(LogId)+1,1) FROM VisitLog_TS),(SELECT top 1 Id FROM VisitorTS where shimanoBadge=@ShimanoBadge), HostName,HostDepartment, CONVERT(TIME, GETDATE(),103), 
CONVERT(DATE, GETDATE(),103), '2300', 'CHECKIN'
from Temp_VisitorTS where Id=@temp_Id
";
            return SQLCon.executeNonQueryBeginTran(query, ConnectionDB.VMSConnection);
        }
        public MessageNonQuery PostVisitorLunch(string VisitorId, string NeedLunch)
        {
            string query = @"
DECLARE 
@VisitorId nvarchar(5) = '" + VisitorId + @"',
@NeedLunch nvarchar (10) = '" + NeedLunch + @"'

UPDATE V set NeedLunch = @NeedLunch FROM VisitLog_TS V where TSVisitorId=@VisitorId and CONVERT(DATE, DateVisit,103) = CONVERT(DATE, GETDATE(),103)
And Status='CHECKIN'
";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
    }
}
