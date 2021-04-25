using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using RestSharp;
using DataClasses;

namespace Client
{
    public partial class ShowServicesWindow : Window
    {
        //Shows all services
        public ShowServicesWindow()
        {
            InitializeComponent();

            PassObject<string> pass = new PassObject<string>();
            pass.Token = DataSingleton.Instance.token;
            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/allservices/");
            request.AddJsonBody(pass);
            IRestResponse result = client.Post(request);

            //Returns all services
            ReturnObject<List<ServiceModel>> ret = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(result.Content);
            
            //If successful then create buttons and add them to a stack panel
            if(!ret.Status.Equals("Denied"))
            {
                List<ServiceModel> services = ret.Returned;

                DataSingleton.Instance.serviceModel = services;

                foreach (ServiceModel service in services)
                {
                    Button newBut = new Button();
                    newBut.Content = service.Name;
                    newBut.Name = service.Name;

                    newBut.Click += new RoutedEventHandler(Button_Click);
                    sp.Children.Add(newBut);
                }
            }
            else
            {
                ErrorText.Text = ret.Status + ": " + ret.Reason;
            }
        }

        //Takes user to test the selected service
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            string endpoint = DataSingleton.FindEndpoint(name);
            TestServiceWindow testwin = new TestServiceWindow(this, endpoint);
            testwin.Show();
            this.Hide();
        }

        //Returns user to menu
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Hide();
        }
    }
}
