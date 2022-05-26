using System.Web.Http;
using VMS.Web.Models;

namespace VMS.Web.Controllers.Api
{
    [RoutePrefix("api/User")]
    public class UserApiController : ApiController
    {
        UserAction userAct = new UserAction();
        //GET /api/userApi
        //public User GetUser(string username, string password)
        //{
        //    bool isWinAuth = true;
        //    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        //    {
        //        return null;
        //    }
        //    if (isWinAuth == true)
        //    {
        //        return userAct.GetLoginWinAuth(username, password);
        //    }
        //    else
        //    {
        //        return userAct.GetLoginCommon(username, password);
        //    }
        //}

    }
}
