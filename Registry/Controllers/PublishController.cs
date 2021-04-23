using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataClasses;
using Newtonsoft.Json;
using System.IO;

namespace Registry.Controllers
{
    public class PublishController : ApiController
    {
        public const string SERVICE_DESCRIPTIONS = @"D:\Harshil\Uni\Units\DC\Assignment\Service_Descriptions.txt";
        private ServiceModel[] valid_services = { new ServiceModel("AddTwoNumbers", "Adds two numbers", "https://localhost:44303/api/addtwonumbers/", 2, "integer"), new ServiceModel("AddThreeNumbers", "Adds three numbers", "https://localhost:44303/api/addthreenumbers/", 3, "integer"), new ServiceModel("MulTwoNumbers", "Multiplys two numbers", "https://localhost:44303/api/multwonumbers/", 2, "integer"), new ServiceModel("MulThreeNumbers", "Multiplys three numbers", "https://localhost:44303/api/multhreenumbers/", 3, "integer"), new ServiceModel("GenPrimeNumbersInRange", "Generates all prime numbers in a given range", "https://localhost:44303/api/genprimenumbersinrange/", 2, "integer"), new ServiceModel("GenPrimeNumbersToValue", "Generates all primes numbers from 1 to a given number", "https://localhost:44303/api/genprimenumberstovalue/", 1, "integer"), new ServiceModel("IsPrimeNumber", "Checks if input number is a prime", "https://localhost:44303/api/isprimenumber/", 1, "integer") };

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<ServiceModel> pass)
        {
            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            string valid = auth.Validate(pass.Token);

            if (!valid.Equals("validated"))
            {
                ret.Status = "Denied";
                ret.Reason = "Authentication failed";
                return ret;
            }

            ServiceModel value = pass.Pass;

            if(ValidateService(value))
            {
                List<ServiceModel> services = new List<ServiceModel>();
                if (new FileInfo(SERVICE_DESCRIPTIONS).Length == 0)
                {
                    //Make this  a list
                    services.Add(value);
                }
                else
                {
                    using(StreamReader sr = File.OpenText(SERVICE_DESCRIPTIONS))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));
                        bool exist = false;
                        foreach (ServiceModel sm in services)
                        {
                            //Console.WriteLine("Checking if true");
                            if (sm.Name.Equals(value.Name))
                            {
                                //already exists
                        //Console.WriteLine("It does exist");
                                exist = true;
                            }
                        }
                        if (!exist)
                        {
                            Console.WriteLine(value.Name + " added as a service");
                            services.Add(value);
                        }
                    //services = services.Distinct<ServiceModel>();
                    //Read all lines, now put into a list of services
                    }
                }

                string output = JsonConvert.SerializeObject(services);

                using (StreamWriter sw = new StreamWriter(SERVICE_DESCRIPTIONS))
                {
                    sw.WriteLine(output);
                    sw.Close();
                }
//Console.WriteLine(output);
            }
            return ret;
        }

        private bool ValidateService(ServiceModel service)
        {
            bool valid = false;

            foreach(ServiceModel ser in valid_services)
            {
                Console.WriteLine(service.Name + " " + service.API_Endpoint);
                //Matches everything but description
                if(service.Name.Equals(ser.Name) && service.API_Endpoint.Equals(ser.API_Endpoint)) // && service.Number_Of_Operands == ser.Number_Of_Operands && service.Operand_Type.Equals(ser.Operand_Type))
                {
                    valid = true;
                }
            }

            return valid;
        }

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}