using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataClasses;

namespace Registry.Controllers
{
    public class AllServicesController : ApiController
    {

        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
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

                //Adds all services to matches
                foreach(ServiceModel service in services)
                {
                        matches.Add(service);
                }
                sr.Close();
            }

            ret.Returned = matches;
            return ret;
        }
    }
}