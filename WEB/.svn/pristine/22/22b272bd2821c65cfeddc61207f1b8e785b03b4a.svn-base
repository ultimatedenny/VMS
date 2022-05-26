using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface IApprovalResult
    {
        MessageNonQuery CheckAuthorize(string UsePlant, string isDelegate);
        MessageNonQuery CheckDelegate(string Username);
        MessageNonQuery ChangeDelegate(string isDelegate, string Uselev);
        List<LogHistory> ShowRequestApproval(string Plant);
        MessageNonQuery ApproveAction(string Username, string LogId, string Status, string ApprovedRemark = "");
        List<LogHistory> ShowHistoryApproval(string Username);

    }
}
