using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VMS.Core.Constants;
using VMS.Core.Model;

namespace VMS.Core.Repository
{
    public class VisitorRepository
    {
        public const string Url = Config.Url; //"http://SBM-VMIIS03:81/VMSApi/api/tsvisitor/";


        public async Task<VisitorJoinLog> GetVisitor_TSByBadge(string ShimanoBadge)
        {
            VisitorJoinLog list = new VisitorJoinLog();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var urlMethod = Url + Link.GetorPostVisitorTS + "?ShimanoBadge=" + ShimanoBadge;
                    
                    Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                    HttpResponseMessage response = await getResponse;
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<VisitorJoinLog>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {
                    return null;
                    throw;
                }
            }
        }
        public async Task<Visitor_TS> GetVisitorTSForCheck(string ShimanoBadge)
        {
            Visitor_TS list = new Visitor_TS();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var urlMethod = Url + Link.GetorPostVisitorTS + "?ShimanoBadge=" + ShimanoBadge;

                    Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                    HttpResponseMessage response = await getResponse;
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<Visitor_TS>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {
                    return null;
                    throw;
                }
            }
        }

        public async Task<List<Temp_VisitorTS>> GetListVisitorTemp(string Name)
        {
            List<Temp_VisitorTS> list = new List<Temp_VisitorTS>();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    var urlMethod = Url + Link.GetListVisitorTemp + "?Name=" + Name;
                    Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                    HttpResponseMessage response = await getResponse;
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<Temp_VisitorTS>>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        public async Task<List<Visitor_TS>> GetListVisitor(string Name)
        {
            List<Visitor_TS> list = new List<Visitor_TS>();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    var urlMethod = Url + Link.GetListVisitorTS + "?Name=" + Name;
                    Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                    HttpResponseMessage response = await getResponse;
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<Visitor_TS>>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task<MessageNonQuery> PostNewShimanoBadge(ShimanoBadgeModel postData)
        {
            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostNewShimanoBadge, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
             
        }
        public async Task<MessageNonQuery> PostChangeShimanoBadge(ShimanoBadgeModel postData)
        {
            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostChangeShimanoBadge, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
             
        }
        public async Task<WifiAccount> GetWifiAccount(string Username, string _Status ="", string IPAddress="")
        {
            if(_Status == "WIFI")
            {
                string responseJsonString;
                using (var client = new HttpClient())
                {
                    try
                    {
                        var urlMethod = Url + Link.GetWifiAccount + "?Username=" + Username;
                        Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                        HttpResponseMessage response = await getResponse;
                        responseJsonString = await response.Content.ReadAsStringAsync();
                        var list = JsonConvert.DeserializeObject<WifiAccount>(responseJsonString);
                        return list;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
            else
            {
                var _CodLst = await GetCodLst("SSID_TS_" + IPAddress);
                WifiAccount wa = new WifiAccount();
                wa.WifiName = _CodLst[0].Cod;
                wa.Username = "";
                wa.Password = _CodLst[0].CodAbb;
                return wa;
            }
            
        }

        public async Task<MessageNonQuery> PostNewWIFI(string WIFI_Host, string WIFI_Visitor, string Username, string Password, string CreBy="SystemInsert")
        {
            ShimanoWIFI postData = new ShimanoWIFI();
            postData.Host = WIFI_Host;
            postData.Visitor = WIFI_Visitor;
            postData.UserName = Username;
            postData.Password = Password;
            postData.CreBy = CreBy;
            


            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostNewWIFI, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task<List<CodLst>> GetCodLst(string GrpCod)
        {
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    var urlMethod = Url + Link.GetCodLst + "?GrpCod=" + GrpCod;
                    Task<HttpResponseMessage> getResponse = client.GetAsync(urlMethod);
                    HttpResponseMessage response = await getResponse;
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<CodLst>>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        public async Task<MessageNonQuery> PostVisitorCheckin(ShimanoBadgeModel postData)
        {
            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    string a = JsonConvert.SerializeObject(postData).Replace("/","").Replace(@"\", "");



                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostVisitorCheckin, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        public async Task<MessageNonQuery> PostVisitorPhoto(PhotoAndroid postData)
        {
            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostVisitorPhoto, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        public async Task<MessageNonQuery> PostVisitorCheckOut(string LogId)
        {
            ShimanoBadgeModel postData = new ShimanoBadgeModel
            {
                Temp_Visitor = LogId
            };
            MessageNonQuery list = new MessageNonQuery();
            string responseJsonString;
            using (var client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url + Link.PostVisitorCheckOut, content);
                    responseJsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<MessageNonQuery>(responseJsonString);
                    return list;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

    }
}
