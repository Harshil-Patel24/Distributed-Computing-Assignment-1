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
using System.Runtime.Remoting.Messaging;

namespace Client
{
    //Tests a service
    public partial class TestServiceWindow : Window
    {
        private ServiceModel ser;
        private Window caller;

        //Use this for multithreading
        public delegate string TestDel(RestClient client, RestRequest req);

        public TestServiceWindow(Window call, string api_endpoint)
        {
            InitializeComponent();
            caller = call;

            ser = DataSingleton.FindService(api_endpoint);
            NameLabel.Content = ser.Name;

            //Creates a grid with the required inputs for a service
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

        //Executes the service requests
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestDel testDel;
                AsyncCallback callback;

                string api_ep = ser.API_Endpoint;
                string URL = api_ep.Split('/')[0] + "/" + api_ep.Split('/')[1] + "/" + api_ep.Split('/')[2] + "/";
                string req = api_ep.Split('/')[3] + "/" + api_ep.Split('/')[4] + "/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest(req);

                testDel = this.TestService;
                callback = this.OnServiceCompletion;

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

                    testDel.BeginInvoke(client, request, callback, null);

                    WaitingBar.Visibility = Visibility.Visible;
                    WaitingBar.IsIndeterminate = true;
                    WaitingLabel.Visibility = Visibility.Visible;

                }
                else
                {
                    ResponseText.Text = "Invalid inputs, please enter integers!";
                }
            }
            catch(Exception ex)
            {

            }
        }

        //Wrapper method for rest pose request
        private string TestService(RestClient client, RestRequest request)
        {
            IRestResponse result = client.Post(request);
            ReturnObject<string> res = JsonConvert.DeserializeObject<ReturnObject<string>>(result.Content);
            string ans = res.Returned;
            string resp;

            if (res.Status.Equals("Denied"))
            {
                resp = res.Status + ": " + res.Reason;
            }
            else
            {
                resp = ans;
            }

            return resp;
        }

        private void OnServiceCompletion(IAsyncResult asyncResult)
        {
            string ans;
            //IRestResponse response;
            TestDel testDel;
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            if(asyncObj.EndInvokeCalled == false)
            {
                testDel = (TestDel)asyncObj.AsyncDelegate;
                ans = testDel.EndInvoke(asyncObj);

                Dispatcher.Invoke(() => 
                { 
                    ResponseText.Text = ans;
                    WaitingBar.Visibility = Visibility.Hidden;
                    WaitingBar.IsIndeterminate = false;
                    WaitingLabel.Visibility = Visibility.Hidden;
                });
               
            }
            asyncObj.AsyncWaitHandle.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            caller.Show();
            this.Close();
        }
    }
}
