using System;
using System.Web.Http;
using DataClasses;
using System.ServiceModel;

namespace Service_Provider.Controllers
{
    public class AddTwoNumbersController : ApiController
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

            if (pass.Pass.Length != 2)
            {
                ret.Status = "Denied";
                ret.Reason = "Input was invalid";
                return ret;
            }

            try
            {
                long ans = checked(((long)value[0] + (long)value[1]));
                ret.Returned = ans.ToString();
            }
            catch (OverflowException)
            {
                ret.Status = "Denied";
                ret.Reason = "Numbers were too large";
                return ret;
            }

            return ret;
        }
    }
}