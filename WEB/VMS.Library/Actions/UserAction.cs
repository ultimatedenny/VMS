using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Models;
using VMS.Library.Constants;

namespace VMS.Library.Actions
{
    public class UserAction 
    {
        WindowsAuth winAuth = new WindowsAuth();
        Database SQLCon = new Database();
        public User GetLoginCommon(string username, string password)
        {
            string query = "";
            DataTable dt = new DataTable();
            query = @"SELECT UseID, UseNam, UseDep, u.BusFunc, UseLev, UsePlant, isDelegate, StartPage
                        FROM USR u
						INNER JOIN BusFunc bf on u.BusFunc = bf.BusFunc where UseID='" + username + "' and PWDCOMPARE('" + password + "', [UsePass])=1";
            
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);

            var _data = (from row in dt.AsEnumerable()
                         select new User()
                         {
                             UseID= row["UseID"].ToString(),
                             UseNam = row["UseNam"].ToString(),
                             UseDep = row["UseDep"].ToString(),
                             BusFunc = row["BusFunc"].ToString(),
                             UseLev = row["UseLev"].ToString(),
                             isDelegate = bool.Parse(row["isDelegate"].ToString()),
                             UsePlant = row["UsePlant"].ToString(),
                             StartPage = row["StartPage"].ToString()
                         }).FirstOrDefault();
            return _data;
        }


        public List<User> GetUsers(string Name)
        {
            string query = @"SELECT UseID, UseNam, UseDep from Usr";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _data = (from rw in dt.AsEnumerable()
                         select new User
                         {
                             UseID = rw["UseID"].ToString(),
                             UseNam = rw["UseNam"].ToString() + " - " + rw["UseDep"].ToString(),
                             UseDep = rw["UseDep"].ToString()
                         }).ToList();
            return _data;
        }

        public List<User> GetUserDatatables(string Name)
        {
            string query = @"SELECT UseID, UseNam, UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant from Usr where UseID like '%" + Name + @"%' or UseNam like '%" + Name + @"%'";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                         select new User
                         {
                             UseID = rw["UseID"].ToString(),
                             UseNam = rw["UseNam"].ToString(),
                             UseDep = rw["UseDep"].ToString(),
                             UseLev = rw["UseLev"].ToString(),
                             UseEmail = rw["UseEmail"].ToString(),
                             UseIC = rw["UseIC"].ToString(),
                             UseTel = rw["UseTel"].ToString(),
                             BusFunc = rw["BusFunc"].ToString(),
                             UsePlant = rw["UsePlant"].ToString()
                         }).ToList();
            return _datas;
        }

        public MessageNonQuery PostNewUser(User user, string OldUseID)
        {
            string query = @"
IF EXISTS(SELECT * FROM Usr where UseID = '" + OldUseID + @"')
BEGIN 
UPDATE [Usr]
   SET [UseID] = '" + user.UseID + @"'
      ,[UseNam] = '" + user.UseNam + @"'
      ,[UseDep] = '" + user.UseDep + @"'
      ,[UseLev] = '" + user.UseLev + @"'
      ,[UseEmail] = '" + user.UseEmail + @"'
      ,[UseIC] = '" + user.UseIC + @"'
      ,[UseTel] = '" + user.UseTel + @"'
      ,[BusFunc] = '" + user.BusFunc + @"'
      ,[UsePlant] = '" + user.UsePlant + @"'
      ,[ChgUsr] ='" + user.ChgUser + @"'
      ,[ChgDat] ='" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
 WHERE UseID='" + OldUseID + @"'
 SELECT 'SUCCESS' as isSuccess, 'Update Successfully' as message
END

ELSE
BEGIN
IF EXISTS(SELECT * FROM Usr where UseID='" + user.UseID + @"')
BEGIN
SELECT 'FAIL' as isSuccess, 'Update Failed, Duplicate Data' as message
END

ELSE
BEGIN
INSERT INTO [dbo].[Usr]
           ([Group]
           ,[UseID]
           ,[UseNam]
           ,[UseDep]
           ,[UseLev]
           ,[UseEmail]
           ,[UseIC]
           ,[UseTel]
           ,[BusFunc]
           ,[UsePlant]
           ,[isDelegate]
           ,[CreUsr]
           ,[CreDat])
     VALUES
           ('VisitorMS'
           ,'" + user.UseID + @"'
           ,'" + user.UseNam + @"'
           ,'" + user.UseDep + @"'
           ,'" + user.UseLev + @"'
           ,'" + user.UseEmail + @"'
           ,'" + user.UseIC + @"'
           ,'" + user.UseTel + @"'
           ,'" + user.BusFunc + @"'
           ,'" + user.UsePlant + @"'
           ,'0'
           ,'" + user.CreUser + @"'
           ,'" + DateTime.Now.ToString("yyyy-MM-dd") + @"')
SELECT 'SUCCESS' as isSuccess, 'Insert Successfully' as message
END

END";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }

        public MessageNonQuery PutChangePassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        public List<MenulistDto> GetMenuList(string BusFunc)
        {
            DataTable dt = new DataTable();
            string query = @"select MnuNam, ML.MnuCod, MnuPrt, Pic, FrmNam from Menulist ML 
INNER JOIN LevelMenu LM on ML.MnuCod = LM.MnuCod where busFunc ='" + BusFunc + "' and isView='1' order by MnuSeq";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                         select new MenulistDto
                         {
                             MenuAction = (rw["FrmNam"].ToString() == "")? "" : (rw["FrmNam"].ToString().Split('_'))[1],
                             MenuController = (rw["FrmNam"].ToString() == "") ? "" : (rw["FrmNam"].ToString().Split('_'))[0],
                             MenuCode = rw["MnuCod"].ToString(),
                             MenuIcon = rw["Pic"].ToString(),
                             MenuName = rw["MnuNam"].ToString(),
                             MenuParent = rw["MnuPrt"].ToString()
                         }).ToList();

            return _datas;
        }

        public List<BusFunc> GetBusfunc()
        {
            string query = @"SELECT BusFunc from Busfunc";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new BusFunc
                          {
                              Id = rw["BusFunc"].ToString(),
                              Name = rw["BusFunc"].ToString(),
                          }).ToList();
            return _datas;
        }

        public List<Menulist> GetMenuListEdit(string BusFunc)
        {
            string query = @"select * into #temp from LevelMenu Where busfunc='" + BusFunc + @"'
select MnuNam, mnuseq, ML.MnuCod, MnuPrt, FrmNam, isnull(isView,0) as isView from Menulist ML 
LEFT OUTER JOIN #temp LM on ML.MnuCod = LM.MnuCod Where ML.mnucod !='0000000' order by MnuSeq
drop table #temp";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new Menulist
                          {
                              MenuName = rw["MnuNam"].ToString(),
                              MenuSeq = rw["MnuSeq"].ToString(),
                              MenuCode = rw["MnuCod"].ToString(),
                              MenuParent = rw["MnuPrt"].ToString(),
                              isView = rw["isView"].ToString().ToUpper(),
                          }).ToList();
            return _datas;
        }

        public User GetUserDetail(string UseID)
        {
            string query = @"SELECT UseID, UseNam, UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant from Usr where UseID='"+ UseID + "'";
            DataTable dt = new DataTable();
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new User
                          {
                              UseID = rw["UseID"].ToString(),
                              UseNam = rw["UseNam"].ToString(),
                              UseDep = rw["UseDep"].ToString(),
                              UseLev = rw["UseLev"].ToString(),
                              UseEmail = rw["UseEmail"].ToString(),
                              UseIC = rw["UseIC"].ToString(),
                              UseTel = rw["UseTel"].ToString(),
                              BusFunc = rw["BusFunc"].ToString(),
                              UsePlant = rw["UsePlant"].ToString()
                          }).FirstOrDefault();
            return _datas;
        }

        public DataTable CheckUser(string UseID)
        {
            string query = @"SELECT UseID, UseNam, UseDep, UseLev, UseEmail, UseIC, UseTel, BusFunc, UsePlant from Usr where UseID='" + UseID + "'";
            return (SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection));
        }

        public List<User> GetListApprover()
        {
            string query = $@"SELECT UseID,UseNam,UseTel,UseEmail,isDelegate, ApprovalGroup, UseDep FROM Usr where UseLev in ('1','2') and ApprovalGroup='{Sessions.GetApprovalGroup()}' order by UseLev";
            DataTable dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _datas = (from rw in dt.AsEnumerable()
                          select new User
                          {
                              UseID = rw["UseID"].ToString(),
                              UseNam = rw["UseNam"].ToString(),
                              UseEmail = rw["UseEmail"].ToString(),
                              UseTel = rw["UseTel"].ToString(),
                              isDelegate = (rw["isDelegate"].ToString().ToUpper() == "TRUE"? true : false)


                          }).ToList();
            return _datas;
        }

        public MessageNonQuery ChangeDelegateByRec(string UseID)
        {
            string query = @"UPDATE Usr set isDelegate = case
    when isDelegate = 1 then 0 else 1 end
	where  UseID='"+UseID+ @"'
    
    EXEC [Email_Delegate] '"+UseID+"' ";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
    }

}
