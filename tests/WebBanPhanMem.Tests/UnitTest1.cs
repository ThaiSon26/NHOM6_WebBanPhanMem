using WebBanPhanMem.Helpers;

namespace WebBanPhanMem.Tests;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ShouldReturnValidHash_WhenPasswordIsProvided()
    {
        // Arrange
        string rawPassword = "UserSecurePassword@2026";

        // Act
        string hash = PasswordHelper.HashPassword(rawPassword);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.Equal(64, hash.Length); // SHA-256 generates a 64-char hex string
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        string password = "AdminPassword@123";
        string hash = PasswordHelper.HashPassword(password);

        // Act
        bool isValid = PasswordHelper.VerifyPassword(password, hash);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForWrongPassword()
    {
        // Arrange
        string password = "CorrectPassword@123";
        string wrongPassword = "IncorrectPassword@123";
        string hash = PasswordHelper.HashPassword(password);

        // Act
        bool isValid = PasswordHelper.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyPassword_ShouldSupportPlaintextFallback()
    {
        // Arrange
        string plainPassword = "PlainTextPassword123";

        // Act
        bool isValid = PasswordHelper.VerifyPassword(plainPassword, plainPassword);

        // Assert
        Assert.True(isValid);
    }
}
