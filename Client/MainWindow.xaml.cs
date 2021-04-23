using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Text.Json;
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

//THIS IS PURELY FOR DEBUGGING/TESTING PLS REMOVE REFERNECE TO OTHER PROJECTS LATER
//using Service_Provider;
//using Service_Provider.Models;
//using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var tcp = new NetTcpBinding();
            //var URL = "net.tcp://localhost:8101/AuthenticationService";
            //var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            //var auth = authFactory.CreateChannel();

        }
  
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
                //MessageBox.Show(val.ToString());

                if (int.TryParse(val, out DataSingleton.Instance.token))
                {
                    ErrorText.Text = "";
                    //DataSingleton.Instance.token = val;
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
                System.Windows.MessageBox.Show(anf.Message);
            }
            catch(FaultException<FileFormatInvalidFault> ffi)
            {
                System.Windows.MessageBox.Show(ffi.Message);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }


        }

        private void RegisterWindowButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regwin = new RegisterWindow();
            regwin.Show();
            this.Hide();
        }
    }
}
