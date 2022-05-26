using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using VMS.Library;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using static VMS.Web.Controllers.MasterDataController;
using System.Configuration;

namespace VMS.Web.Models
{
    public class MasterDataAction
    {
        DataTable dt;
        long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public VMSRes<string> ChangeStatusMasterData(string TableName, string ColumnName, string id, string Username)
        {
            string query = @"
                IF EXISTS(SELECT 1 FROM " + TableName + @" WHERE " + ColumnName + @" ='" + id + @"')
                BEGIN
                    UPDATE " + TableName + @" set isActive = case
                        when isActive = 1 then 0 else 1 end
	                    where  " + ColumnName + @" = '" + id + @"'
                END";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }

        //DataTables
        public List<AreaVisitor> GetAreaDatatables(string AreaName)
        {
            string query = @"SELECT AV.Plant,PlantName, areaId, areaName, AV.VisitorType, VT.VisitorType as VisitorTypeName,IsDisclaimer=ISNULL(IsDisclaimer,0)
                            from AreaVisitor AV
                            INNER JOIN Plant P on AV.Plant = P.plantId
                            INNER JOIN VisitorType VT on AV.VisitorType = VT.id where AV.Plant like '%" + AreaName + @"%' or AV.areaId like '%" + AreaName + @"%' or AV.AreaName like '%" + AreaName + @"%' or P.plantName like '%" + AreaName + @"%'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    Plant = int.Parse(rw["Plant"].ToString()),
                                    PlantName = rw["PlantName"].ToString(),
                                    VisitorType = int.Parse(rw["VisitorType"].ToString()),
                                    VisitorTypeName = rw["VisitorTypeName"].ToString(),
                                    areaId = rw["areaId"].ToString(),
                                    areaName = rw["areaName"].ToString(),
                                    isDisclaimer = rw["IsDisclaimer"].ToBoolean(),
                                }).ToList();
            return _visitorArea;
        }
       public List<mmApprovalLevel> GetmmApprovalLevel(string VisitorType, string Plant, string Department)
        {
            string query = @"DECLARE
	                            @VisitorType NVARCHAR(100)= '"+ VisitorType + @"'
	                            ,@Plant NVARCHAR(100) = '" + Plant + @"'
	                            ,@Department NVARCHAR(100) = '" + Department + @"'
	                            IF @VisitorType = 'All' or @VisitorType = '' SET @VisitorType ='%'
	                            IF @Plant = 'All' or @Plant = '' SET @Plant ='%'
	                            IF @Department = 'All' or @Department = '' SET @Department ='%'
                            SELECT min(id) As ID,Plant, VisitorType, Department, ChangeUser As UpdateBy, MIN(ChangeDate) As UpdateAt
                            FROM mmApprovalLevel
                            WHERE 
	                            VisitorType LIKE '%'+ @VisitorType + '%'
	                            AND Plant LIKE '%'+ @Plant + '%'
	                            AND Department LIKE '%'+ @Department + '%'
                            GROUP BY Plant, VisitorType, Department, ChangeUser
                            ORDER BY Plant, VisitorType, Department";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                select new mmApprovalLevel
                                {
                                    ID = int.Parse(rw["ID"].ToString().ToUpper()),
                                    Plant = rw["Plant"].ToString().ToUpper(),
                                    VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                    Department = rw["Department"].ToString().ToUpper(),
                                    ChangeUser = rw["UpdateBy"].ToString().ToUpper(),
                                    ChangeDate = rw["UpdateAt"].ToString().ToUpper()
                                }).ToList();
            return _mmApprovalLevel;
        }
        public List<mmDepartment> GetmmDepartment(string Plant)
        {
            string query = @"DECLARE
	                            @Plant nvarchar(10) = '" + Plant + @"'
                            SELECT
	                            Plant
	                            ,Dept
	                            ,DeptName
	                            ,UpdateBy
	                            ,UpdateAt
                            FROM Dept
                            WHERE Plant = @Plant
                            ORDER BY Plant, Dept";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new mmDepartment
                                    {
                                        Plant = rw["Plant"].ToString().ToUpper(),
                                        Dept = rw["Dept"].ToString().ToUpper(),
                                        DeptName = rw["DeptName"].ToString().ToUpper(),
                                        UpdateBy = rw["UpdateBy"].ToString().ToUpper(),
                                        UpdateAt = rw["UpdateAt"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<mmApprovalLevel> ViewApproverAppointment(string Plant, string Department, string VisitorType)
        {
            string query = @"DECLARE
	                            @Plant varchar(10) = '" + Plant + @"'
	                            ,@Department varchar(50) = '" + Department + @"'
	                            ,@VisitorType varchar(50) = '" + VisitorType + @"'
                            SELECT
	                            a.ApprovalLevel
	                            ,b.UseNam As Approval
	                            ,a.VisitorType
	                            ,c.CodAbb As Category
	                            ,a.Department
                            FROM mmApprovalLevel a JOIN Usr b ON a.WindowsID = b.UseID JOIN CodLst c ON a.Category = c.Cod
                            WHERE a.plant = @Plant
                            AND a.VisitorType = @VisitorType
                            AND a.Department = @Department
                            AND c.GrpCod = 'ApprovalCategory'
                            ORDER BY a.ApprovalLevel";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new mmApprovalLevel
                                    {
                                        ApprovalLevel = rw["ApprovalLevel"].ToString().ToUpper(),
                                        Approval = rw["Approval"].ToString().ToUpper(),
                                        VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                        Category = rw["Category"].ToString().ToUpper(),
                                        Department = rw["Department"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<mmApprovalLevel> ViewApproverAppointmentRecord(string Plant, string Department, string VisitorType)
        {
            string query = @"DECLARE
	                            @Plant varchar(10) = '" + Plant + @"'
	                            ,@Department varchar(50) = '" + Department + @"'
	                            ,@VisitorType varchar(50) = '" + VisitorType + @"'
                                ,@Result INT 
							SELECT @Result = NeedApprove FROM VisitorType WHERE VisitorType = @VisitorType
							IF (@Result = 0)
							BEGIN
								SET @Result = 1
								SELECT @Result As Result
							END
							ELSE
							BEGIN
								SELECT COUNT(*) As Result FROM mmApprovalLevel
								WHERE Plant = @Plant
								AND Department = @Department
								AND VisitorType = @VisitorType
								AND ApprovalLevel = 1
								AND Category NOT IN (2,3)
							END";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new mmApprovalLevel
                                    {
                                        ApprovalLevel = rw["Result"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<TmpmmApprovalLevel> GetmmApprovalLevelConfirm(string user)
        {
            string query = @"DECLARE
	                            @User NVARCHAR(50)='"+ user + @"'

                            SELECT Plant,VisitorType,Department, CASE WHEN SUM(CASE WHEN UploadStatus<>'OK' then 1 else 0 end) >0 then 'FAILED' ELSE 'OK' END As UploadStatus
                            FROM TmpmmApprovalLevel where UploadUser=@User
                            GROUP BY Plant,VisitorType,Department
                            ORDER BY CASE WHEN SUM(CASE WHEN UploadStatus<>'OK' then 1 else 0 end) >0 then 'FAILED' ELSE 'OK' END,plant,department, VisitorType";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new TmpmmApprovalLevel
                                    {
                                        Plant = rw["Plant"].ToString().ToUpper(),
                                        VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                        Department = rw["Department"].ToString().ToUpper(),
                                        UploadStatus = rw["UploadStatus"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<TmpmmAutoApproval> GetmmAutoApprovalConfirm(string user)
        {
            string query = @"DECLARE
	                            @User NVARCHAR(50)='" + user + @"'

                            SELECT Plant, Department, WindowsID, [Status], Remark FROM Tmp_mmAutoApproval
                            WHERE UploadUser = @User
                            GROUP BY Plant,Department, WindowsID, [Status], Remark
                            ORDER BY CASE WHEN SUM(CASE WHEN [Status]<>'OK' then 1 else 0 end) >0 then 'FAILED' ELSE 'OK' END,plant,department, WindowsID";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new TmpmmAutoApproval
                                    {
                                        Plant = rw["Plant"].ToString().ToUpper(),
                                        Department = rw["Department"].ToString().ToUpper(),
                                        WindowsID = rw["WindowsID"].ToString().ToUpper(),
                                        Status = rw["Status"].ToString().ToUpper(),
                                        Remark = rw["Remark"].ToString().ToUpper(),
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<mmAutoApproval> GetmmAutoApproval(string Plant, string Department)
        {
            double Number = 1;
            string query = @"DECLARE
	                            @Plant varchar(4) = '"+ Plant + @"'
	                            ,@Department varchar(50) = '" + Department + @"'
	                            IF @Plant = '' Or @Plant = 'All' SET @Plant = '%'
	                            IF @Department = '' Or @Department = 'All' SET @Department = '%'
                            SELECT 
	                            ROW_NUMBER() OVER(ORDER BY id) AS [No]
                                ,a.id
	                            ,a.Plant
	                            ,a.Department
	                            ,a.WindowsID
	                            ,b.UseNam As UserName
	                            ,a.ChangeUser As UpdateBy
	                            ,a.ChangeDate As UpdateDate
                            FROM mmAutoApproval a join Usr b on a.WindowsID = b.UseID
                            WHERE Plant LIKE '%'+ @Plant +'%'
                            AND Department LIKE '%' + @Department + '%'
                            ORDER BY id";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new mmAutoApproval
                                   {
                                        Number = Number++,
                                        ID = int.Parse(rw["id"].ToString().ToUpper()),
                                        Plant = rw["Plant"].ToString(),
                                        Department = rw["Department"].ToString().ToUpper(),
                                        WindowsID = rw["WindowsID"].ToString().ToUpper(),
                                        UserName = rw["UserName"].ToString().ToUpper(),
                                        ChangeUser = rw["UpdateBy"].ToString().ToUpper(),
                                        ChangeDate = rw["UpdateDate"].ToString().ToUpper()
                                   }).ToList();
            return _mmAutoApproval;
        }
        public List<mmApprovalLevel> GetViewApproverLevel(string Plant, string Department, string VisitorType)
        {
            string query = @"DECLARE
	                            @Plant varchar(20) = '"+ Plant + @"'
	                            ,@VisitorType varchar(50) = '" + VisitorType + @"'
	                            ,@Department varchar(50) = '" + Department + @"'

                            SELECT
	                             UPPER(a.ApprovalLevel) as ApprovalLevel
	                            ,UPPER(b.UseNam) As Approval
	                            ,UPPER(a.VisitorType) as VisitorType
	                            ,UPPER(a.Category) as Category
	                            ,UPPER(a.Department) as Department
                            FROM mmApprovalLevel a join Usr b on a.WindowsID = b.UseID
                            WHERE a.Plant like @Plant
                            AND a.Department like @Department
                            AND a.VisitorType like @VisitorType
                            ORDER BY a.ApprovalLevel";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new mmApprovalLevel
                                   {
                                       VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                       Department = rw["Department"].ToString().ToUpper(),
                                       ApprovalLevel = rw["ApprovalLevel"].ToString().ToUpper(),
                                       Approval = rw["Approval"].ToString().ToUpper(),
                                       ChangeUser = rw["UpdateBy"].ToString().ToUpper(),
                                       ChangeDate = rw["UpdateDate"].ToString().ToUpper()
                                   }).ToList();
            return _mmAutoApproval;
        }
        public List<mmApprovalLevel> GetmmApprovalLevelDetail(string VisitorType, string Plant, string Department)
        {
            string query = @"DECLARE
	                            @Plant nvarchar(100) = '"+ Plant + @"'
	                            ,@VisitorType nvarchar(100) = '" + VisitorType + @"'
	                            ,@Department nvarchar(100) = '" + Department + @"'
	                            IF @VisitorType = 'All' or @VisitorType = '' SET @VisitorType ='%'
	                            IF @Plant = 'All' or @Plant = '' SET @Plant ='%'
	                            IF @Department = 'All' or @Department = '' SET @Department ='%'
                            SELECT
                                 UPPER(a.ID) as ID
	                            ,UPPER(a.ApprovalLevel) as ApprovalLevel
	                            ,UPPER(a.WindowsID) as WindowsID
	                            ,UPPER(b.UseNam) As Approval
	                            ,UPPER(c.CodAbb) As Category
	                            ,UPPER(a.Remarks) as Remarks
	                            ,UPPER(a.Department) as Department
	                            ,UPPER(a.Plant) as Plant
	                            ,UPPER(a.VisitorType) as VisitorType
                            FROM mmApprovalLevel a JOIN Usr b on a.WindowsID = b.UseID JOIN CodLst c ON a.Category = c.Cod
                            WHERE c.GrpCod = 'ApprovalCategory'
                            AND a.Plant LIKE @Plant
                            AND a.VisitorType LIKE @VisitorType
                            AND a.Department LIKE @Department
                            ORDER BY a.ApprovalLevel";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new mmApprovalLevel
                                    {
                                        ID = int.Parse(rw["ID"].ToString().ToUpper()),
                                        ApprovalLevel = rw["ApprovalLevel"].ToString().ToUpper(),
                                        WindowsID = rw["WindowsID"].ToString().ToUpper(),
                                        Approval = rw["Approval"].ToString().ToUpper(),
                                        Category = rw["Category"].ToString().ToUpper(),
                                        Remarks = rw["Remarks"].ToString().ToUpper(),
                                        Department = rw["Department"].ToString().ToUpper(),
                                        Plant = rw["Plant"].ToString().ToUpper(),
                                        VisitorType = rw["VisitorType"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<TmpmmApprovalLevel> GetmmApprovalLevelDetailConfirm(string VisitorType, string Plant, string Department, string user)
        {
            string query = @"DECLARE
	                            @User NVARCHAR(50) = '" + user + @"'
	                            ,@Plant VARCHAR(10) = '" + Plant + @"'
	                            ,@VisitorType NVARCHAR(50) = '" + VisitorType + @"'
	                            ,@Department NVARCHAR(50) = '" + Department + @"'
                            SELECT ApprovalLevel
	                            , WindowsID
	                            , Category
	                            , Remarks
	                            , UploadStatus
                                , UploadUser
                                , UploadDate
                            FROM TmpmmApprovalLevel 
                            WHERE UploadUser=@User 
	                            AND Plant =@Plant 
	                            AND VisitorType = @VisitorType 
	                            AND Department = @Department";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmApprovalLevel = (from rw in dt.AsEnumerable()
                                    select new TmpmmApprovalLevel
                                    {
                                        ApprovalLevel = rw["ApprovalLevel"].ToString().ToUpper(),
                                        WindowsID = rw["WindowsID"].ToString().ToUpper(),
                                        Category = rw["Category"].ToString().ToUpper(),
                                        Remarks = rw["Remarks"].ToString().ToUpper(),
                                        UploadStatus = rw["UploadStatus"].ToString().ToUpper(),
                                        UploadUser = rw["UploadUser"].ToString().ToUpper(),
                                        UploadDate = rw["UploadDate"].ToString().ToUpper()
                                    }).ToList();
            return _mmApprovalLevel;
        }
        public List<VisitType> GetVisitorTypeDatatables()
        {
            string query = @"SELECT [Id]
              ,[VisitorType]
              ,[NeedApprove] 
              ,[NeedMailAlert]
              ,[AllowUnknown]
              ,[IsView]
            FROM [VisitorType]";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = int.Parse(rw["Id"].ToString().ToUpper()),
                                    VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                    NeedApprove = rw["NeedApprove"].ToString().ToUpper(),
                                    NeedMailAlert = rw["NeedMailAlert"].ToString().ToUpper(),
                                    AllowUnknown = rw["AllowUnknown"].ToString().ToUpper(),
                                    IsView = rw["IsView"].ToString().ToUpper(),
                                }).ToList();
            return _visitorType;
        }
        public List<Visitor> GetAllowUnknown(string VisitorType)
        {
            string query = @"DECLARE
	                            @VisitorType varchar(50) = '"+ VisitorType +@"'
	
                            SELECT AllowUnknown FROM VisitorType WHERE VisitorType = @VisitorType";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new Visitor
                                   {
                                       Unknown = Convert.ToBoolean(rw["AllowUnknown"].ToString().ToUpper())
                                   }).ToList();
            return _mmAutoApproval;
        }
        public List<Visitor> GetLogo()
        {
            string query = @"SELECT CodDes As SystemName FROM CodLst WHERE Cod = 'SystemName'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new Visitor
                                   {
                                       Company = rw["SystemName"].ToString().ToUpper()
                                   }).ToList();
            return _mmAutoApproval;
        }
        public List<Visitor> GetResultNotOK(string user)
        {
            string query = @"DECLARE
	                            @User VARCHAR(50) = '" + user + @"'
                            SELECT COUNT(*) As Result FROM TmpmmApprovalLevel
                            WHERE UploadUser = @User AND UploadStatus != 'OK' ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new Visitor
                                   {
                                       id = Convert.ToInt32(rw["Result"].ToString().ToUpper())
                                   }).ToList();
            return _mmAutoApproval;
        }
        public List<Visitor> CheckVisitorTypeName(string visitortype)
        {
            string query = @"DECLARE
	                            @VisitorType VARCHAR(50) = '" + visitortype + @"'

                            IF EXISTS (SELECT * FROM VisitorType
                            WHERE VisitorType = @VisitorType)
                            BEGIN
	                            SELECT '1' As Result
                            END
                            ELSE
                            BEGIN 
	                            SELECT '0' As Result
                            END ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new Visitor
                                   {
                                       id = Convert.ToInt32(rw["Result"].ToString())
                                   }).ToList();
            return _mmAutoApproval;
        }
        public AreaVisitor GetAreaDetail(string AreaId, string VisitorType)
        {
            string query = @"SELECT Plant, areaId, areaName, AV.VisitorType
            from AreaVisitor AV
            INNER JOIN Plant P on AV.Plant = P.plantId
            INNER JOIN VisitorType VT on AV.VisitorType = VT.id
            WHERE areaId='" + AreaId + @"' and AV.VisitorType='" + VisitorType + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    Plant = int.Parse(rw["Plant"].ToString().ToUpper()),
                                    VisitorType = int.Parse(rw["VisitorType"].ToString().ToUpper()),
                                    areaId = rw["areaId"].ToString().ToUpper(),
                                    areaName = rw["areaName"].ToString().ToUpper(),
                                }).FirstOrDefault();
            return _visitorArea;
        }
        //public ExitPermit2 GetExitPermitDetail(string Id)
        //{
        //    string query = @"SELECT [Id], EPNo, U.UseDep as UserDep, U.UseID as UserID, U.UseNam as Username, P.PlantName as PlantName, Destination, EP.[Date] as PermitDate, EP.[Out] as PermitTimeOut, EP.[In] as PermitTimeIn, ActOut, ActIn, CompTrans
        //                    from ExitPermit EP
        //                    INNER JOIN Plant P on EP.plantID = P.plantId
        //                    INNER JOIN Usr U on EP.UseID = U.UseID where Id like '%" + Id + @"%'";
        //    using (var sql = new MSSQL())
        //    {
        //        dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
        //    }
        //    var _ExitPermit = (from rw in dt.AsEnumerable()
        //                       select new ExitPermit2
        //                       {
        //                           Id = int.Parse(rw["Id"].ToString()),
        //                           EPNo = rw["EPNo"].ToString(),
        //                           UseDep = rw["UserDep"].ToString(),
        //                           UseID = rw["UserID"].ToString(),
        //                           Username = rw["Username"].ToString(),
        //                           plantID = rw["PlantName"].ToString(),
        //                           Destination = rw["Destination"].ToString(),
        //                           Date = Convert.ToDateTime(rw["PermitDate"].ToString()).ToString("yyyy-MM-dd"),
        //                           In = TimeSpan.Parse(rw["PermitTimeIn"].ToString()).ToString(),
        //                           Out = TimeSpan.Parse(rw["PermitTimeOut"].ToString()).ToString(),
        //                           CompTrans = bool.Parse(rw["CompTrans"].ToString()).ToString(),
        //                           ActIn = TimeSpan.Parse(rw["ActIn"].ToString()).ToString(),
        //                           ActOut = TimeSpan.Parse(rw["ActOut"].ToString()).ToString(),
        //                       }).FirstOrDefault();
        //    return _ExitPermit;
        //}
        public VisitType GetVisitorTypeDetail(string Id)
        {
            string query = @"SELECT [Id]
      ,[VisitorType]
      ,[NeedApprove] 
              ,[NeedMailAlert]
              ,[AllowUnknown]
              ,[IsView] FROM [VisitorType] where Id='" + Id + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = int.Parse(rw["Id"].ToString().ToUpper()),
                                    VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                    NeedApprove = rw["NeedApprove"].ToString().ToUpper(),
                                    NeedMailAlert = rw["NeedMailAlert"].ToString().ToUpper(),
                                    AllowUnknown = rw["AllowUnknown"].ToString().ToUpper(),
                                    IsView = rw["IsView"].ToString().ToUpper(),
                                }).FirstOrDefault();
            return _visitorType;
        }
        public Department GetDeptDetail(string Plant, string Department)
        {
            string query = @"DECLARE
		                            @Plant VARCHAR(10) = '" + Plant + @"'
		                            ,@Dept NVARCHAR(50) = N'" + Department + @"'
                            SELECT 
	                            Plant,
	                            Dept,
	                            DeptName
                            FROM Dept
                            WHERE Plant = @Plant AND Dept = @Dept";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _Dept = (from rw in dt.AsEnumerable()
                                select new Department
                                {
                                    Plant = rw["Plant"].ToString().ToUpper(),
                                    Dept = rw["Dept"].ToString().ToUpper(),
                                    DeptName = rw["DeptName"].ToString().ToUpper(),
                                }).FirstOrDefault();
            return _Dept;
        }
        //Save
        public VMSRes<string> SaveArea(AreaVisitor _area, string oldArea, string oldVisitorType)
        {
            string query = @"
            IF EXISTS(SELECT * FROM AreaVisitor where areaId='" + oldArea + "' and VisitorType='" + oldVisitorType + @"')
            BEGIN
            Update AreaVisitor set 
            Plant='" + _area.Plant.ToString().ToUpper() + @"', 
            areaId='" + _area.areaId.ToString().ToUpper() + @"', 
            areaName='" + _area.areaName.ToString().ToUpper() + @"', 
            VisitorType='" + _area.VisitorType.ToString().ToUpper() + @"', 
            UpdateBy='" + _area.UpdateBy.ToString().ToUpper() + @"', 
            UpdateAt='" + DateTime.Now.ToString("yyyy-MM-dd") + @"' where areaId='" + oldArea.ToString().ToUpper() + @"' and VisitorType = '" + oldVisitorType.ToString().ToUpper() + @"' 
            END
                    ELSE
            BEGIN
            INSERT INTO AreaVisitor(Plant, AreaId, AreaName, VisitorType, UpdateAt, UpdateBy)
            VALUES('" + _area.Plant.ToString().ToUpper() + @"','" + _area.areaId.ToString().ToUpper() + @"','" + _area.areaName.ToString().ToUpper() + @"','" + _area.VisitorType.ToString().ToUpper() + @"','" + DateTime.Now.ToString("yyyy-MM-dd") + @"','" + _area.UpdateBy.ToString().ToUpper() + @"')
            END";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        //public VMSRes<string> SaveExitPermit(ExitPermit2 ExitPermit)
        //{
        //    string query = @"
        //    IF EXISTS(SELECT * FROM ExitPermit where Id='" + ExitPermit.Id + @"')
        //    BEGIN
        //    UPDATE [dbo].[ExitPermit]
        //       SET [UseDep] = '" + ExitPermit.UseDep + @"'
        //          ,[UseID] = '" + ExitPermit.UseID + @"'
        //          ,[plantID] = '" + ExitPermit.plantID + @"'
        //          ,[Destination] = '" + ExitPermit.Destination + @"'
        //          ,[Date] = '" + ExitPermit.Date + @"'
        //          ,[Out] = '" + ExitPermit.Out + @"'
        //          ,[ActOut] = '" + ExitPermit.ActOut + @"'
        //          ,[In] = '" + ExitPermit.In + @"'
        //          ,[ActIn] = '" + ExitPermit.ActIn + @"'
        //          ,[CompTrans] = '" + ExitPermit.CompTrans + @"'
        //    WHERE Id='" + ExitPermit.Id + @"'
        //    END
        //    ELSE
        //    BEGIN
        //    DECLARE 
        //    @maxId int
        //    set @MaxId = (select isnull (max(cast(id as int))+1,1) from ExitPermit)
        //    INSERT INTO [dbo].[ExitPermit]
        //               ([Id]
        //,[EPNo]
        //,[UseDep]
        //               ,[UseID]
        //               ,[plantID]
        //               ,[Destination]
        //,[Date]
        //,[Out]
        //               ,[ActOut]
        //,[In]
        //               ,[ActIn]
        //               ,[CompTrans]
        //,[CreatedBy]
        //,[CreatedDate]
        //)
        //         VALUES
        //               (
        //                   @MaxId
        //                   ,'EP'+ CONVERT([NVARCHAR](50),FORMAT(GETDATE(), 'yyMMdd')) + '-' + REPLICATE('0',(3) - LEN(CONVERT([NVARCHAR](50),@MaxId)))+ CONVERT([NVARCHAR](50),@MaxId)
        //                   ,'" + ExitPermit.UseDep + @"'
        //                   ,'" + ExitPermit.UseID + @"'
        //                   ,'" + ExitPermit.plantID + @"'
        //                   ,'" + ExitPermit.Destination + @"'
        //                   ,'" + ExitPermit.Date + @"'
        // ,'" + ExitPermit.Out + @"'
        //                   ,'" + ExitPermit.ActOut + @"'
        //                   ,'" + ExitPermit.In + @"'
        //                   ,'" + ExitPermit.ActIn + @"'
        //                   ,'" + ExitPermit.CompTrans + @"'
        //                   ,'" + ExitPermit.CreatedBy + @"'
        //                   ,'" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
        //               )
        //    END
        //    ";
        //    using (MSSQL sql = new MSSQL())
        //    {
        //        num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
        //    }
        //    return num.NonQueryResults();
        //}
        //public VMSRes<string> SaveVisitorType(VisitType VisitType)
        //{
        //    string query = @"
        //    IF EXISTS(SELECT * FROM VISITORTYPE where Id='" + VisitType.Id + @"')
        //    BEGIN
        //    UPDATE [dbo].[VisitorType]
        //       SET [VisitorType] = '" + VisitType.VisitorType + @"'
        //          ,[NeedApprove] = '" + VisitType.NeedApprove + @"'
        //          ,[NeedMailAlert] = '" + VisitType.NeedMailAlert + @"'
        //          ,[AllowUnknown] = '" + VisitType.AllowUnknown + @"'
        //          ,[IsView] = '" + VisitType.IsView + @"'
        //          ,[UpdateAt] = '" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
        //          ,[UpdateOn] = '" + VisitType.UpdateBy + @"'
        //     WHERE Id='" + VisitType.Id + @"'
        //    END
        //    ELSE
        //    BEGIN
        //    DECLARE 
        //    @maxId int
        //    set @MaxId = (select isnull (max(cast(id as int))+1,1) from VisitorType)
        //    INSERT INTO [dbo].[VisitorType]
        //               ([Id]
        //               ,[VisitorType]
        //               ,[NeedApprove]
        //               ,[NeedMailAlert]
        //               ,[AllowUnknown]
        //               ,[IsView]
        //               ,[UpdateAt]
        //               ,[UpdateOn])
        //         VALUES
        //    (
        //               @MaxId
        //               ,'" + VisitType.VisitorType + @"'
        //               ,'" + VisitType.NeedApprove + @"'
        //               ,'" + VisitType.NeedMailAlert + @"'
        //               ,'" + VisitType.AllowUnknown + @"'
        //               ,'" + VisitType.IsView + @"'
        //               ,'" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
        //               ,'" + VisitType.UpdateBy + @"')
        //    END
        //    ";
        //    using (var sql = new MSSQL())
        //    {
        //        num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
        //    }
        //    return num.NonQueryResults();
        //}
        //add by suharjo ng 2021-11-15
        public VMSRes<string> SaveVisitorType(string ID, string VisitorType, string NeedApprove, string NeedMailAlert, string AllowUnknown, string IsView, string UpdateOn)
        {
            try
            {
                //string query = $@"Update Usr Set DelegateUseID = '{DelegateUseID}'
                //            ,DelegateEffectiveFrom = '{DelegateEffectiveFrom}',DelegateEffectiveTo = '{DelegateEffectiveTo}'
                //            ,DelegateRemarks = '{DelegationRemarks}'
                //            ,ChgUsr = '{UseID}', Chgdat = GETDATE() Where UseId = '{UseID}';
                //            EXEC [Email_Delegate] @UseID = '{UseID}',@DelegateUseID = '{DelegateUseID}', @DelegateStatus = 'DELEGATION'";
                string query = @"DECLARE
	                                @ID VARCHAR(100) = '" + ID.ToString().ToUpper() + @"'
	                                ,@VisitorType VARCHAR(50) = '" + VisitorType.ToString().ToUpper() + @"'
	                                ,@NeedApprove BIT = CONVERT(BIT, '" + NeedApprove + @"')
	                                ,@NeedMailAlert BIT = CONVERT(BIT, '" + NeedMailAlert + @"')
	                                ,@AllowUnknown BIT = CONVERT(BIT, '" + AllowUnknown + @"')
	                                ,@IsView BIT = CONVERT(BIT, '" + IsView + @"')
	                                ,@UpdateOn VARCHAR(50) = '" + UpdateOn.ToString().ToUpper() + @"'
                                IF EXISTS(SELECT * FROM VISITORTYPE where Id=@ID)
                                BEGIN
                                UPDATE [dbo].[VisitorType]
                                    SET [VisitorType] = @VisitorType
                                        ,[NeedApprove] = @NeedApprove
                                        ,[NeedMailAlert] = @NeedMailAlert
                                        ,[AllowUnknown] = @AllowUnknown
                                        ,[IsView] = @IsView
                                        ,[UpdateAt] = CONVERT(DATE, GETDATE())
                                        ,[UpdateOn] = @UpdateOn
                                    WHERE Id=@ID
                                END
                                ELSE
                                BEGIN
                                DECLARE 
                                @maxId int
                                set @MaxId = (select isnull (max(cast(id as int))+1,1) from VisitorType)
                                INSERT INTO [dbo].[VisitorType]
                                            ([Id]
                                            ,[VisitorType]
                                            ,[NeedApprove]
                                            ,[NeedMailAlert]
                                            ,[AllowUnknown]
                                            ,[IsView]
                                            ,[UpdateAt]
                                            ,[UpdateOn])
                                        VALUES
                                (
                                            @MaxId
                                            ,@VisitorType
                                            ,@NeedApprove
                                            ,@NeedMailAlert
                                            ,@AllowUnknown
                                            ,@IsView
                                            ,CONVERT(DATE, GETDATE())
                                            ,@UpdateOn)
                                END
                                ";
                using (var sql = new MSSQL())
                {
                    num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
                }
            }
            catch (Exception ex)
            {
                return new VMSRes<string>
                {
                    Success = false,
                    data = "",
                    Message = "Failed to save visitor type. Please check with IT." + ex.Message
                };
            }

            return new VMSRes<string>
            {
                Success = true,
                data = "",
                Message = "Successfully save visitor type."
            };
        }
        public List<ModelKnowledgeMsg> SaveDept(string Plant, string Dept, string DeptName, string UseId, string Action)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"
                                BEGIN TRY
								DECLARE
									@Plant VARCHAR(10) = '" + Plant.ToString().ToUpper() + @"'
									,@Dept NVARCHAR(50) = N'" + Dept.ToString().ToUpper() + @"'
									,@DeptName NVARCHAR(50) = N'" + DeptName.ToString().ToUpper() + @"'
									,@UpdateBy NVARCHAR(50) = N'" + UseId.ToString().ToUpper() + @"'
									,@Action NVARCHAR(10) = '" + Action.ToString().ToUpper() + @"'

								IF (@Action = 'ADD')
								BEGIN
									IF EXISTS (SELECT * FROM Dept WHERE Plant=@Plant AND Dept = @Dept AND DeptName = @DeptName)
									BEGIN
										SELECT Status='0' , stsMessage='Department already exists.'
									END
									ELSE
									BEGIN
										INSERT INTO Dept (Plant, Dept, DeptName, UpdateBy, UpdateAt)
										VALUES (UPPER(@Plant), UPPER(@Dept), UPPER(@DeptName), UPPER(@UpdateBy), GETDATE())
										SELECT Status='success', stsMessage='Department successfully save.'
									END
								END
								ELSE IF (@Action = 'UPDATE')
								BEGIN
									UPDATE Dept
		                            SET DeptName = UPPER(@DeptName)
		                            ,UpdateBy = UPPER(@UpdateBy)
		                            ,UpdateAt = GETDATE()
		                            WHERE Plant = @Plant AND Dept = @Dept
									SELECT Status='success', stsMessage='Department successfully update.'
								END
							END TRY
							BEGIN CATCH
								SELECT Status='0',stsMessage=ERROR_MESSAGE()
							END CATCH
                                ";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        //end add
        public VMSRes<string> SaveAutoApproval(mmAutoApproval AutoApproval)
        {
            string query = @"
            DECLARE
	            @Plant varchar(4) = '"+ AutoApproval.Plant.ToString().ToUpper() + @"'
	            ,@Department varchar(50) = '" + AutoApproval.Department.ToString().ToUpper() + @"'
	            ,@WindowsID varchar(50) = '" + AutoApproval.WindowsID.ToString().ToUpper() + @"'
	            ,@PostBy varchar(50) = '" + AutoApproval.ChangeUser.ToString().ToUpper() + @"'
	            ,@id int = '" + AutoApproval.ID + @"'

            IF (@id = 0)
            BEGIN
	            INSERT INTO mmAutoApproval(Plant,Department, WindowsID, CreateUser, ChangeUser)
	            VALUES (@Plant,@Department,@WindowsID,@PostBy,@PostBy)
            END
            ELSE
            BEGIN
	            UPDATE mmAutoApproval
	            SET Plant = @Plant
	            ,Department = @Department
	            ,WindowsID = @WindowsID
	            ,ChangeUser = @PostBy
	            ,ChangeDate = GETDATE()
	            WHERE id = @id
            END
            ";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> DeleteAutoApprovalByID(mmAutoApproval AutoApproval)
        {
            string query = @"
            declare
	            @id int = '" + AutoApproval.ID + @"'

            DELETE FROM mmAutoApproval WHERE id = @id
            
            ";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public VMSRes<string> DeleteDept(string Plant, string Dept)
        {
            string query = @"
            declare
	            @Plant VARCHAR(10) = '" + Plant.ToString().ToUpper() + @"'
                ,@Dept NVARCHAR(50) = N'" + Dept.ToString().ToUpper() + @"'
            DELETE FROM Dept WHERE Plant = @Plant AND Dept= @Dept
            
            ";
            using (var sql = new MSSQL())
            {
                num = sql.ExecNonQuery(ConnectionDB, query, null, null, false);
            }
            return num.NonQueryResults();
        }
        public List<ModelKnowledgeMsg> SaveEditVisitor(string LogId, string id, string FullName, string Company, string JobDesc, string Email, string Phone, string VehicleNo, string UpdateBy)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"
                                BEGIN TRY
                                DECLARE
	                                @LogID BIGINT  ='" + LogId + @"' 
	                                ,@IDVisitor BIGINT = '" + id + @"'
	                                ,@fullName VARCHAR(100) = '" + FullName.ToString().ToUpper() + @"'--'MOHAMAD SHAHAIMI BIN MD SHARIF'
	                                ,@Company VARCHAR(50) = '" + Company.ToString().ToUpper() + @"'
	                                ,@JobDesc VARCHAR(100) = '" + JobDesc.ToString().ToUpper() + @"'
	                                ,@Email VARCHAR(100) = '" + Email.ToString().ToUpper() + @"'
	                                ,@Phone VARCHAR(20) = '" + Phone.ToString().ToUpper() + @"'
	                                ,@VehicleNo VARCHAR(20) = '" + VehicleNo.ToString().ToUpper() + @"'
	                                ,@UpdateBy VARCHAR(50) = '" + UpdateBy.ToString().ToUpper() + @"'
                                DECLARE @visitorID VARCHAR(100)
                                
                                IF EXISTS (SELECT * FROM Visitor WHERE Company = @Company AND fullName = @fullName)
                                BEGIN
	                                UPDATE Visitor SET
		                                fullName = UPPER(@fullName)
		                                ,JobDesc = UPPER(@JobDesc)
		                                ,email = UPPER(@Email)
		                                ,phone = UPPER(@Phone)
		                                ,VehicleNo = UPPER(@VehicleNo)
		                                ,UpdateBy = UPPER(@UpdateBy)
		                                ,UpdateAt = GETDATE()
	                                WHERE Company = @Company AND fullName = @fullName
	                                UPDATE VisitLog SET
		                                VisitorId = (SELECT id FROM Visitor WHERE Company = @Company AND fullName = @fullName)
		                                ,ChgUser = UPPER(@UpdateBy)
		                                ,ChgDate = GETDATE()
	                                WHERE LogId = @LogID AND VisitorId = @IDVisitor
	                                DELETE FROM Visitor WHERE id = @IDVisitor
                                    SELECT @visitorID=Id FROM Visitor WHERE Company = @Company AND fullName = @fullName
                                    SELECT Status='success',stsMessage='Data Updated. With RefNo: '+ CONVERT(VARCHAR, @LogID) +' and VisitorID: '+ CONVERT(VARCHAR,@visitorID)
                                END
                                ELSE
                                BEGIN
	                                --SELECT 'UPDATE UNKNOWN + UPDATE VisitorId di visitlog'
	                                UPDATE Visitor SET
		                                fullName = UPPER(@fullName)
		                                ,JobDesc = UPPER(@JobDesc)
		                                ,email = UPPER(@Email)
		                                ,phone = UPPER(@Phone)
		                                ,VehicleNo = UPPER(@VehicleNo)
		                                ,UpdateBy = UPPER(@UpdateBy)
		                                ,UpdateAt = GETDATE()
	                                WHERE id = @IDVisitor
	
	                                UPDATE VisitLog SET
		                                VisitorId = (SELECT id FROM Visitor WHERE Company = @Company AND fullName = @fullName)
		                                ,ChgUser = UPPER(@UpdateBy)
		                                ,ChgDate = GETDATE()
	                                WHERE LogId = @LogID AND VisitorId = @IDVisitor
                                    SELECT @visitorID=Id FROM Visitor WHERE Company = @Company AND fullName = @fullName
                                    SELECT Status='success',stsMessage='Data Replace. With RefNo: '+ CONVERT(VARCHAR, @LogID) +' and VisitorID: '+ CONVERT(VARCHAR,@visitorID)
                                END
                                END TRY
                                BEGIN CATCH
	                                SELECT Status='unsuccess',stsMessage=ERROR_MESSAGE()
                                END CATCH
                                ";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        //
        public List<AreaVisitor> GetAreaforDDList(string Plant, string VisitType)
        {
            DataTable dt = new DataTable();
            //string query = @"select areaId, areaName from AreaVisitor where plant='" + Plant + "' and VisitorType = '" + VisitType + "' ";
            string query = @"select areaId, areaName from AreaVisitor where plant='" + Plant + "' and VisitorType = '" + VisitType + "' order by AreaName "; // Modified by CANICE 20210927
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    areaId = rw["areaId"].ToString().ToUpper(),
                                    areaName = rw["areaName"].ToString().ToUpper(),
                                }).ToList();
            return _visitorArea;
        }
        public List<Plant> GetPlantforDDList(string PlantName = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT plantId, plantName FROM Plant where plantName like '%" + PlantName + @"%'
                            ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _plants = (from rw in dt.AsEnumerable()
                           select new Plant
                           {
                               plantId = Convert.ToInt16(rw["plantId"].ToString()),
                               plantName = rw["plantName"].ToString().ToUpper(),
                           }).ToList();
            return _plants;
        }
        public List<VisitType> GetVisitTypeforDDList(string VisitType = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, VisitorType FROM VisitorType
                            where visitorType is not null ";
            if (VisitType == "1")
            {
                query += @" and NeedApprove != 1 ";
            }

            query += @" order by VisitorType "; // Added by CANICE 20210927

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = Convert.ToInt16(rw["Id"].ToString()),
                                    VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                }).ToList();
            return _visitorType;
        }
        public List<VisitType> GetVisitTypeforDDListPublicView(string VisitType = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, VisitorType FROM VisitorType
                            where visitorType is not null AND IsView = 1 ";
            if (VisitType == "1")
            {
                query += @" and NeedApprove != 1 ";
            }

            query += @" order by VisitorType "; // Added by CANICE 20210927

            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = Convert.ToInt16(rw["Id"].ToString()),
                                    VisitorType = rw["VisitorType"].ToString().ToUpper(),
                                }).ToList();
            return _visitorType;
        }
        //
        public List<Visitor> PopulateVisitorCompany(string text)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE @USEMDM varchar(50)
set  @USEMDM = (select CodDes from CodLst where GrpCod='VISITORCONFIG' and Cod='USINGMDM')if (@USEMDM is null or @USEMDM = 'true')
BEGIN
SELECT DISTINCT [Description] from MDMDB.dbo.TVENDOR where [Description] Like'%{text}%' UNION
SELECT DISTINCT Company as [Description] from Visitor where Company Like '%{text}%'
END
else
BEGIN
SELECT DISTINCT Company as [Description] from Visitor where Company Like '%{text}%'
END
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Visitor
                         {
                             Company = rw["Description"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Visitor> PopulateVisitoryByCompany(string text, string company)
        {
            DataTable dt = new DataTable();
            string query = $@"
DECLARE
	@Company VARCHAR(100) = '" + company + @"'
    ,@fullName VARCHAR(100) = '" + text + @"'
SELECT DISTINCT
	fullName As [Description]
FROM Visitor
WHERE Company = @Company
AND fullName LIKE '%'+ @fullName +'%'
";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Visitor
                         {
                             FullName = rw["Description"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Department> GetDepartmentforDDList(string plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Dept, DeptName from Dept where plant = '" + plant + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Dept"].ToString().ToUpper(),
                             DeptName = rw["DeptName"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Department> GetDepartmentApprovalLevelDDList(string plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT DISTINCT Dept, DeptName FROM Dept
                             WHERE PLant = '" + plant + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Dept"].ToString().ToUpper(),
                             DeptName = rw["DeptName"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Department> GetApprovalCategoryDDList()
        {
            DataTable dt = new DataTable();
            string query = @"select Cod, CodAbb from CodLst WHERE GrpCod = 'ApprovalCategory'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Cod"].ToString().ToUpper(),
                             DeptName = rw["CodAbb"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Department> GetWindowsIDDDList()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT UseID as Dept, UseID +' - '+ UseNam As DeptName FROM Usr ORDER BY UseNam";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Dept"].ToString().ToUpper(),
                             DeptName = rw["DeptName"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Department> GetApprovalLevel(string Plant, string VisitorType, string Department, string Category, string WindowsID)
        {
            DataTable dt = new DataTable();
            string query = @"exec spGetDropDownListApprovalLevel '" + Plant + "', '" + VisitorType + "', '" + Department + "', '" + Category + "', '"+ WindowsID +"' ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Val"].ToString().ToUpper(),
                             DeptName = rw["Val"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<CodLst> GetCodLst(string GrpCod)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Cod, CodAbb from CodLst where GrpCod = '" + GrpCod + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _data = (from rw in dt.AsEnumerable()
                         select new CodLst
                         {
                             Cod = rw["Cod"].ToString().ToUpper(),
                             CodAbb = rw["CodAbb"].ToString().ToUpper()
                         }).ToList();
            return _data;
        }
        public List<Plant> GetPlantMDMList(string PlantName = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [Plant], [Description] FROM [MDMDB].[dbo].[TPLANT] where [Description] like '%" + PlantName + @"%' ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _plants = (from rw in dt.AsEnumerable()
                           select new Plant
                           {
                               plantId = Convert.ToInt16(rw["Plant"].ToString()),
                               plantName = rw["Description"].ToString(),
                           }).ToList();
            return _plants;
        }
        public VMSRes<string> PostUploadAutoApproval(string PhysicalPath, string SheetName, string HostId)
        {
            List<mmAutoApproval> listVisitor = SysUtil.GetObjectFromExcel<mmAutoApproval>(PhysicalPath, SheetName);
            int countSuccess = 0, countFail = 0;
            List<string> _message = new List<string>();
            foreach (var row in listVisitor)
            {
                var para = new List<ctSqlVariable>();
                para.Add(new ctSqlVariable { Name = "Plant", Type = "String", Value = row.Plant.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "Department", Type = "String", Value = row.Department.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "WindowsID", Type = "String", Value = row.WindowsID.IsStringNull().ToUpper() });
                para.Add(new ctSqlVariable { Name = "ChangeUser", Type = "String", Value = row.ChangeUser.IsStringNull() == "" ? HostId : row.ChangeUser.IsStringNull() }); //Modified by CANICE 20210407 //para.Add(new ctSqlVariable { Name = "HostId", Type = "String", Value = Sessions.GetUseID() });
                using (var sql = new MSSQL())
                {
                    dt = sql.ExecuteStoProcDT(ConnectionDB, "[UploadmmAutoAppointmentFromExcel]", para);
                }
                var result = dt.OneRowdtToResult();
                _message.Add(result.Message);
                if (result.Success) countSuccess++; else countFail++;
            }
            var finalMessage =
                    $@"Total Success={countSuccess.ToString()}, Total Fail={countFail}<br/>Remark : <br/>"
                    + string.Join("<br/>", _message);
            return new VMSRes<string>
            {
                Success = countFail == 0 ? true : false,
                Message = finalMessage,
                data = "true"
            };
        }
        public List<ModelKnowledgeMsg> MasterDataUpload(string NameFile, string MasterData, string User)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = "";
                if(MasterData == "AutoApproval")
                {
                    query = @"EXEC [UploadmmAutoApprovalFromExcel] '" + MasterData + @"','" + NameFile + @"','" + User + @"'";
                }
                else if (MasterData == "ApprovalLevel")
                {
                    query = @"EXEC [sp_UploadApprovalLevel] '" + MasterData + @"','" + NameFile + @"','" + User + @"'";
                }
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null,false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        public List<ModelKnowledgeMsg> UploadAutoApprovalNext(string User, string MasterData)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = "";
                if (MasterData.ToUpper() == "APPROVALLEVEL")
                {
                    query = @"EXEC [dbo].[sp_UploadApprovalLevelNext] '" + User + @"'";
                }
                else if (MasterData.ToUpper() == "AUTOAPPROVAL")
                {
                    query = @"EXEC [dbo].[UploadmmAutoApprovalFromExcel_Next] '" + User + @"'";
                }
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        public List<ModelKnowledgeMsg> DeleteApprovalLevel(string Plant, string VisitorType, string Department)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = "";
                query = @"DECLARE
	                        @Plant VARCHAR(10) = '"+ Plant.ToUpper() + @"'
	                        ,@VisitorType VARCHAR(50) ='" + VisitorType.ToUpper() + @"'
	                        ,@Department VARCHAR(50) = '" + Department.ToUpper() + @"'

                        BEGIN TRY
	                        DELETE FROM mmApprovalLevel
	                        WHERE Plant = @Plant
	                        AND VisitorType = @VisitorType
	                        AND Department = @Department
                            SELECT Status='success',stsMessage='Data successfully deleted.'
                        END TRY
                        BEGIN CATCH
                            SELECT Status='unsuccess',stsMessage=ERROR_MESSAGE();
                        END CATCH";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        public List<User> GetmmDelegationLog(string Plant, string Department, string StartDate, string EndDate)
        {
            double Number = 1;
            string query = @"DECLARE
	                            @Plant VARCHAR(4) = '" + Plant.ToUpper() + @"'
	                            ,@Department VARCHAR(50) = '" + Department.ToUpper() + @"'
	                            ,@StartDate VARCHAR(50) = '" + StartDate.ToUpper() + @"'
	                            ,@EndDate VARCHAR(50) = '" + EndDate.ToUpper() + @"'

                            IF @Department = '' SET @Department = '%'
                            IF @StartDate = '' SET @StartDate = CONVERT(VARCHAR,CONVERT(DATE,GETDATE()))
                            IF @EndDate = '' SET @EndDate = CONVERT(VARCHAR,CONVERT(DATE,GETDATE()))

                            --SET TO NULL IF EFFECTIVEDELEGATETO SMALL THAN TODAY
                            UPDATE Usr
                            SET 
	                            DelegateUseID = null
	                            ,DelegateEffectiveFrom = null
	                            ,DelegateEffectiveTo = null
	                            ,DelegateRemarks = null
                            WHERE CONVERT(DATE,DelegateEffectiveTo) < CONVERT(DATE,GETDATE())
                            -- END SET TO NULL

                            SELECT
	                            a.UsePlant As Plant
	                            ,a.UseID As WindowsID
	                            ,a.UseNam As UserName
	                            ,a.UseDep As Department
	                            ,a.DelegateEffectiveFrom As StartDate
	                            ,a.DelegateEffectiveTo As EndDate
	                            ,a.DelegateUseID As DelegatorWindowsID
	                            ,b.UseNam As Delegator
	                            ,a.ChgDat As SubmittedOn
                                ,a.DelegateRemarks
	                            ,CASE WHEN ISNULL(a.DelegateUseID, '') = '' THEN 'Not Active' ELSE 'Active' END As [Status]
                            FROM Usr a JOIN Usr b ON a.DelegateUseID = b.UseID
                            WHERE 
                            ISNULL(a.DelegateUseID,'') <> ''
                            AND a.UseDep LIKE @Department
                            AND CONVERT(DATE, a.DelegateEffectiveFrom) >= CONVERT(DATE, @StartDate) AND CONVERT(DATE, a.DelegateEffectiveFrom) <= CONVERT(DATE, @EndDate)
                            AND ISNULL(a.DelegateUseID, '') <> ''
                            ORDER BY a.UseID
                            ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                   select new User
                                   {
                                       Number = Number++,
                                       UsePlant = rw["Plant"].ToString().ToUpper(),
                                       UseID = rw["WindowsID"].ToString().ToUpper(),
                                       UseNam = rw["UserName"].ToString().ToUpper(),
                                       UseDep = rw["Department"].ToString().ToUpper(),
                                       DelegateEffectiveFrom = rw["StartDate"].ToString().ToUpper(),
                                       DelegateEffectiveTo = rw["EndDate"].ToString().ToUpper(),
                                       DelegateUseID = rw["DelegatorWindowsID"].ToString().ToUpper(),
                                       DelegateUseNam = rw["Delegator"].ToString().ToUpper(),
                                       ChgDate = rw["SubmittedOn"].ToString().ToUpper(),
                                       Status = rw["Status"].ToString().ToUpper(),
                                       DelegateRemarks = rw["DelegateRemarks"].ToString().ToUpper(),
                                   }).ToList();
            return _mmAutoApproval;
        }
        public DataTable GetListForExport(string Plant, string Department)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE
	                            @Plant VARCHAR(20) = '"+Plant.ToUpper() + @"'
	                            ,@Department VARCHAR(50) = '" + Department.ToUpper() + @"'

                            IF @Plant = '' OR @Plant = 'All' SET @Plant = '%'
                            IF @Department = '' OR @Department = 'All' SET @Department = '%'
                            SELECT
	                            a.Plant
	                            ,a.Department
	                            ,a.WindowsID
	                            ,b.UseNam As UserName
	                            ,a.ChangeUser
	                            ,a.ChangeDate
                            FROM mmAutoApproval a JOIN Usr b ON a.WindowsID = b.UseID
                            WHERE Plant LIKE @Plant
                            AND Department LIKE @Department ";
            //var param = new List<ctSqlVariable>();
            //param.Add(new ctSqlVariable { Name = "Plant", Type = "string", Value = Plant });
            //param.Add(new ctSqlVariable { Name = "Department", Type = "string", Value = Department });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public DataTable ExportFunction(string MasterData, string Plant, string Department)
        {
            DataTable dt = new DataTable();
            string query = "";
            if (MasterData == "VisitorType")
            {
                query = @"SELECT 
	                        [Id]
	                        ,[VisitorType]
	                        ,NeedApprove
	                        ,NeedMailAlert
	                        ,AllowUnknown
	                        ,IsView
	                        ,UpdateAt
	                        ,UpdateOn
                        FROM VisitorType";
            }
            else if(MasterData == "Department")
            {
                query = @"SELECT 
	                        Plant
	                        ,Dept
	                        ,DeptName
	                        ,UpdateBy
	                        ,UpdateAt
                        FROM Dept
                        ORDER BY Plant, Dept";
            }
            //string query = $@"DECLARE
	           //                 @Plant VARCHAR(20) = '" + Plant + @"'
	           //                 ,@Department VARCHAR(50) = '" + Department + @"'

            //                IF @Plant = '' OR @Plant = 'All' SET @Plant = '%'
            //                IF @Department = '' OR @Department = 'All' SET @Department = '%'
            //                SELECT
	           //                 a.Plant
	           //                 ,a.Department
	           //                 ,a.WindowsID
	           //                 ,b.UseNam As UserName
	           //                 ,a.ChangeUser
	           //                 ,a.ChangeDate
            //                FROM mmAutoApproval a JOIN Usr b ON a.WindowsID = b.UseID
            //                WHERE Plant LIKE @Plant
            //                AND Department LIKE @Department ";
            //var param = new List<ctSqlVariable>();
            //param.Add(new ctSqlVariable { Name = "Plant", Type = "string", Value = Plant });
            //param.Add(new ctSqlVariable { Name = "Department", Type = "string", Value = Department });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public DataTable GetListForExportApprovalLevel(string Plant, string Department, string VisitorType)
        {
            DataTable dt = new DataTable();
            string query = $@"DECLARE
	                                @Plant VARCHAR(10) = '" + Plant.ToUpper() + @"'
	                                ,@Department VARCHAR(50) = '" + Department.ToUpper() + @"'
	                                ,@VisitorType VARCHAR(50) = '" + VisitorType.ToUpper() + @"'
                                IF @Plant = '' SET @Plant = '%'
                                IF @Department = '' SET @Department = '%'
                                IF @VisitorType = '' SET @VisitorType = '%'
                                SELECT 
	                                a.Plant
	                                ,a.VisitorType
	                                ,a.Department
	                                ,a.ApprovalLevel
	                                ,a.WindowsID
	                                ,b.UseNam As UserName
	                                ,c.CodAbb
	                                ,a.Remarks
	                                ,a.CreateUser
	                                ,a.CreateDate
                                FROM mmApprovalLevel a
                                JOIN Usr b ON a.WindowsID = b.UseID
                                JOIN Codlst c ON a.Category = c.Cod AND c.GrpCod='ApprovalCategory'
                                WHERE a.Plant LIKE @Plant
                                AND a.Department LIKE @Department
                                AND a.VisitorType LIKE @VisitorType
                                ORDER BY a.Plant, a.VisitorType, a.Department, a.ApprovalLevel ASC ";
            //var param = new List<ctSqlVariable>();
            //param.Add(new ctSqlVariable { Name = "Plant", Type = "string", Value = Plant });
            //param.Add(new ctSqlVariable { Name = "Department", Type = "string", Value = Department });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            return dt;
        }
        public List<ModalMessage> LoadModalMessage(string useID)
        {
            string query = @"DECLARE
	                            @UserID VARCHAR(50) = '"+ useID.ToUpper() + @"'

                            DECLARE
	                            @HeaderDelegated VARCHAR(MAX)
	                            ,@HeaderDelegator VARCHAR(MAX)
                            SELECT TOP 1 @HeaderDelegated ='Hi '+ UseID +', your approval has been delegated.' 
                            FROM Usr
                            WHERE UseID = @UserID

                            SELECT TOP 1 @HeaderDelegator='Hi '+ DelegateUseID +', you have been delegated an approval.'
                            FROM Usr
                            WHERE DelegateUseID = @UserID

                            --Detail Untuk Delegated
                            SELECT 
	                            @HeaderDelegated As Header
	                            ,a.UseID As DelegatedUserID
	                            ,a.UseNam As DelegatedUserName
	                            ,a.UseDep As DelegatedDepartment
	                            ,a.DelegateUseID As DelegatorUserID
	                            ,b.UseNam As DelegatorUserName
	                            ,a.DelegateEffectiveFrom As StartTime
	                            ,a.DelegateEffectiveTo As EndTime
	                            ,b.UseDep As DelegatorDepartment
	                            ,'Delegated' As [Status]
                            FROM Usr a JOIN Usr b ON a.DelegateUseID = b.UseID
                            WHERE a.UseID = @UserID and A.DelegateUseID != ''
                            UNION
                            --Detail Untuk Delegator
                            SELECT 
	                            @HeaderDelegator As Header
	                            ,a.UseID As DelegatedUserID
	                            ,a.UseNam As DelegatedUserName
	                            ,a.UseDep As DelegatedDepartment
	                            ,a.DelegateUseID As DelegatorUserID
	                            ,b.UseNam As DelegatorUserName
	                            ,a.DelegateEffectiveFrom As StartTime
	                            ,a.DelegateEffectiveTo As EndTime
	                            ,b.UseDep As DelegatorDepartment
	                            ,'Delegator' As [Status]
                            FROM Usr a JOIN Usr b ON a.DelegateUseID = b.UseID
                            WHERE a.DelegateUseID = @UserID and A.DelegateUseID != ''
                            ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            if (dt != null)
            {
                var _mmAutoApproval = (from rw in dt.AsEnumerable()

                                       select new ModalMessage
                                       {
                                           Header = rw["Header"].ToString().ToUpper(),
                                           DelegatedUserID = rw["DelegatedUserID"].ToString().ToUpper(),
                                           DelegatedUserName = rw["DelegatedUserName"].ToString().ToUpper(),
                                           DelegatedDepartment = rw["DelegatedDepartment"].ToString().ToUpper(),
                                           DelegatorUserID = rw["DelegatorUserID"].ToString().ToUpper(),
                                           DelegatorUserName = rw["DelegatorUserName"].ToString().ToUpper(),
                                           DelegatorDepartment = rw["DelegatorDepartment"].ToString().ToUpper(),
                                           StartTime = rw["StartTime"].ToString().ToUpper(),
                                           EndTime = rw["EndTime"].ToString().ToUpper(),
                                           Status = rw["Status"].ToString().ToUpper(),
                                       }).ToList();
                return _mmAutoApproval;
            }
            else
            {
                return null;
            }
        }

        //HIDEF DISCLAIMER
        public List<Disclaimer> GetDisclaimerDatatables(string Search)
        {
            string PathDisclaimer = ConfigurationManager.AppSettings["PathDisclaimer"];

            //string query = @"SELECT 
            //                DIS.id, DIS.Plant,P.plantName, DIS.DisclaimerCategory, ValidFrom=convert(varchar, DIS.ValidFrom, 106), ValidTo=convert(varchar, DIS.ValidTo, 106), DIS.Type, DIS.DisclaimerFile, DIS.DisclaimerDesc
            //                , ChangeUser=CASE WHEN DIS.ChangeUser IS NULL THEN DIS.CreateUser ELSE DIS.ChangeUser END
            //                , ChangeDate=CASE WHEN DIS.ChangeDate IS NULL THEN DIS.CreateDate ELSE DIS.ChangeDate END
            //                ,aValidFrom=convert(varchar, DIS.ValidFrom, 23)
            //                ,aValidTo=convert(varchar, DIS.ValidTo, 23)
            //                FROM mmDisclaimer DIS
            //                INNER JOIN Plant P on DIS.Plant = P.plantId
            //                WHERE DIS.DisclaimerCategory LIKE  '%" + Search + @"%'";
            string query = @"DECLARE @Search NVARCHAR(MAX)='" + Search + @"'

                            SELECT 
                                DIS.id, DIS.Plant,P.plantName, DIS.DisclaimerCategory, ValidFrom=convert(varchar, DIS.ValidFrom, 106), ValidTo=convert(varchar, DIS.ValidTo, 106), DIS.Type, DIS.DisclaimerFile, DIS.DisclaimerDesc
                                , ChangeUser=CASE WHEN DIS.ChangeUser IS NULL THEN DIS.CreateUser ELSE DIS.ChangeUser END
                                , ChangeDate=CASE WHEN DIS.ChangeDate IS NULL THEN DIS.CreateDate ELSE DIS.ChangeDate END
                                ,aValidFrom=convert(varchar, DIS.ValidFrom, 23)
                                ,aValidTo=convert(varchar, DIS.ValidTo, 23)
	                            INTO #Data
                                FROM mmDisclaimer DIS
                                INNER JOIN Plant P on DIS.Plant = P.plantId

	                            SELECT * FROM #Data
	                            WHERE DisclaimerCategory LIKE  '%'+@Search+'%' 
	                            OR PlantName LIKE  '%'+@Search+'%' 
	                            OR Type LIKE '%'+@Search+'%'
	                            OR DisclaimerDesc LIKE '%'+@Search+'%'
	                            OR ChangeUser LIKE '%'+@Search+'%'

	                            DROP TABLE #Data";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _Disclaimer = (from rw in dt.AsEnumerable()
                               select new Disclaimer
                               {
                                   ID = rw["id"].ToString(),
                                   Plant = rw["Plant"].ToString(),
                                   PlantName = rw["plantName"].ToString(),
                                   DisclaimerCategory = rw["DisclaimerCategory"].ToString(),
                                   ValidFrom = rw["ValidFrom"].ToString(),
                                   ValidTo = rw["ValidTo"].ToString(),
                                   Type = rw["Type"].ToString(),
                                   DisclaimerFile = rw["DisclaimerFile"].ToString(),
                                   DisclaimerLinkFile = PathDisclaimer + "/" + rw["DisclaimerFile"].ToString(),
                                   DisclaimerDesc = rw["DisclaimerDesc"].ToString(),
                                   UpdateBy = rw["ChangeUser"].ToString(),
                                   UpdateAt = rw["ChangeDate"].ToString(),
                                   _ValidFrom = rw["aValidFrom"].ToString(),
                                   _ValidTo = rw["aValidTo"].ToString(),
                               }).ToList();
            return _Disclaimer;
        }

        public List<BusFunc> GetTypeDisclaimer()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Cod FROM CodLst WHERE GrpCod='TypeDisclaimer'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _type = (from rw in dt.AsEnumerable()
                         select new BusFunc
                         {
                             Id = rw["Cod"].ToString(),
                             Name = rw["Cod"].ToString(),
                         }).ToList();
            return _type;
        }
        public List<BusFunc> getArea(string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT DISTINCT AreaId,AreaName=AreaId+'-'+AreaName FROM AreaVisitor WHERE Plant='" + Plant + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _type = (from rw in dt.AsEnumerable()
                         select new BusFunc
                         {
                             Id = rw["AreaId"].ToString(),
                             Name = rw["AreaName"].ToString(),
                         }).ToList();
            return _type;
        }
        public List<BusFunc> getDisclaimerCategory(string Plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT DisclaimerCategory FROM mmDisclaimer WHERE Plant='" + Plant + @"'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }

            var _type = (from rw in dt.AsEnumerable()
                         select new BusFunc
                         {
                             Id = rw["DisclaimerCategory"].ToString(),
                             Name = rw["DisclaimerCategory"].ToString(),
                         }).ToList();
            return _type;
        }

        public List<ModelKnowledgeMsg> SaveDisclaimer(ItemDisclaimer Item, string NameFile, string UseID)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"EXEC spSaveDisclaimer '" + Item.ID + @"','" + Item.Plant + @"','" + Item.DisclaimerCategory + @"','" + Item.Type + @"','" + Item.ValidFrom + @"','" + Item.ValidTo + @"','" + Item.DisclaimerDesc + @"','" + NameFile + @"','" + UseID + @"'";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["Msg"].ToString(),
                        Remark = dr["ID"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }

        public List<ModelKnowledgeMsg> SaveDisclaimerOnly(ItemDisclaimer Item, string UseID)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"UPDATE mmDisclaimer SET 
			                            DisclaimerDesc='" + Item.DisclaimerDesc + @"'
			                            ,ChangeUser='" + UseID + @"'
			                            ,ChangeDate=GETDATE()
                                WHERE Plant='" + Item.Plant + @"'
                                SELECT Status='success',Msg='Successfully',ID='" + Item.Plant + @"'
                                ";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Status = dr["Status"].ToString(),
                        Message = dr["Msg"].ToString(),
                        Remark = dr["ID"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }

        public List<ModelKnowledgeMsg> DeleteDisclaimer(string ID)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"DECLARE @ID VARCHAR(50)='" + ID + @"'
                                        DELETE FROM mmDisclaimer WHERE id=@ID
                                        
                                        SELECT 'Delete Successfully' AS stsMessage";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }

        public List<DisclaimerArea> GetDisclaimerAreaDatatables(string Search)
        {
            string PathDisclaimer = ConfigurationManager.AppSettings["PathDisclaimer"];

            string query = @"SELECT DISTINCT
                            a.id,a.Plant,P.plantName,a.DisclaimerCategory,a.AreaId,b.AreaName
                            , ChangeUser=CASE WHEN a.ChangeUser IS NULL THEN a.CreateUser ELSE a.ChangeUser END
                            , ChangeDate=CASE WHEN a.ChangeDate IS NULL THEN a.CreateDate ELSE a.ChangeDate END                          
                            FROM mmDisclaimerArea a
                            JOIN AreaVisitor b ON a.Plant=b.Plant AND a.AreaId=b.AreaId
                            INNER JOIN Plant P on a.Plant = P.plantId
                            WHERE P.plantName LIKE '%" + Search + @"%' OR a.DisclaimerCategory LIKE '%" + Search + @"%' OR a.AreaId LIKE '%" + Search + @"%' OR b.AreaName LIKE '%" + Search + @"%'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _Disclaimer = (from rw in dt.AsEnumerable()
                               select new DisclaimerArea
                               {
                                   ID = rw["id"].ToString(),
                                   Plant = rw["Plant"].ToString(),
                                   PlantName = rw["plantName"].ToString(),
                                   DisclaimerCategory = rw["DisclaimerCategory"].ToString(),
                                   AreaId = rw["AreaId"].ToString(),
                                   AreaName = rw["AreaName"].ToString(),
                                   UpdateBy = rw["ChangeUser"].ToString(),
                                   UpdateAt = rw["ChangeDate"].ToString(),
                               }).ToList();
            return _Disclaimer;
        }

        public List<ModelKnowledgeMsg> SaveDisclaimerArea(string ID, string Plant, string Category, string Area, string UseID)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @" EXEC spSaveDisclaimerArea '" + Plant + @"'
		                            ,'" + Category + @"'
		                            ,'" + Area + @"'
		                            ,'" + UseID + @"'
		                            ,'" + ID + @"'";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Message = dr["msg"].ToString(),
                        Status = dr["status"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }

        public List<ModelKnowledgeMsg> DeleteDisclaimerArea(string ID)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"DECLARE @ID VARCHAR(50)='" + ID + @"'
                                        DELETE FROM mmDisclaimerArea WHERE id=@ID
                                        
                                        SELECT 'Delete Successfully' AS stsMessage";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
        public List<ModelKnowledgeMsg> ChangeStatusDisclaimer(string Plant, string Area, string Type, string Value)
        {
            List<ModelKnowledgeMsg> datas = new List<ModelKnowledgeMsg>();
            try
            {
                string query = @"IF (SELECT IsDisclaimer FROM AreaVisitor a WHERE Plant='" + Plant + @"' AND AreaId='" + Area + @"' AND VisitorType='" + Type + @"')=1
								BEGIN
									UPDATE a SET IsDisclaimer='0' FROM AreaVisitor a WHERE Plant='" + Plant + @"' AND AreaId='" + Area + @"' AND VisitorType='" + Type + @"'
								END
								ELSE
								BEGIN
									UPDATE a SET IsDisclaimer='1' FROM AreaVisitor a WHERE Plant='" + Plant + @"' AND AreaId='" + Area + @"' AND VisitorType='" + Type + @"'
								END

                                SELECT 'Successfully' AS stsMessage

                                EXEC spInsertDisclaimerActivity";
                DataSet ds = new DataSet();
                using (MSSQL sql = new MSSQL())
                {
                    ds = sql.ExecDSQuery(ConnectionDB, query, null, null, false);
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    datas.Add(new ModelKnowledgeMsg()
                    {
                        Message = dr["stsMessage"].ToString(),
                    });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datas;
        }
    }

    public enum Status
    {
        PENDING = 1,
        APPROVED = 2,
        REJECTED = 3,
        CHECKIN = 4,
        BREAK = 5,
        CHECKOUT = 6,
        DELETE = 7

    }
    public enum Method
    {
        Security = 1,
        User = 2
    }
    class MasterDataCommon
    {
    }
    public class Plant
    {
        public int plantId { get; set; }
        public string plantName { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class VisitType
    {
        public int Id { get; set; }
        public string VisitorType { get; set; }
        public string NeedApprove { get; set; }
        public string NeedMailAlert { get; set; }
        public string AllowUnknown { get; set; }
        public string IsView { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
    public class AreaVisitor
    {
        public int Plant { get; set; }
        public string PlantName { get; set; }
        public string areaGroupId { get; set; }
        public string areaId { get; set; }
        public string areaName { get; set; }
        public int VisitorType { get; set; }
        public string VisitorTypeName { get; set; }
        public DateTime UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public bool isDisclaimer { get; set; }
    }
    public class mmApprovalLevel
    {
        public int ID { get; set; }
        public string Plant { get; set; }
        public string VisitorType { get; set; }
        public string Department { get; set; }
        public string ApprovalLevel { get; set; }
        public string WindowsID { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string ChangeUser { get; set; }
        public string ChangeDate { get; set; }
        public string Approval { get; set; }
    }
    public class mmDepartment
    {
        public string Plant { get; set; }
        public string Dept { get; set; }
        public string DeptName { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateAt { get; set; }
    }
    public class TmpmmApprovalLevel
    {
        public string Plant { get; set; }
        public string VisitorType { get; set; }
        public string Department { get; set; }
        public string ApprovalLevel { get; set; }
        public string WindowsID { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string UploadStatus { get; set; }
        public string UploadUser { get; set; }
        public string UploadDate { get; set; }
    }
    public class TmpmmAutoApproval
    {
        public string Plant { get; set; }
        public string Department { get; set; }
        public string WindowsID { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string UploadUser { get; set; }
        public string UploadDate { get; set; }
    }
    public class mmAutoApproval
    {
        public double Number { get; set; }
        public int ID { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string WindowsID { get; set; }
        public string UserName { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string ChangeUser { get; set; }
        public string ChangeDate { get; set; }
    }
    public class SqlVariable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class HostAppointment
    {
        public string LogId { get; set; }
        public string UseID { get; set; }
        public string UseNam { get; set; }
        public string UseTel { get; set; }
        public string UseDep { get; set; }
        public string Area { get; set; }
        public string VisitorType { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string NeedApprove { get; set; }
    }
    public class VisitLogArea
    {
        public string LogId { get; set; }
        public string AreaId { get; set; }

    }
    public class Chart
    {
        public int Data { get; set; }
        public string Name { get; set; }
    }
    public class MenulistDto
    {
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string MenuAction { get; set; }
        public string MenuParent { get; set; }
        public string MenuController { get; set; }
        public string MenuIcon { get; set; }
    }
    public class BusFunc
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class Menulist
    {
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string MenuAction { get; set; }
        public string MenuParent { get; set; }
        public string MenuController { get; set; }
        public string MenuIcon { get; set; }
        public string MenuSeq { get; set; }
        public string isView { get; set; }
    }
    public class LogBook
    {
        public int LogId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string InfoGrant { get; set; }
        public string NameGrant { get; set; }
        public string DateGrant { get; set; }
        public string TimeGrant { get; set; }
        public string PhotoGrant { get; set; }
        public string NameReceive { get; set; }
        public string DateReceive { get; set; }
        public string TimeReceive { get; set; }
        public string PhotoReceive { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Plant { get; set; }
    }
    public class Department
    {
        public string Plant { get; set; }
        public string Dept { get; set; }
        public string DeptName { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateAt { get; set; }
    }
    public class CodLst
    {
        public string GrpCod { get; set; }
        public string Cod { get; set; }
        public string CodAbb { get; set; }
        public string CodDesc { get; set; }

    }
    public class PhotoLogBook
    {
        public string PhotoItem { get; set; }
        public string PhotoUser { get; set; }
    }


    public class ExitPermit2
    {
        public int Id { get; set; }
        public string EPNo { get; set; }
        public string UseDep { get; set; }
        public string UseID { get; set; }
        public string Username { get; set; }
        public string plantID { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Out { get; set; }
        public string In { get; set; }
        public string ActOut { get; set; }
        public string ActIn { get; set; }
        public string CompTrans { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
    public class EPMaster2
    {
        public string MasterId { get; set; }
        public int SENo { get; set; }
        public string EPNo { get; set; }
        public string UseDep { get; set; }
        public string PlantID { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Out { get; set; }
        public string In { get; set; }
        public string CompTrans { get; set; }
        public string CompTransTime { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
    }
    public class EPMasterDetail
    {
        public string MasterId { get; set; }
        public int SENo { get; set; }
        public string EPNo { get; set; }
        public string UseDep { get; set; }
        public string PlantID { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string Out { get; set; }
        public string In { get; set; }
        public string CompTrans { get; set; }
        public string CompTransTime { get; set; }
        public string Status { get; set; }
        public string DetailId { get; set; }
        public string UseID { get; set; }
        public string ActOut { get; set; }
        public string ActIn { get; set; }
        public int Id { get; set; }
    }


    public class MDeliveryOrder
    {
        public string MasterId { get; set; }
        public int SENo { get; set; }
        public string DONo { get; set; }
        public string UseDep { get; set; }
        public string UseID { get; set; }
        public string ReqDate { get; set; }
        public string Address { get; set; }
        public string DelVia { get; set; }
        public string DriName { get; set; }
        public string VechNo { get; set; }
        public string TimeOut { get; set; }
        public string ContainerNo { get; set; }
        public string SealNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceivedDate { get; set; }
        public string ReceivedPic { get; set; }
        public string SecurityCheck { get; set; }
        public string SecurityPic { get; set; }
        public string ManagerApprove { get; set; }
        public string Status { get; set; }
    }
    public class DDeliveryOrder
    {
        public string DetailId { get; set; }
        public string MasterId { get; set; }
        public int Id { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
        public string IsReturned { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnedDate { get; set; }
        public string Photo { get; set; }
    }
    public class MDDeliveryOrder
    {
        public string MasterId { get; set; }
        public int SENo { get; set; }
        public string DONo { get; set; }
        public string UseDep { get; set; }
        public string UseID { get; set; }
        public string ReqDate { get; set; }
        public string Address { get; set; }
        public string DelVia { get; set; }
        public string DriName { get; set; }
        public string VechNo { get; set; }
        public string TimeOut { get; set; }
        public string ContainerNo { get; set; }
        public string SealNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceivedDate { get; set; }
        public string ReceivedPic { get; set; }
        public string SecurityCheck { get; set; }
        public string SecurityPic { get; set; }
        public string ManagerApprove { get; set; }
        public string Status { get; set; }
        public string DetailId { get; set; }
        public int Id { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
        public string IsReturned { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnedDate { get; set; }
        public string Photo { get; set; }
    }

    
    public class Attendance
    {
        public int LogId { get; set; }
        public int TSVisitorId { get; set; }
        public string HostName { get; set; }
        public string HostDepartment { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string DateVisit { get; set; }
        public string NeedLunch { get; set; }
        public string Status { get; set; }
        public string Plant { get; set; }
        public string Remark { get; set; }
        public string NeedStay { get; set; }
        public string DateofEnd { get; set; }
        public string Premises { get; set; }
    }
    public class UploadMasterData
    {
        public HttpPostedFileBase File { get; set; }
        public string MasterData { get; set; }
    }
    public class JRes
    {
        private List<object> data;
        private bool result;
        private string msg;

        public List<object> Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }


        /// <summary>
        /// Instantiate new JRES object
        /// </summary>
        /// <param name="datalist">Any new list to perform add function</param>
        /// <param name="result">default set to false</param>
        /// <param name="message">default set to ""</param>
        public JRes(List<object> datalist, bool result, string message)
        {
            this.Data = datalist;
            this.Msg = message;
            this.Result = result;
        }
    }
    public class ModelKnowledgeMsg
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Remark { get; set; }

    }
    public class ModalMessage
    {
        public string Header { get; set; }
        public string DelegatedUserID { get; set; }
        public string DelegatedUserName { get; set; }
        public string DelegatedDepartment { get; set; }
        public string DelegatorUserID { get; set; }
        public string DelegatorUserName { get; set; }
        public string DelegatorDepartment { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
    }

    //DISCLAIMER
    public class Disclaimer
    {
        public string ID { get; set; }
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string DisclaimerCategory { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string _ValidFrom { get; set; }
        public string _ValidTo { get; set; }
        public string Type { get; set; }
        public string DisclaimerFile { get; set; }
        public string DisclaimerLinkFile { get; set; }
        public string DisclaimerDesc { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime aValidFrom { get; set; }
        public DateTime aValidTo { get; set; }
    }
    public class DisclaimerArea
    {
        public string ID { get; set; }
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string DisclaimerCategory { get; set; }
        public string AreaId { get; set; }
        public string AreaName { get; set; }
        public string UpdateAt { get; set; }
        public string UpdateBy { get; set; }
    }
}
