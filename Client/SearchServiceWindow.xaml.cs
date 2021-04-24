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
    //This class is used to search a service
    public partial class SearchServiceWindow : Window
    {
        public SearchServiceWindow()
        {
            InitializeComponent();
        }

        //Takes user back to the menu window
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Close();
        }

        //Searches for any services with the inputted text
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //Clearing children to only show results in current search
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

            //Checks if the called service denied user service
            if (!ret.Status.Equals("Denied"))
            {
                DataSingleton.Instance.serviceModel = services;
                //Creates buttons for each service result
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

        //Takes user to test the service 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string name = btn.Name;
            string endpoint = DataSingleton.FindEndpoint(name);
            TestServiceWindow testwin = new TestServiceWindow(this, endpoint);
            testwin.Show();
            this.Hide();
        }

    }
}
