namespace BlackJack.Modelle
{
    // Kleine Modellklasse für die Tests: An ihr lässt sich gut zeigen,
    // dass GitHub Actions nach jedem Push automatisch fachliche Regeln prüfen kann.
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public decimal Balance { get; set; } = 1000;
    }
}
