using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataClasses;
using System.ServiceModel;

namespace Registry.Controllers
{
    public class SearchController : ApiController
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
            string value = pass.Pass;
            //using (StreamReader sr = File.OpenText(PublishController.SERVICE_DESCRIPTIONS))
            //{
            //    string[] lines = File.ReadAllLines(PublishController.SERVICE_DESCRIPTIONS);

            //}
           
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

            using (StreamReader sr = File.OpenText(PublishController.SERVICE_DESCRIPTIONS))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<ServiceModel> services = (List<ServiceModel>)serializer.Deserialize(sr, typeof(List<ServiceModel>));

                foreach(ServiceModel service in services)
                {
                    if(service.Name.ToUpper().Contains(value.ToUpper()))
                    {
                        matches.Add(service);
                    }
                }
                sr.Close();
            }
            //return JsonConvert.SerializeObject(matches);
            ret.Returned = matches;
            return ret;

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