using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceModel;
using Newtonsoft.Json;
using RestSharp;
using DataClasses;
//using System.Web.Script.Serialization;
//using System.Windows.Forms;
//using System.Web.UI.WebControls;

namespace Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowServicesWindow : Window
    {
        //List<ServiceModel> services = DataSingleton.Instance.serviceModel;
        public ShowServicesWindow()
        {
            InitializeComponent();

            //int[] twoints = { one, two };

            //string jsonString = System.Text.Json.JsonSerializer.Serialize<TwoInts>(twoints);
            PassObject<string> pass = new PassObject<string>();
            pass.Token = DataSingleton.Instance.token;
            string URL = "https://localhost:44358/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/allservices/");
            //request.AddJsonBody(twoints);
            request.AddJsonBody(pass);
            IRestResponse result = client.Post(request);

            ReturnObject<List<ServiceModel>> ret = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(result.Content);
            
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menu = new MenuWindow();
            menu.Show();
            this.Hide();
        }
    }
}
