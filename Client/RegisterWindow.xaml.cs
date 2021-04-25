using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Data;
using DataClasses;

namespace Client
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        //Registers user using autheinticators registration service
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UsernameText.Text;
            string password = PasswordBox.Password;
            string val = "";
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8101/AuthenticationService";
            var authFactory = new ChannelFactory<AuthenticatorInterface>(tcp, URL);
            var auth = authFactory.CreateChannel();

            val = auth.Register(name, password);

            //Either displays successful or error message
            ResponseText.Text = val;
        }

        //Takes user back to the login page
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
