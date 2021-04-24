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
    public class MulThreeNumbersController : ApiController
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

            ret.Returned = (value[0] * value[1] * value[2]).ToString();

            return ret;
        }
    }
}