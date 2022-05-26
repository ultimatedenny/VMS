using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Api.Models
{
    public static class Global
    {
        public static  MessageNonQuery NonQueryVMS (this long num)
        {
            return new MessageNonQuery
            {
                isSuccess = num > 0 ? true : false,
                message = num > 0 ? num.ToString() +  "Row(s) Affected" : "No data Affected"
            };

        }
    }
}