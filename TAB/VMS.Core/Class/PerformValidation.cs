using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Core.Class
{
    public class PerformValidation
    {

    }
    public enum SttVisitor{
        PENDING = 1,
        NOTREGISTER,
        CHECKIN,
        CHECKOUT,
    }
    public enum gVarEnum
    {
        Id,
        Name,
        Plant,
        Status,
        Department,
        CardId,
        Temp_VisitorId,
        Lunch,
        Photo,
        Stay,
        StayDate
    }
    public enum welcome
    {
        SignInWelcome,
        SignInSubWelcome,
        SignOutWelcome,
        SignOutSubWelcome
    }
}
