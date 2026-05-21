namespace BlackJack.Modelle
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public decimal Balance { get; set; } = 1000;
    }
}