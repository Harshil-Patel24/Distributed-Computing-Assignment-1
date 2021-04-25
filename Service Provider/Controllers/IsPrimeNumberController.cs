using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using DataClasses;

namespace Service_Provider.Controllers
{
    public class IsPrimeNumberController : ApiController
    {
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
            string prime = "This is not a prime";

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

            if (id % 2 != 0 && id % 3 != 0 && id % 5 != 0 && id % 7 != 0 && id > 1)
            {
                prime = "This is a prime";
            }

            ret.Returned = prime;

            return ret;
        }
    }
}