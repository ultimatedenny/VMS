using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface IUserResult
    {
        //GET
        User GetLoginWinAuth(string username, string password);
        User GetLoginCommon(string username, string password);
        List<User> GetUsers(string Name);
        List<User> GetUserDatatables(string Name);
        List<BusFunc> GetBusfunc();
        List<Menulist> GetMenuListEdit(string BusFunc);
        List<MenulistDto> GetMenuList(string BusFunc);
        List<User> GetListApprover();
        User GetUserDetail(string UseID);
        DataTable CheckUser(string UseID);


        //POST
        MessageNonQuery PutChangePassword(string username, string password);
        MessageNonQuery PostNewUser(User user, string OldUseID);
        MessageNonQuery ChangeDelegateByRec(string UseID);
    }
}
