using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Text.Json;
using System.Web.Http;
using DataClasses;
using System.ServiceModel;
//using Service_Provider.Models;


namespace Service_Provider.Controllers
{
    public class AddThreeNumbersController : ApiController
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
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
            //Add exception handling here
            ReturnObject<string> ret = new ReturnObject<string>();
            int[] value = pass.Pass;

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

            if(pass.Pass.Length != 3)
            {
                ret.Status = "Denied";
                ret.Reason = "Input was invalid";
            }

            ret.Returned = (value[0] + value[1] + value[2]).ToString();

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