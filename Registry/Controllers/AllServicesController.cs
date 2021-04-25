using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Web.Http;
using Newtonsoft.Json;
using DataClasses;

namespace Registry.Controllers
{
    public class AllServicesController : ApiController
    {

        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
            if (!File.Exists(PublishController.SERVICE_DESCRIPTIONS))
            {
                StreamWriter sw = File.CreateText(PublishController.SERVICE_DESCRIPTIONS);
                sw.Close();
            }

            List<ServiceModel> matches = new List<ServiceModel>();
            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();
  
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            string valid = auth.Validate(pass.Token);

            //If return was not valid ie. no token return denied
            if (!valid.Equals("validated"))
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
                    //Adds all services to matches
                    foreach(ServiceModel service in services)
                    {
                            matches.Add(service);
                    }
                    sr.Close();
                }
                else
                {
                    ret.Status = "Denied";
                    ret.Reason = "No services have been published";
                }

            }

            ret.Returned = matches;
            return ret;
        }
    }
}