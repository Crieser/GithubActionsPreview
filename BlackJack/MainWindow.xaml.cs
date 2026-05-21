using System.Windows;
using BlackJack.Views;
namespace BlackJack
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            BlackjackView blackjack = new BlackjackView();
            blackjack.Show();

            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginView login = new LoginView();
            login.Show();

            this.Close();
        }
    }
}