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
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
            string primes = "  ";
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

            if (pass.Pass.Length != 1)
            {
                ret.Status = "Denied";
                ret.Reason = "Input was invalid";
                return ret;
            }

            if(id < 0)
            {
                ret.Status = "Denied";
                ret.Reason = "There are no negative prime numbers";
                return ret;
            }

            for (int ii = 1; ii <= id; ii++)
            {
                if(ii % 2 != 0 && ii % 3 != 0 && ii % 5 != 0 && ii % 7 != 0 && ii > 1)
                {
                    primes += ii.ToString() + ", ";
                }
            }

            if(primes.Equals("  "))
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