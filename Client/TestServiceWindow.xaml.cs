using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using DataClasses;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestServiceWindow : Window
    {
        //private string api_ep;
        private ServiceModel ser;
        private Window caller;
        public TestServiceWindow(Window call, string api_endpoint)
        {
            InitializeComponent();
            caller = call;
            //api_ep = api_endpoint;
            //string URL = api_endpoint.Split('/')[0] + "/" + api_endpoint.Split('/')[1] + "/" + api_endpoint.Split('/')[2]  + "/";
            //string req = api_endpoint.Split('/')[3] + "/" + api_endpoint.Split('/')[4] + "/";
            //RestClient client = new RestClient(URL);
            //RestRequest request = new RestRequest(req);

            ser = DataSingleton.FindService(api_endpoint);
            NameLabel.Content = ser.Name;
            
            ColumnDefinition boxes = new ColumnDefinition();
            ColumnDefinition labels = new ColumnDefinition();
            Grd.ColumnDefinitions.Add(labels);
            Grd.ColumnDefinitions.Add(boxes);

            for (int ii = 0; ii < ser.Number_Of_Operands; ii++)
            {
                TextBox textbox = new TextBox();
                Label label = new Label();
                
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(90);
                Grd.RowDefinitions.Add(row);

                label.Content = "Input " + ii.ToString() + ":";
                textbox.Text = "";
                textbox.Name = "tb" + ii.ToString();

                Grid.SetRow(label, ii);
                Grid.SetColumn(label, 0);

                Grid.SetRow(textbox, ii);
                Grid.SetColumn(textbox, 1);

                Grd.Children.Add(label);
                Grd.Children.Add(textbox);
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            string api_ep = ser.API_Endpoint;
            string URL = api_ep.Split('/')[0] + "/" + api_ep.Split('/')[1] + "/" + api_ep.Split('/')[2] + "/";
            string req = api_ep.Split('/')[3] + "/" + api_ep.Split('/')[4] + "/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest(req);
            int numOps = ser.Number_Of_Operands;
            int numRows = Grd.RowDefinitions.Count;
            
            int opcount = 0;
            int[] ops = new int[numOps];
            bool failed = false;

            foreach(Control ctr in Grd.Children)
            {
                if(ctr.GetType() == typeof(TextBox))
                {
                    TextBox tb = (TextBox)ctr;
                    if(int.TryParse(tb.Text, out ops[opcount]))
                    {
                        opcount++;
                    }
                    else
                    {
                        failed = true;
                        break;
                    }
                }
            }

            if(!failed)
            {
                PassObject<int[]> pass = new PassObject<int[]>();
                pass.Token = DataSingleton.Instance.token;
                pass.Pass = ops;
                request.AddJsonBody(pass);

                IRestResponse result = client.Post(request);
                ReturnObject<string> res = JsonConvert.DeserializeObject<ReturnObject<string>>(result.Content);
                string ans = res.Returned;

                if(res.Status.Equals("Denied"))
                {
                    ResponseText.Text = res.Status + ": " + res.Reason; 
                }
                else
                {
                    ResponseText.Text = ans;
                }

            }
            else
            {
                ResponseText.Text = "Invalid inputs, please enter integers!";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            caller.Show();
            this.Close();
        }
    }
}
