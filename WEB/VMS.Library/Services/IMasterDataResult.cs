using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface IMasterDataResult
    {
        //GET
        List<Plant> GetPlantforDDList(string PlantName = "");
        List<Plant> GetPlantMDMList(string PlantName = "");
        List<VisitType> GetVisitTypeforDDList(string VisitType ="");
        List<AreaVisitor> GetAreaforDDList(string Plant, string VisitType);
        List<Department> GetDepartmentforDDList(string plant);

        #region Area Visitor
        List<AreaVisitor> GetAreaDatatables(string AreaName);
        AreaVisitor GetAreaDetail(string AreaId, string VisitorType);
        MessageNonQuery SaveArea(AreaVisitor _area, string oldArea, string oldVisitorType);
        #endregion
        #region Visitor Type
        List<VisitType> GetVisitorTypeDatatables();
        VisitType GetVisitorTypeDetail(string Id);
        MessageNonQuery SaveVisitorType(VisitType VisitType);
        #endregion
        List<CodLst> GetCodLst(string GrpCod);
        List<Visitor> PopulateVisitorCompany(string text);
        MessageNonQuery ChangeStatusMasterData(string TableName, string ColumnName, string Id, string Username);




        //POST
    }
}
