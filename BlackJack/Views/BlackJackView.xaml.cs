using System.Windows;

namespace BlackJack.Views
{
    public partial class BlackjackView : Window
    {
        public BlackjackView()
        {
            InitializeComponent();
        }

        private async void Hit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Karte gezogen!");
        }

        private async void Stand_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dealer ist dran!");
            await Task.Delay(2000);
            MessageBox.Show("Der Dealer hat Gezogen!");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            MessageBox.Show("Die meisten hören vor dem großen Gewinn auf!   ~Auf Wiedersehen");

            this.Close();
        }
    }
}