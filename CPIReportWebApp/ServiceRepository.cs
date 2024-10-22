using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CPIReportWebApp
{
    public class ServiceRepository<T>
    {
        public HttpClient Client { get; set; }

        public ServiceRepository()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServiceUrl"].ToString());
        }

        public HttpResponseMessage GetResponse(string url)
        {
            return Client.GetAsync(url).Result;
        }
        
        public HttpResponseMessage DeleteResponse(string url)
        {
            return Client.DeleteAsync(url).Result;
        }

        public T GetList(string ido, string properties, string filter, string orderby = null)
        {
            T resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>("");
            string queryStr = $"/MGRESTService.svc/js/{ido}/adv?props={properties}&filter={filter}&rowcap=-1";
            if (orderby != null)
            {
                queryStr += "&orderby={orderby}";
            }
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + queryStr;
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken());
            HttpResponseMessage response = Client.GetAsync(requestUrl).Result;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            using (HttpContent content = response.Content)
            {
                string jsonData = content.ReadAsStringAsync().Result;
                resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData, settings);
            }

            return resultList;
        }

        public T InvokeMethod(string ido, string method, string parms)
        {
            T resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>("");
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/MGRESTService.svc/js/method/{ido}/{method}?parms={parms}";
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken());
            HttpResponseMessage response = Client.GetAsync(requestUrl).Result;

            using (HttpContent content = response.Content)
            {
                string jsonData = content.ReadAsStringAsync().Result;
                resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
            }

            return resultList;
        }

        public void UpdateItem(string ido, UpdateProperty updateProp, List<IDOUpdateItem> idoItem)
        {
            string json = string.Empty;
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/MGRESTService.svc/xml/{ido}/updateitems";
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken());

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            using (XmlWriter writer = xdoc.CreateWriter())
            {
                DataContractSerializer serializer = new DataContractSerializer(idoItem.GetType());
                serializer.WriteObject(writer, idoItem);
            }
            HttpResponseMessage response = Client.PutAsync(requestUrl, new StringContent(xdoc.ToString(),
                Encoding.UTF8, "application/xml")).Result;

            using (HttpContent content = response.Content)
            {
                Task<string> result = content.ReadAsStringAsync();
                json = result.Result;
            }

        }

        public void AddItems(string ido, List<T> idoItems)
        {
            T resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>("");
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/MGRESTService.svc/xml/{ido}/additems";
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", GetToken());

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            using (XmlWriter writer = xdoc.CreateWriter())
            {
                DataContractSerializer serializer = new DataContractSerializer(idoItems.GetType());
                serializer.WriteObject(writer, idoItems);
            }
            // pass the List<IDOUpdateItem> as the request data and send the insert request
            HttpResponseMessage response = Client.PostAsync(requestUrl, new StringContent(xdoc.ToString(), Encoding.UTF8, "application/xml")).Result;

            using (HttpContent content = response.Content)
            {
                //string jsonData = content.ReadAsStringAsync().Result;
                Task<string> result = content.ReadAsStringAsync();
                string xml = result.Result;
            }
        }

        public string GetToken()
        {
            string json = string.Empty;
            string userId = ConfigurationManager.AppSettings["UserId"].ToString();
            string pswd = ConfigurationManager.AppSettings["Pswd"].ToString();
            string config = GetConfiguration();
            //string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/MGRESTService.svc/json/token/{config}";
            //Client.DefaultRequestHeaders.Add("userid", userId); 
            //Client.DefaultRequestHeaders.Add("password", pswd);
            //HttpResponseMessage response = Client.GetAsync(requestUrl).Result;
            //using (HttpContent content = response.Content)
            //{
            //    Task<string> result = content.ReadAsStringAsync();
            //    string jsonData = content.ReadAsStringAsync().Result;
            //    Tokens jsons = JsonConvert.DeserializeObject<Tokens>(jsonData);
            //    json = jsons.Token;
            //}
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/ido/token/{config}";
            Client.DefaultRequestHeaders.Add("username", userId);
            Client.DefaultRequestHeaders.Add("password", pswd);
            HttpResponseMessage response = Client.GetAsync(requestUrl).Result;
            using (HttpContent content = response.Content)
            {
                Task<string> result = content.ReadAsStringAsync();
                // get the response containing the token
                string jresult = result.Result;
                Tokens jsons = JsonConvert.DeserializeObject<Tokens>(jresult);
                json = jsons.Token;
            }
            return json;
        }

        public string GetConfiguration()
        {
            string json = string.Empty;
            string configGroup = ConfigurationManager.AppSettings["ConfigGroup"].ToString();
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/ido/configurations?configgroup={configGroup}";
            HttpResponseMessage response = Client.GetAsync(requestUrl).Result;

            using (HttpContent content = response.Content)
            {
                string jsonData = content.ReadAsStringAsync().Result;
                Configs jsons = JsonConvert.DeserializeObject<Configs>(jsonData);
                json = jsons.Configurations[0];
            }
            return json;
        }        

    }
}