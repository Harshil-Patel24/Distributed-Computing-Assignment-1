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

namespace Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void ShowAllServicesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowServicesWindow showin = new ShowServicesWindow();
            showin.Show();
            this.Hide();
        }

        private void SearchAServiceButton_Click(object sender, RoutedEventArgs e)
        {
            SearchServiceWindow searchwin = new SearchServiceWindow();
            searchwin.Show();
            this.Hide();
        }

        //private void TestAServiceButton_Click(object sender, RoutedEventArgs e)
        //{
        //    TestServiceWindow testwin = new TestServiceWindow();
        //    testwin.Show();
        //    this.Hide();
        //}

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            //Remove the authentication token first -- yet to be implemented
            MainWindow mainwin = new MainWindow();
            DataSingleton.Instance.token = 0;
            mainwin.Show();
            this.Hide();
        }
    }
}
