using BlackJack.Modelle;
using Xunit;

namespace BlackJack.Tests;

public class UserTests
{
    [Fact]
    public void NewUser_ShouldStartWithDefaultValues()
    {
        // Dieser Test ist ein einfaches Beispiel für GitHub Actions:
        // In der Pipeline wird er automatisch mit "dotnet test" ausgeführt.
        User user = new();

        Assert.Equal(0, user.Id);
        Assert.Equal("", user.Username);
        Assert.Equal("", user.PasswordHash);
        Assert.Equal(1000m, user.Balance);
    }

    [Fact]
    public void UserProperties_ShouldStoreAssignedValues()
    {
        User user = new()
        {
            Id = 7,
            Username = "Bruno",
            PasswordHash = "hashed-password",
            Balance = 1250.50m,
        };

        Assert.Equal(7, user.Id);
        Assert.Equal("Bruno", user.Username);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal(1250.50m, user.Balance);
    }

    [Theory]
    [InlineData(1000, 100, 1100)]
    [InlineData(1000, -250, 750)]
    [InlineData(1250.50, -250.50, 1000)]
    public void Balance_ShouldReflectGameResults(decimal startingBalance, decimal gameResult, decimal expectedBalance)
    {
        // [Theory] prüft dieselbe Logik mit mehreren Datensätzen.
        // Das ist praktisch, weil CI-Systeme viele Fälle automatisch abarbeiten können.
        User user = new()
        {
            Balance = startingBalance,
        };

        user.Balance += gameResult;

        Assert.Equal(expectedBalance, user.Balance);
    }
}
