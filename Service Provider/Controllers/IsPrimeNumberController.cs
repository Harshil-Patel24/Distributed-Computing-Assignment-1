using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Text.Json;
using System.Web.Http;
using System.ServiceModel;
using DataClasses;
//using Service_Provider.Models;


namespace Service_Provider.Controllers
{
    public class IsPrimeNumberController : ApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
            string prime = "false";

            ReturnObject<string> ret = new ReturnObject<string>();
            int id = pass.Pass[0];

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

            if (id % 2 != 0 && id % 3 != 0 && id % 5 != 0 && id % 7 != 0)
            {
                prime = "true";
            }

            ret.Returned = prime;

            return ret;
        }

        //// POST api/<controller>
        //public int Post([FromBody] int[] value)
        //{
        //    //Add exception handling here
        //    return value[0] + value[1] + value[3];
        //}

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