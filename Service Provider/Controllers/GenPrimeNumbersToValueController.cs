using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Text.Json;
using DataClasses;
using System.ServiceModel;
using System.Web.Http;
//using Service_Provider.Models;


namespace Service_Provider.Controllers
{
    public class GenPrimeNumbersToValueController : ApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
            string primes = "";
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

            for (int ii = 1; ii <= id; ii++)
            {
                if(ii % 2 != 0 && ii % 3 != 0 && ii % 5 != 0 && ii % 7 != 0)
                {
                    primes += ii.ToString() + ", ";
                }
            }

            primes = primes.Remove(primes.Length - 2);
            //primes = primes.Remove(primes.Length - 2);
            ret.Returned = primes;

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