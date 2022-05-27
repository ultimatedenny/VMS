using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VMS.Library;

namespace VMS.Web.Models
{
    public class ExitPermit
    {
        DataTable dt;
        //long num;
        public string ConnectionDB = System.Configuration.ConfigurationManager.ConnectionStrings["VMSConnection"].ToString();
        public List<EPMaster2> GetExitPermitDatatables(string ExitPermitNo, string DateFrom, string DateTo, string UseID)
        {
            string QUERY = "EXEC [SP_GET_PENDING_EP] '" + ExitPermitNo + "','PENDING','"+ DateFrom + "','"+ DateTo + "','"+ UseID + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, QUERY, null, null, false);
            }
            var _ExitPermit = (from rw in dt.AsEnumerable()
                               select new EPMaster2
                               {
                                   MasterId = rw["Id"].ToString(),
                                   SENo = int.Parse(rw["No"].ToString()),
                                   EPNo = rw["ExitPermit"].ToString(),
                                   UseDep = rw["Departement"].ToString(),
                                   PlantID = rw["Section"].ToString(),
                                   Destination = rw["Destination"].ToString(),
                                   Date = Convert.ToDateTime(rw["Date"].ToString()).ToString("yyyy-MM-dd"),
                                   In = TimeSpan.Parse(rw["In"].ToString()).ToString(),
                                   Out = TimeSpan.Parse(rw["Out"].ToString()).ToString(),
                                   CompTrans = bool.Parse(rw["CompanyTransport"].ToString()).ToString(),
                                   CompTransTime = TimeSpan.Parse(rw["CompanyTransportTime"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                               }).ToList();
            return _ExitPermit;
        }
        public List<EPMaster2> GetExitPermitDatatablesSecurity(string ExitPermitNo, string DateFrom, string DateTo)
        {
            string QUERY = "EXEC [SP_GET_PENDING_EP_SECURITY] '" + ExitPermitNo + "','APPROVED','" + DateFrom + "','" + DateTo + "'";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, QUERY, null, null, false);
            }
            var _ExitPermit = (from rw in dt.AsEnumerable()
                               select new EPMaster2
                               {
                                   MasterId = rw["Id"].ToString(),
                                   SENo = int.Parse(rw["No"].ToString()),
                                   EPNo = rw["ExitPermit"].ToString(),
                                   UseDep = rw["Departement"].ToString(),
                                   PlantID = rw["Section"].ToString(),
                                   Destination = rw["Destination"].ToString(),
                                   Date = Convert.ToDateTime(rw["Date"].ToString()).ToString("yyyy-MM-dd"),
                                   In = TimeSpan.Parse(rw["In"].ToString()).ToString(),
                                   Out = TimeSpan.Parse(rw["Out"].ToString()).ToString(),
                                   CompTrans = bool.Parse(rw["CompanyTransport"].ToString()).ToString(),
                                   CompTransTime = TimeSpan.Parse(rw["CompanyTransportTime"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                               }).ToList();
            return _ExitPermit;
        }
        public List<EPMaster2> GetExitPermitDatatablesHistory(string ExitPermitNo, string UseId, string DateFrom, string DateTo)
        {
            string query = @"SELECT	EM.MasterId as Id, 
                                EM.SENo as No,
		                        EM.EPNo as ExitPermit,
		                        EM.UseDep as Departement,
		                        P.PlantName as Section,
		                        EM.Destination as Destination,
								EM.CreatedBy as Requestor,
		                        Convert(nvarchar(10),EM.[Date],121) as [Date],
		                        EM.[Out] as [Out],
		                        EM.[In] as [In],
		                        EM.CompTrans as CompanyTransport,
		                        EM.CompTransTime as CompanyTransportTime,
		                        EM.[Status] as [Status]
                                from EPMASTER EM
                                INNER JOIN Plant P on EM.PlantID = P.plantId where EM.EPNo like '%" + ExitPermitNo + @"%' and  EM.[Status] = 'PENDING' and EM.CreatedBy = '" + UseId + @"' and
                                                                                   EM.[Date] between '" + DateFrom + @"' and '" + DateTo + @"'Order by EM.[Date] DESC";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _ExitPermit = (from rw in dt.AsEnumerable()
                               select new EPMaster2
                               {
                                   MasterId = rw["Id"].ToString(),
                                   SENo = int.Parse(rw["No"].ToString()),
                                   EPNo = rw["ExitPermit"].ToString(),
                                   UseDep = rw["Departement"].ToString(),
                                   PlantID = rw["Section"].ToString(),
                                   Destination = rw["Destination"].ToString(),
                                   Date = Convert.ToDateTime(rw["Date"].ToString()).ToString("yyyy-MM-dd"),
                                   In = TimeSpan.Parse(rw["In"].ToString()).ToString(),
                                   Out = TimeSpan.Parse(rw["Out"].ToString()).ToString(),
                                   CompTrans = bool.Parse(rw["CompanyTransport"].ToString()).ToString(),
                                   CompTransTime = TimeSpan.Parse(rw["CompanyTransportTime"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                                   CreatedBy = rw["Requestor"].ToString(),
                               }).ToList();
            return _ExitPermit;
        }

        public List<EPMasterDetail> GetExitPermitDatatables2(string ExitPermitNo, string DateFrom, string DateTo)
        {
            string query = @"SELECT EM.MasterId as MasterId, 
								ED.DetailId as DetailId,
                                ED.Id as No,
		                        EM.EPNo as ExitPermit,
								P.PlantName as Section,
		                        EM.UseDep as Departement,
		                        U.UseNam as Employee,
		                        EM.Destination as Destination,
		                        Convert(nvarchar(10),EM.[Date],121) as [Date],
		                        EM.[Out] as [Out],
								EM.ActOut as ActOut,
		                        EM.[In] as [In],
								EM.ActIn as ActIn,
		                        EM.CompTrans as CompanyTransport,
		                        EM.CompTransTime as CompanyTransportTime,
		                        EM.[Status] as [Status]
                                from EPMaster EM
								INNER JOIN EPDetails ED on EM.MasterId = ED.MasterId
                                INNER JOIN Plant P on EM.PlantID = P.plantId
								INNER JOIN Usr U on ED.UseID = U.UseID where EM.EPNo like '%" + ExitPermitNo + @"%' and  
                                                                             EM.[Date] between '" + DateFrom + @"' and '" + DateTo + @"' Order by EM.[Date] DESC ";
            using (var sql = new MSSQL())
            {
                dt = sql.ExecDTQuery(ConnectionDB, query, null, null, false);
            }
            var _ExitPermit = (from rw in dt.AsEnumerable()
                               select new EPMasterDetail
                               {
                                   MasterId = rw["MasterId"].ToString(),
                                   DetailId = rw["DetailId"].ToString(),
                                   Id = int.Parse(rw["No"].ToString()),
                                   EPNo = rw["ExitPermit"].ToString(),
                                   UseDep = rw["Departement"].ToString(),
                                   PlantID = rw["Section"].ToString(),
                                   UseID = rw["Employee"].ToString(),
                                   Destination = rw["Destination"].ToString(),
                                   Date = Convert.ToDateTime(rw["Date"].ToString()).ToString("yyyy-MM-dd"),
                                   Out = TimeSpan.Parse(rw["Out"].ToString()).ToString(),
                                   ActOut = TimeSpan.Parse(rw["ActOut"].ToString()).ToString(),
                                   In = TimeSpan.Parse(rw["In"].ToString()).ToString(),
                                   ActIn = TimeSpan.Parse(rw["ActIn"].ToString()).ToString(),
                                   CompTrans = bool.Parse(rw["CompanyTransport"].ToString()).ToString(),
                                   CompTransTime = TimeSpan.Parse(rw["CompanyTransportTime"].ToString()).ToString(),
                                   Status = rw["Status"].ToString(),
                               }).ToList();
            return _ExitPermit;
        }
        public DataTable ExportExitPermit(string DateStart, string DateEnd, string EPNo)
        {
            DataTable dt = new DataTable();
            string query = $@"spGetExitPermit";
            var param = new List<ctSqlVariable>();
            param.Add(new ctSqlVariable { Name = "DateStart", Type = "string", Value = DateStart });
            param.Add(new ctSqlVariable { Name = "DateEnd", Type = "string", Value = DateEnd });
            param.Add(new ctSqlVariable { Name = "EPNo", Type = "string", Value = EPNo });
            using (var sql = new MSSQL())
            {
                dt = sql.ExecuteStoProcDT(ConnectionDB, query, param);
            }
            return dt;
        }
    }
}