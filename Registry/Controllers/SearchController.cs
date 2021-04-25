using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Newtonsoft.Json;
using DataClasses;
using System.ServiceModel;

namespace Registry.Controllers
{
    //Finds all services that contain the search string
    public class SearchController : ApiController
    {

        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
            if (!File.Exists(PublishController.SERVICE_DESCRIPTIONS))
            {
                StreamWriter sw = File.CreateText(PublishController.SERVICE_DESCRIPTIONS);
                sw.Close();
            }

            List<ServiceModel> matches = new List<ServiceModel>();
            string value = pass.Pass;

            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            //Denies user of not logged in
            if (!auth.Validate(pass.Token).Equals("validated"))
            {
                ret.Status = "Denied";
                ret.Reason = "Authentication failed";
                return ret;
            }

            using (StreamReader sr = File.OpenText(PublishController.SERVICE_DESCRIPTIONS))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<ServiceModel> services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));

                if(services != null)
                {
                    foreach(ServiceModel service in services)
                    {
                        //Matches if service name contains the search string ignoring case
                        if(service.Name.ToUpper().Contains(value.ToUpper()))
                        {
                            matches.Add(service);
                        }
                    }
                }
                else
                {
                    ret.Status = "Denied";
                    ret.Reason = "No services have been published";
                }

                sr.Close();
            }
            ret.Returned = matches;
            return ret;

        }
    }
}