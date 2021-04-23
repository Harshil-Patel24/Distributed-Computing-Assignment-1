using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataClasses;

namespace Registry.Controllers
{
    public class AllServicesController : ApiController
    {
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
        public ReturnObject<List<ServiceModel>> Post([FromBody] PassObject<string> pass)
        {
            List<ServiceModel> matches = new List<ServiceModel>();
            ReturnObject<List<ServiceModel>> ret = new ReturnObject<List<ServiceModel>>();
            //using (StreamReader sr = File.OpenText(PublishController.SERVICE_DESCRIPTIONS))
            //{
            //    string[] lines = File.ReadAllLines(PublishController.SERVICE_DESCRIPTIONS);

            //}
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

            using (StreamReader sr = File.OpenText(PublishController.SERVICE_DESCRIPTIONS))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<ServiceModel> services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));

                foreach(ServiceModel service in services)
                {
                        matches.Add(service);
                }
                sr.Close();
            }
            //Console.WriteLine(JsonConvert.SerializeObject(matches));
            //string matches_string = JsonConvert.SerializeObject(matches);
            //return matches_string;
            //string json = new JavaScriptSerializer().Serialize(matches);
            ret.Returned = matches;
            return ret;
            //return JsonSerializer.Serialize(matches);
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