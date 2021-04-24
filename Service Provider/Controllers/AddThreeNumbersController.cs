using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataClasses;
using System.ServiceModel;


namespace Service_Provider.Controllers
{
    public class AddThreeNumbersController : ApiController
    {
        public ReturnObject<string> Post([FromBody] PassObject<int[]> pass)
        {
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
    }
}