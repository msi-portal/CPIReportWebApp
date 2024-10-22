using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace CPIReportWebApp
{
    public class IDOWebServices
    {
        public IDOWebServices()
        {

        }

        public string GetConfig()
        {
            string configGroup = ConfigurationManager.AppSettings["ConfigGroup"].ToString();
            string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
            IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient("IDOWebServiceSoap", requestUrl + $"/IDOWebService.asmx?configgroup=" + configGroup);
            string[] configs = soapClient.GetConfigurationNames();
            return configs[0];
        }

        public string GetToken()
        {
            IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();
            string _token = "";
            try
            {
                string userId = ConfigurationManager.AppSettings["UserId"].ToString();
                string pswd = ConfigurationManager.AppSettings["Pswd"].ToString();
                string config = GetConfig();
                _token = soapClient?.CreateSessionToken(userId, pswd, config);
            }
            catch (System.Exception)
            {
                _token = "";
            }

            return _token;
        }

        public DataSet GetDataSet(string ido, string properties, string filter = null, string orderBy = "")
        {
            DataSet ds = new DataSet();
            string postQueryMethod = string.Empty;
            string token = GetToken();
            int recordCap = -1;
            IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();
            ds = soapClient.LoadDataSet(token, ido, properties, filter, orderBy, postQueryMethod, recordCap);
            return ds;
        }
    }
}