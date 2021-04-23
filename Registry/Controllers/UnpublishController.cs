using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataClasses;
using Newtonsoft.Json;
using System.IO;

namespace Registry.Controllers
{
    public class UnpublishController : ApiController
    {
        public const string SERVICE_DESCRIPTIONS = @"D:\Harshil\Uni\Units\DC\Assignment\Service_Descriptions.txt";
        private ServiceModel[] valid_services = { new ServiceModel("AddTwoNumbers", "Adds two numbers", "https://localhost:44301/api/addtwonumbers/", 2, "integer"), new ServiceModel("AddThreeNumbers", "Adds three numbers", "https://localhost:44301/api/addthreenumbers/", 3, "integer"), new ServiceModel("MulTwoNumbers", "Multiplys two numbers", "https://localhost:44301/api/multwonumbers/", 2, "integer"), new ServiceModel("MulThreeNumbers", "Multiplys three numbers", "https://localhost:44301/api/multhreenumbers/", 2, "integer"), new ServiceModel("GenPrimeNumbersInRange", "Generates all prime numbers in a given range", "https://localhost:44301/api/genprimenumbersinrange/", 2, "integer"), new ServiceModel("GenPrimeNumbersToValue", "Generates all primes numbers from 1 to a given number", "https://localhost:44301/api/genprimenumberstovalue/", 1, "integer"), new ServiceModel("IsPrimeNumber", "Checks if input number is a prime", "https://localhost:44301/api/isprimenumber/", 1, "integer") };

        //Change to be an api endpoint, and remove the service of that API endpoint
        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
            string value = pass.Pass;
            //ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();
            List<ServiceModel> services = new List<ServiceModel>();

            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            if (!auth.Validate(pass.Token).Equals("validated"))
            {
                ret.Status = "Denied";
                ret.Reason = "Authentication failed";
                return ret;
            }

            //Use this to return if successful or not
            bool removed = false;
            if (new FileInfo(SERVICE_DESCRIPTIONS).Length != 0)
            {
                //List<ServiceModel> services = new List<ServiceModel>();
                using(StreamReader sr = File.OpenText(SERVICE_DESCRIPTIONS))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));
                    
                    foreach(ServiceModel service in services)
                    {
                        if(service.API_Endpoint.Equals(value))
                        {
                            services.Remove(service);
                            removed = true;
                        }
                    }
                }
            }


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

        //private bool ValidateService(string endpoint)
        //{
        //    bool valid = false;

        //    return valid;
        //}
    }
}