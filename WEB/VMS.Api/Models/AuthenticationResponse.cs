using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VMS.Api.Models
{
    public class AuthenticationResponse
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
    }
}