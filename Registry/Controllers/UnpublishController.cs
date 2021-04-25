using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Web.Http;
using DataClasses;
using Newtonsoft.Json;
using System.IO;

namespace Registry.Controllers
{
    public class UnpublishController : ApiController
    {
        //Location of service descriptions, change this if using a different computer
        public const string SERVICE_DESCRIPTIONS = @"D:\Harshil\Uni\Units\DC\Assignment\Service_Descriptions.txt";

        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
            string value = pass.Pass;
            List<ServiceModel> services = new List<ServiceModel>();

            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            //Denies user if not logged in
            if (!auth.Validate(pass.Token).Equals("validated"))
            {
                ret.Status = "Denied";
                ret.Reason = "Authentication failed";
                return ret;
            }

            bool removed = false;
            if (new FileInfo(SERVICE_DESCRIPTIONS).Length != 0)
            {
                using(StreamReader sr = File.OpenText(SERVICE_DESCRIPTIONS))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));
                    
                    foreach(ServiceModel service in services.ToList())
                    {
                        //If api endpoints match, then remove the service
                        if(service.API_Endpoint.Equals(value))
                        {
                            services.Remove(service);
                            removed = true;
                        }
                    }
                }
            }

            //Return messages if failed or removed
            if(removed)
            {
                ret.Status = "Failed";
                ret.Reason = "Service with this API endpoint not found!";
            }
            else
            {
                ret.Status = "Succeeded";
                ret.Reason = "Service removed";
            }

            string output = JsonConvert.SerializeObject(services);

            using (StreamWriter sw = new StreamWriter(SERVICE_DESCRIPTIONS))
            {
                sw.WriteLine(output);
                sw.Close();
            }
            Console.WriteLine(output);

            return ret;
        }
    }
}