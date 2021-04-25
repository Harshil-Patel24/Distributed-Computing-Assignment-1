using System;
using System.Collections.Generic;
using System.Windows;
using RestSharp;
using DataClasses;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Runtime.Remoting.Messaging;

namespace Client
{
    //This class is used to search a service
    public partial class SearchServiceWindow : Window
    {
        public delegate ReturnObject<List<ServiceModel>> SearchDel(RestClient client, RestRequest req);

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
            try
            {
                SearchDel search;
                AsyncCallback callback;

                //Clearing children to only show results in current search
                sp.Children.Clear();
                PassObject<string> pass = new PassObject<string>();
                pass.Token = DataSingleton.Instance.token;
                pass.Pass = SearchText.Text;

                string URL = "https://localhost:44358/";
                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest("api/search");
                request.AddJsonBody(pass);

                search = this.SearchService;
                callback = this.OnSearchCompletion;

                search.BeginInvoke(client, request, callback, null);

                WaitingBar.Visibility = Visibility.Visible;
                WaitingBar.IsIndeterminate = true;
                WaitingLabel.Visibility = Visibility.Visible;
                //IRestResponse response = client.Post(request);

                //ReturnObject<List<ServiceModel>> ret = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(response.Content);
                //List<ServiceModel> services = ret.Returned;

                ////Checks if the called service denied user service
                //if (!ret.Status.Equals("Denied"))
                //{
                //    DataSingleton.Instance.serviceModel = services;
                //    //Creates buttons for each service result
                //    foreach (ServiceModel service in services)
                //    {
                //        Button newBut = new Button();
                //        newBut.Content = service.Name;
                //        newBut.Name = service.Name;

                //        newBut.Click += new RoutedEventHandler(Button_Click);
                //        sp.Children.Add(newBut);
                //    }
                //}
                //else
                //{
                //    ErrorText.Text = ret.Status + ": " + ret.Reason;
                //}

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSearchCompletion(IAsyncResult ar)
        {
            SearchDel search;
            AsyncResult asyncObj = (AsyncResult)ar;
            ReturnObject<List<ServiceModel>> retobj;

            if(asyncObj.EndInvokeCalled == false)
            {
                search = (SearchDel)asyncObj.AsyncDelegate;
                retobj = search.EndInvoke(asyncObj);

                List<ServiceModel> services = retobj.Returned;

                //Checks if the called service denied user service
                if (!retobj.Status.Equals("Denied"))
                {
                    Dispatcher.Invoke(() =>
                    {
                        WaitingBar.Visibility = Visibility.Hidden;
                        WaitingBar.IsIndeterminate = false;
                        WaitingLabel.Visibility = Visibility.Hidden;

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
                    });
                }
                else
                {
                    ErrorText.Text = retobj.Status + ": " + retobj.Reason;
                }
            }
            asyncObj.AsyncWaitHandle.Close();
        }

        private ReturnObject<List<ServiceModel>> SearchService(RestClient client, RestRequest request)
        {
            IRestResponse response = client.Post(request);

            ReturnObject<List<ServiceModel>> ret = JsonConvert.DeserializeObject<ReturnObject<List<ServiceModel>>>(response.Content);
            //List<ServiceModel> services = ret.Returned;

            ////Checks if the called service denied user service
            //if (!ret.Status.Equals("Denied"))
            //{
            //    DataSingleton.Instance.serviceModel = services;
            //    //Creates buttons for each service result
            //    foreach (ServiceModel service in services)
            //    {
            //        Button newBut = new Button();
            //        newBut.Content = service.Name;
            //        newBut.Name = service.Name;

            //        newBut.Click += new RoutedEventHandler(Button_Click);
            //        sp.Children.Add(newBut);
            //    }
            //}
            //else
            //{
            //    ErrorText.Text = ret.Status + ": " + ret.Reason;
            //}

            return ret;

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
