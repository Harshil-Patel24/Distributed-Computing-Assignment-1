using System.Windows;


namespace Client
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        //Takes user to show all services
        private void ShowAllServicesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowServicesWindow showin = new ShowServicesWindow();
            showin.Show();
            this.Hide();
        }

        //Takes user to search a service
        private void SearchAServiceButton_Click(object sender, RoutedEventArgs e)
        {
            SearchServiceWindow searchwin = new SearchServiceWindow();
            searchwin.Show();
            this.Hide();
        }

        //Logs user out and takes them back to login window
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = new MainWindow();
            DataSingleton.Instance.token = 0;
            mainwin.Show();
            this.Hide();
        }
    }
}
