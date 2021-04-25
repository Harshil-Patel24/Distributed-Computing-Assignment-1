using System;
using System.ServiceModel;
using RestSharp;
using DataClasses;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Service_Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        case 0:
                            Environment.Exit(0);
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

            Console.WriteLine(val);
        }

        public static void Login()
        {
            Console.WriteLine("Please enter your username");
            string username = Console.ReadLine();

            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            if (!auth.AccountExists(username))
            {
                Console.WriteLine("Username not registerd");
            }
            else
            {
                Console.WriteLine("Please enter your password");
                string password = Console.ReadLine();

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
                Console.WriteLine("Select an option:\n1) Publish a service\n2) Publish all valid services\n3) Unpublish a service\n0) Log out");
                string selection = Console.ReadLine();
                int sel = -1;
                bool isNum = int.TryParse(selection, out sel);

                if(isNum)
                {
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
                            DataSingleton.Instance.token = 0;
                        break;
                        default:
                            Console.WriteLine("Please enter a valid number selection {0-3}");
                            break;
                    }
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

            ServiceModel[] services = new ServicePublisher().valid_services;
            bool cont = true;
            do
            {
                int count = 0;
                Console.WriteLine("Please select which of the following services you would like to publish (or enter 0 to return to the menu): ");
                foreach(ServiceModel service in services)
                {
                    Console.WriteLine((count + 1).ToString() + ") " + service.Name);
                    count++;
                }
                Console.WriteLine("0) Return to menu");

                string entry = Console.ReadLine();

                bool isNum = int.TryParse(entry, out int selection);

                if (!isNum || selection < 0 || selection > count - 1)
                {
                    Console.WriteLine("Please enter a valid input");
                }
                else
                {
                    if (selection == 0)
                    {
                        break;
                    }
                    else
                    {
                        cont = false;

                        ServiceModel ser = services[selection - 1];
                        ServicePublisher servicePublisher = new ServicePublisher();
                        servicePublisher.Publish(ser);
                        Console.WriteLine(ser.Name + " has been published");
                    }
                }
            }
            while (cont);
        }

        public static void Unpublish()
        {
            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);

            RestRequest asrequest = new RestRequest("api/allservices/");
            PassObject<string> pass = new PassObject<string>();
            pass.Token = DataSingleton.Instance.token;
            asrequest.AddJsonBody(pass);
            IRestResponse serviceret = client.Post(asrequest);


            ReturnObject<List<ServiceModel>> services = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(serviceret.Content);
            string[] endpoints = new string[services.Returned.Count];
            bool cont = true;
            do
            {
                int count = 0;

                Console.WriteLine("Please select which of the following services you would like to remove (or enter 0 to return to the menu): ");
                foreach (ServiceModel service in services.Returned)
                {
                    endpoints[count] = service.API_Endpoint;
                    Console.WriteLine((count + 1).ToString() + ") " + service.Name);
                    count++;
                }
                Console.WriteLine("0) Return to menu");

                string entry = Console.ReadLine();

                bool isNum = int.TryParse(entry, out int selection);

                if (!isNum || selection < 0 || selection > count - 1)
                {
                    Console.WriteLine("Please enter a valid input");
                }
                else
                {
                    if(selection == 0)
                    {
                        break;
                    }
                    else
                    {
                        cont = false;

                        string endpoint = endpoints[selection - 1];

                        ServicePublisher servicePublisher = new ServicePublisher();
                        servicePublisher.Unpublish(endpoint);
                        Console.WriteLine("Service unpublished");
                    }
                }
            }
            while(cont);

        }

        ////Ensure an endpoint is in a valid format
        //private static bool ValidEndpointFormat(string endpoint)
        //{
        //    bool valid = false;
        //    Regex rx = new Regex(@"https?://localhost:\d\d\d\d\d/api/*/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //    MatchCollection matches = rx.Matches(endpoint);
        //    if(matches.Count > 0)
        //    {
        //        valid = true;
        //    }

        //    return valid;
        //}
    }
}
