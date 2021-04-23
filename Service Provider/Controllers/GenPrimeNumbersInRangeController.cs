using System;
using System.ServiceModel;
using DataClasses;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Text.Json;
using System.Web.Http;
//using Service_Provider.Models;


namespace Service_Provider.Controllers
{
    public class GenPrimeNumbersInRangeController : ApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}



    // GET api/<controller>/5
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {

            ReturnObject<string> ret = new ReturnObject<string>();
            int[] values = pass.Pass;

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
            string primes = "";

            for(int ii = values[0]; ii <= values[1]; ii++)
            {
                if(ii % 2 != 0 && ii % 3 != 0 && ii % 5 != 0 && ii % 7 != 0)
                {
                    primes += ii.ToString() + ", ";
                }
            }

            //Removes last comma
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