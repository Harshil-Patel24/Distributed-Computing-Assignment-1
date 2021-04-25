using DataClasses;
using System.ServiceModel;
using RestSharp;

namespace Service_Publisher
{
    class ServicePublisher : ServicePublisherInterface
    {
        //Valid services
        public ServiceModel[] valid_services = { new ServiceModel("AddTwoNumbers", "Adds two numbers", "https://localhost:44303/api/addtwonumbers/", 2, "integer"), new ServiceModel("AddThreeNumbers", "Adds three numbers", "https://localhost:44303/api/addthreenumbers/", 3, "integer"), new ServiceModel("MulTwoNumbers", "Multiplys two numbers", "https://localhost:44303/api/multwonumbers/", 2, "integer"), new ServiceModel("MulThreeNumbers", "Multiplys three numbers", "https://localhost:44303/api/multhreenumbers/", 3, "integer"), new ServiceModel("GenPrimeNumbersInRange", "Generates all prime numbers in a given range", "https://localhost:44303/api/genprimenumbersinrange/", 2, "integer"), new ServiceModel("GenPrimeNumbersToValue", "Generates all primes numbers from 1 to a given number", "https://localhost:44303/api/genprimenumberstovalue/", 1, "integer"), new ServiceModel("IsPrimeNumber", "Checks if input number is a prime", "https://localhost:44303/api/isprimenumber/", 1, "integer") };

        public int Login(string name, string password)
        {
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            string val = auth.Login(name, password);

            if (!int.TryParse(val, out int token))
            {
                return 0;
            }

            return token;
        }

        public string Register(string name, string password)
        {
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            string status = auth.Register(name, password);
            return status;
        }

        public void Publish(ServiceModel sm)
        {
            PassObject<ServiceModel> pass = new PassObject<ServiceModel>();
            pass.Pass = sm;
            pass.Token = DataSingleton.Instance.token;
            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/publish/");

            request.AddJsonBody(pass);

            client.Post(request);
        }

        public void PublishAllDefault()
        {
            foreach (ServiceModel sm in valid_services)
            {
                Publish(sm);
            }
        }

        public void Unpublish(string api_endpoint)
        {
            PassObject<string> pass = new PassObject<string>();
            pass.Pass = api_endpoint;
            pass.Token = DataSingleton.Instance.token;
            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/unpublish/");
            request.AddJsonBody(pass);

            client.Post(request);
        }
    }
}
