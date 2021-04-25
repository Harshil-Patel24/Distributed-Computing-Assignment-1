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
            string primes = "  ";

            if (pass.Pass.Length != 2)
            {
                ret.Status = "Denied";
                ret.Reason = "Input was invalid";
                return ret;
            }

            for (int ii = values[0]; ii <= values[1]; ii++)
            {
                if(ii % 2 != 0 && ii % 3 != 0 && ii % 5 != 0 && ii % 7 != 0 && ii > 1)
                {
                    primes += ii.ToString() + ", ";
                }
            }

            if (primes.Equals("  "))
            {
                ret.Returned = "There were no primes in this range";
                return ret;
            }

            primes = primes.Remove(primes.Length - 2);
            ret.Returned = primes;
            return ret;
        }
    }
}