using System;
using System.ServiceModel;
using System.IO;
using DataClasses;

namespace Authenticator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a text file to store registered accounts
            if(!File.Exists(Authenticator.REGISTERED_ACCOUNTS_PATH))
            {
                StreamWriter sw = File.CreateText(Authenticator.REGISTERED_ACCOUNTS_PATH);
                sw.Close();
            }

            //Create a text file to store account tokens
            if (!File.Exists(Authenticator.ACCOUNT_TOKENS_PATH))
            {
                StreamWriter sw = File.CreateText(Authenticator.ACCOUNT_TOKENS_PATH);
                sw.Close();
            }

            //Start the server
            Console.WriteLine("Welcome to the authenticator!");
            var tcp = new NetTcpBinding();
            var host = new ServiceHost(typeof(Authenticator));
            host.AddServiceEndpoint(typeof(AuthenticatorInterface), tcp, "net.tcp://localhost:8101/AuthenticationService");
            host.Open();

            Console.WriteLine("Authetication system online...");
            Console.ReadLine();
            host.Close();
        }
    }
}
