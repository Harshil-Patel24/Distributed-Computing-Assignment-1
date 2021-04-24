using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using RestSharp;
using DataClasses;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
  
        //Uses authenticators log in functions to log a user in using their input name and password
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UsernameTextLogin.Text;
            string password = PasswordBox.Password;
            string val = "";
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            try
            {
                val = auth.Login(name, password);

                //Checks if token was a valid return, ie. if it was a successful login
                //The token is stored in a singleton called DataSingleton that is accessible on client side
                if (int.TryParse(val, out DataSingleton.Instance.token))
                {
                    ErrorText.Text = "";
                    //If logged in move to menu
                    MenuWindow win = new MenuWindow();
                    win.Show();
                    this.Hide();
                }
                else
                {
                    ErrorText.Text = val;
                }
            }
            catch(FaultException<AccountNotFoundFault> anf)
            {
                ErrorText.Text = anf.Message;
            }
            catch(FaultException<FileFormatInvalidFault> ffi)
            {
                ErrorText.Text = ffi.Message;
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }


        }
        
        //Move to the register window
        private void RegisterWindowButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regwin = new RegisterWindow();
            regwin.Show();
            this.Hide();
        }
    }
}
