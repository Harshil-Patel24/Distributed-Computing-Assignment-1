using System;
using System.IO;
using System.ServiceModel;
using RestSharp;
using DataClasses;
using System.Text.RegularExpressions;
using Authenticator;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Publisher
{
    class Program
    {
        //Locations of registered accounts and tokens, change this if using a different computer
        public const string REGISTERED_ACCOUNTS_PATH = @"D:\Harshil\Uni\Units\DC\Assignment\Registered_Accounts.txt";
        public const string ACCOUNT_TOKENS_PATH = @"D:\Harshil\Uni\Units\DC\Assignment\Account_Tokens.txt";

        static void Main(string[] args)
        {
            //Create a text file to store registered accounts
            if (!File.Exists(REGISTERED_ACCOUNTS_PATH))
            {
                StreamWriter sw = File.CreateText(REGISTERED_ACCOUNTS_PATH);
                sw.Close();
            }

            if (!File.Exists(ACCOUNT_TOKENS_PATH))
            {
                StreamWriter sw = File.CreateText(ACCOUNT_TOKENS_PATH);
                sw.Close();
            }

            //Start the server
            Console.WriteLine("Welcome to the service publisher!");
            var tcp = new NetTcpBinding();
            var host = new ServiceHost(typeof(ServicePublisher));
            host.AddServiceEndpoint(typeof(ServicePublisherInterface), tcp, "net.tcp://localhost:8102/ServicePublisher");
            host.Open();

            Console.WriteLine("Service publishing system online...");
            Console.ReadLine();

            Menu();

            host.Close();
        }
        
        private static void Menu()
        {
            bool exit = false;
            while(!exit)
            {

                Console.WriteLine("Select an option:\n1) Register\n2) Log in\n0) Exit");
                string selection = Console.ReadLine();
                int sel;
                bool isNum = int.TryParse(selection, out sel);

                if(isNum)
                {
                    switch(sel)
                    {
                        case 1:
                            Register();
                            break;
                        case 2:
                            Login();
                            break;
                        default:
                            Console.WriteLine("Please enter a valid number selection {0-2}");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entry was not an integer! Please enter a valid selection, ie. an integer between 0 and 2!");
                }

            }
        }

        public static void Register()
        {
            Console.WriteLine("Please enter a username to use");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter a password");
            string password = Console.ReadLine();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            string val = auth.Register(username, password);

            Console.WriteLine("Registration successful");
        }

        public static void Login()
        {
            Console.WriteLine("Please enter your username");
            string username = Console.ReadLine();
            bool found = false;
            using (StreamReader sr = File.OpenText(REGISTERED_ACCOUNTS_PATH))
            {
                string[] lines = File.ReadAllLines(REGISTERED_ACCOUNTS_PATH);

                if (lines[0].Split(',').Length != 2 && int.TryParse(lines[0].Split(',')[0], out int n))
                {
                    throw new FaultException<FileFormatInvalidFault>(new FileFormatInvalidFault() { Issue = "The file: " + ACCOUNT_TOKENS_PATH + " was not formatted correctly" });
                }

                foreach (string line in lines)
                {
                    string name = line.Split(',')[0];
                    if (name.Equals(username))
                    {
                        found = true;
                        break;
                    }
                }
                sr.Close();
            }

            if(!found)
            {
                Console.WriteLine("Username not registerd");
            }
            else
            {
                Console.WriteLine("Please enter your password");
                string password = Console.ReadLine();

                var tcp = new NetTcpBinding();
                var URL = "net.tcp://localhost:8101/AuthenticationService";
                var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
                var auth = authFactory.CreateChannel();
                string val = auth.Login(username, password);

                if(int.TryParse(val, out DataSingleton.Instance.token))
                {
                    if(DataSingleton.Instance.token != 0)
                    {
                        Console.WriteLine("Log in successful!");
                        Menu2();
                    }
                    else
                    {
                        Console.WriteLine(val);
                    }
                }
                else
                {
                    Console.WriteLine(val);
                }

            }
        }

        public static void Menu2()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Select an option:\n1) Publish a service\n2) Publish all valid services\n3) Unpublish a service\n0) Exit");
                string selection = Console.ReadLine();
                int sel;
                bool isNum = int.TryParse(selection, out sel);
                switch(sel)
                {
                    case 1:
                        Publish();
                        break;
                    case 2:
                        PublishAllDefault();
                        break;
                    case 3:
                        Unpublish();
                        break;
                    case 0:
                        exit = true;
                    break;
                    default:
                        Console.WriteLine("Please enter a valid number selection {0-3}");
                        break;
                }
            }
        }

        //An easy way to publish all services
        public static void PublishAllDefault()
        {
            Console.WriteLine("Publishing all of the following default services -\nAddTwoNumbers\nAddThreeNumbers\nMulTwoNumbers\nMulThreeNumbers\nIsPrimeNumber\nGenPrimeNumbersToValue\nGenPrimeNumbersInRange");
            ServicePublisher publisher = new ServicePublisher();
            publisher.PublishAllDefault();
        }

        public static void Publish()
        {
            Console.WriteLine("Please enter the name of the service (this must be an exact match to a valid service):");
            string name = Console.ReadLine();
            Console.WriteLine("Please enter a description of the service");
            string desc = Console.ReadLine();
            bool cont = true;
            string endpoint;
            int numop;

            do
            {
                Console.WriteLine("Please enter the api endpoint (this must be the exact endpoint eg. https://localhost:0000/api/serviceendpoint/)");
                endpoint = Console.ReadLine();
                if (ValidEndpointFormat(endpoint))
                {
                    cont = false;
                }
                else
                {
                    Console.WriteLine("Invalid endpoint format!");
                }
            }
            while (cont);

            do
            {
                cont = true;
                Console.WriteLine("Please enter the number of operands");
                string num = Console.ReadLine();
                cont = !(int.TryParse(num, out numop));
            }
            while (cont);

            Console.WriteLine("Please enter the operand type");
            string type = Console.ReadLine();

            ServicePublisher publisher = new ServicePublisher();
            publisher.Publish(name, desc, endpoint, numop, type);
        }
    
        public static void Unpublish()
        {
            Console.WriteLine("Please enter the api endpoint of the service you would like to unpublish (this must be the exact endpoint eg. https://localhost:0000/api/serviceendpoint/)");
            string endpoint = Console.ReadLine();

            string URL = "https://localhost:44351/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/unpublish/");
            request.AddJsonBody(endpoint);

            client.Execute(request);
        }

        //Ensure an endpoint is in a valid format
        private static bool ValidEndpointFormat(string endpoint)
        {
            bool valid = false;
            Regex rx = new Regex(@"https?://localhost:\d\d\d\d\d/api/*/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(endpoint);
            if(matches.Count > 0)
            {
                valid = true;
            }

            return valid;
        }
    }
}
