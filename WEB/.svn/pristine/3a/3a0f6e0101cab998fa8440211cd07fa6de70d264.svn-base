using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Library.Models;

namespace VMS.Library.Services
{
    interface IVisitorResult
    {
        //GET
        Visitor getVisitorByPassCard(string PassCardNo);
        List<Visitor> getAllVisitor(string NameorCompany);
        List<Visitor> getListVisitorAfterInvite(string UserName);
        List<Visitor> getListVisitorBeforeInvite(string NameorCompany, string UserName);
        HostAppointment GetHostAppointment(string CardId);
        Visitor GetVisitorDetail(string Id);
        List<LogHistory> GetVisitorLists();

        //POST NON QUERY
        MessageNonQuery postNewVisitor(Visitor visitor);
        MessageNonQuery putPhotoVisitor(int idVisitor, string pathPhoto);
        MessageNonQuery putEditVisitor(Visitor visitor);
        MessageNonQuery postAddVisitorAppointment(int Id, string UserName);
        MessageNonQuery deleteVisitorAppointment(int Id, string UserName);
        MessageNonQuery postUpdateCardRegister(int Id, string CardId);
    }
}
