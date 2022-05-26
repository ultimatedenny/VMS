using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Class;
using VMS.Library.Constants;
using VMS.Library.Models;

namespace VMS.Library.Actions
{
    public class MasterDataAction 
    {
        Database SQLCon = new Database();
        public MessageNonQuery ChangeStatusMasterData(string TableName, string ColumnName, string id, string Username)
        {
            string query = @"
                IF EXISTS(SELECT 1 FROM " + TableName + @" WHERE " + ColumnName + @" ='" + id + @"')
                BEGIN
                    UPDATE " + TableName + @" set isActive = case
                        when isActive = 1 then 0 else 1 end
	                    where  " + ColumnName + @" = '" + id + @"'
                END";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public List<AreaVisitor> GetAreaDatatables(string AreaName)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT PlantName, areaId, areaName, AV.VisitorType, VT.VisitorType as VisitorTypeName
from AreaVisitor AV
INNER JOIN Plant P on AV.Plant = P.plantId
INNER JOIN VisitorType VT on AV.VisitorType = VT.id where AV.Plant like '%" + AreaName + @"%' or AV.areaId like '%" + AreaName + @"%' or AV.AreaName like '%" + AreaName + @"%' or P.plantName like '%" + AreaName + @"%'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    PlantName = rw["PlantName"].ToString(),
                                    VisitorType = int.Parse(rw["VisitorType"].ToString()),
                                    VisitorTypeName = rw["VisitorTypeName"].ToString(),
                                    areaId = rw["areaId"].ToString(),
                                    areaName = rw["areaName"].ToString(),
                                }).ToList();
            return _visitorArea;
        }
        public List<VisitType> GetVisitorTypeDatatables()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [Id]
      ,[VisitorType]
      ,[NeedApprove] FROM [VisitorType]";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = int.Parse(rw["Id"].ToString()),
                                    VisitorType = rw["VisitorType"].ToString(),
                                    NeedApprove = rw["NeedApprove"].ToString(),
                                }).ToList();
            return _visitorType;
        }
        public VisitType GetVisitorTypeDetail(string Id)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [Id]
      ,[VisitorType]
      ,[NeedApprove] FROM [VisitorType] where Id='"+Id+"'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = int.Parse(rw["Id"].ToString()),
                                    VisitorType = rw["VisitorType"].ToString(),
                                    NeedApprove = rw["NeedApprove"].ToString(),
                                }).FirstOrDefault();
            return _visitorType;
        }
        public AreaVisitor GetAreaDetail(string AreaId, string VisitorType)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Plant, areaId, areaName, AV.VisitorType
from AreaVisitor AV
INNER JOIN Plant P on AV.Plant = P.plantId
INNER JOIN VisitorType VT on AV.VisitorType = VT.id
WHERE areaId='" + AreaId + @"' and AV.VisitorType='" + VisitorType + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    Plant = int.Parse(rw["Plant"].ToString()),
                                    VisitorType = int.Parse(rw["VisitorType"].ToString()),
                                    areaId = rw["areaId"].ToString(),
                                    areaName = rw["areaName"].ToString(),
                                }).FirstOrDefault();
            return _visitorArea;
        }
        public MessageNonQuery SaveArea(AreaVisitor _area, string oldArea, string oldVisitorType)
        {
            string query = @"
        IF EXISTS(SELECT * FROM AreaVisitor where areaId='" + oldArea + "' and VisitorType='" + oldVisitorType + @"')
BEGIN
Update AreaVisitor set Plant='" + _area.Plant + @"', 
areaId='" + _area.areaId + @"', 
areaName='" + _area.areaName + @"', 
VisitorType='" + _area.VisitorType + @"', 
UpdateBy='" + _area.UpdateBy + @"', 
UpdateAt='" + DateTime.Now.ToString("yyyy-MM-dd") + @"' where areaId='" + oldArea + @"' and VisitorType = '" + oldVisitorType + @"' 
END
        ELSE
BEGIN
INSERT INTO AreaVisitor(Plant, AreaId, AreaName, VisitorType, UpdateAt, UpdateBy)
VALUES('" + _area.Plant + @"','" + _area.areaId + @"','" + _area.areaName + @"','" + _area.VisitorType + @"','" + DateTime.Now.ToString("yyyy-MM-dd") + @"','" + _area.UpdateBy + @"')
END";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);
        }
        public List<AreaVisitor> GetAreaforDDList(string Plant, string VisitType)
        {
            DataTable dt = new DataTable();
            string query = @"select areaId, areaName from AreaVisitor where plant='" + Plant + "' and VisitorType = '" + VisitType + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorArea = (from rw in dt.AsEnumerable()
                                select new AreaVisitor
                                {
                                    areaId = rw["areaId"].ToString(),
                                    areaName = rw["areaName"].ToString(),
                                }).ToList();
            return _visitorArea;
        }
        public List<Plant> GetPlantforDDList(string PlantName = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT plantId, plantName FROM Plant where plantName like '%" + PlantName + @"%'
                            ";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _plants = (from rw in dt.AsEnumerable()
                           select new Plant
                           {
                               plantId = Convert.ToInt16(rw["plantId"].ToString()),
                               plantName = rw["plantName"].ToString(),
                           }).ToList();
            return _plants;
        }
        public List<VisitType> GetVisitTypeforDDList(string VisitType = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT id, VisitorType FROM VisitorType
                            where visitorType is not null";
            if (VisitType == "1")
            {
                query += @" and NeedApprove != 1";
            }
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _visitorType = (from rw in dt.AsEnumerable()
                                select new VisitType
                                {
                                    Id = Convert.ToInt16(rw["Id"].ToString()),
                                    VisitorType = rw["VisitorType"].ToString(),
                                }).ToList();
            return _visitorType;
        }
        public List<Visitor> PopulateVisitorCompany(string text)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT DISTINCT [Description] from MDMDB.dbo.TVENDOR where [Description] Like'%" + text + @"%' UNION
SELECT DISTINCT Company as [Description] from Visitor where Company Like '% " + text + @"%'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _data = (from rw in dt.AsEnumerable()
                         select new Visitor
                         {
                             Company = rw["Description"].ToString()
                         }).ToList();
            return _data;
        }

        public List<Department> GetDepartmentforDDList(string plant)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Dept, DeptName from Dept where plant = '" + plant + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _data = (from rw in dt.AsEnumerable()
                         select new Department
                         {
                             Dept = rw["Dept"].ToString(),
                             DeptName = rw["DeptName"].ToString()
                         }).ToList();
            return _data;
        }

        public List<CodLst> GetCodLst(string GrpCod)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT Cod, CodAbb from CodLst where GrpCod = '" + GrpCod + "'";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _data = (from rw in dt.AsEnumerable()
                         select new CodLst
                         {
                             Cod = rw["Cod"].ToString(),
                             CodAbb = rw["CodAbb"].ToString()
                         }).ToList();
            return _data;
        }

        public MessageNonQuery SaveVisitorType(VisitType VisitType)
        {
            string query = @"
IF EXISTS(SELECT * FROM VISITORTYPE where Id='" + VisitType.Id + @"')
BEGIN
UPDATE [dbo].[VisitorType]
   SET [VisitorType] = '" + VisitType.VisitorType + @"'
      ,[NeedApprove] = '" + VisitType.NeedApprove + @"'
      ,[UpdateAt] = '" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
      ,[UpdateOn] = '" + VisitType.UpdateBy + @"'
 WHERE Id='" + VisitType.Id + @"'
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
           ,[UpdateAt]
           ,[UpdateOn])
     VALUES
(
           @MaxId
           ,'" + VisitType.VisitorType+ @"'
           ,'" + VisitType.NeedApprove + @"'
           ,'"+DateTime.Now.ToString("yyyy-MM-dd")+@"'
           ,'"+VisitType.UpdateBy+@"')
END
";
            return SQLCon.executeNonQuery(query, ConnectionDB.VMSConnection);

        }

        public List<Plant> GetPlantMDMList(string PlantName = "")
        {
            DataTable dt = new DataTable();
            string query = @"SELECT [Plant], [Description] FROM [MDMDB].[dbo].[TPLANT] where [Description] like '%" + PlantName + @"%' ";
            dt = SQLCon.getDatatableFromSQL(query, ConnectionDB.VMSConnection);
            var _plants = (from rw in dt.AsEnumerable()
                           select new Plant
                           {
                               plantId = Convert.ToInt16(rw["Plant"].ToString()),
                               plantName = rw["Description"].ToString(),
                           }).ToList();
            return _plants;
        }
    }
}
