using PasswordEncoding;

using Xunit;

namespace PasswordEncodingTests;
public class Class1Tests {
    [Fact]
    public void UrlEncodeConnectionString_WithEncodedPassword_Success() {
        // Arrange
        string connectionString = "Password=password%1;UserId=username;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "Password=password%251;UserId=username;";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithEncodedPasswordAtEnd_Success() {
        // Arrange
        string connectionString = "UserId=username;Password=password%1;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "UserId=username;Password=password%251;";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithEncodedPasswordWithQuotesAtEnd_Success() {
        // Arrange
        string connectionString = "UserId=username;Password='password%1;';";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "UserId=username;Password='password%251;';";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithEncodedPasswordWithQuotesAtEnd_Success2() {
        // Arrange
        string connectionString = "Password='password%1;';UserId=username;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "Password='password%251;';UserId=username;";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithoutPassword_NoChange() {
        // Arrange
        string connectionString = "Server=example.com;Database=mydb;UserId=username;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        Assert.Equal(connectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithMultiplePasswords_EncodeFirstOnly() {
        // Arrange
        string connectionString = "Password=pass%3Bword%1;UserId=username;Password=abc%3Bdef;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "Password=pass%253Bword%251;UserId=username;Password=abc%3Bdef;";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_WithNoPassword() {
        // Arrange
        string connectionString = "UserId=username;";

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        string expectedConnectionString = "UserId=username;";
        Assert.Equal(expectedConnectionString, encodedConnectionString);
    }

    [Fact]
    public void UrlEncodeConnectionString_EmptyString_NoChange() {
        // Arrange
        string connectionString = string.Empty;

        // Act
        string encodedConnectionString = ConnectionStringUtil.UrlEncodeConnectionString(connectionString);

        // Assert
        Assert.Equal(connectionString, encodedConnectionString);
    }
}