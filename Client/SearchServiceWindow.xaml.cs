using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using DataClasses;
using Newtonsoft.Json;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SearchServiceWindow : Window
    {
        public SearchServiceWindow()
        {
            InitializeComponent();
        }

        //string URL = "https://localhost:44351/";
        //RestClient client = new RestClient(URL);
        //RestRequest request = new RestRequest("api/unpublish/");
        //request.AddJsonBody(api_endpoint);

        //    client.Post(request);

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            sp.Children.Clear();
            PassObject<string> pass = new PassObject<string>();
            pass.Token = DataSingleton.Instance.token;
            pass.Pass = SearchText.Text;

            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/search");
            request.AddJsonBody(pass);
            IRestResponse response = client.Post(request);

            ReturnObject<List<ServiceModel>> ret = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(response.Content);
            List<ServiceModel> services = ret.Returned;

            if (!ret.Status.Equals("Denied"))
            {

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            string endpoint = DataSingleton.FindEndpoint(name);
            TestServiceWindow testwin = new TestServiceWindow(this, endpoint);
            testwin.Show();
            this.Hide();
            //Send a get request for the clicked buttons name
        }

    }
}
