using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace PasswordEncoding {
    public class ConnectionStringUtil {

        public static string UrlEncodeConnectionString(string connectionString) {
            // Regular expression pattern to match "Password=" followed by password value
            string pattern = @"(?<=Password=)([']?)([^;]+)([']?)";

            // Match the pattern within the connection string
            MatchCollection matches = Regex.Matches(connectionString, pattern);

            // If matches are found, encode the password values
            if (matches.Count > 0) {
                StringBuilder builder = new(connectionString);

                // Iterate through the matches in reverse order to handle overlapping matches
                for (int i = matches.Count - 1; i >= 0; i--) {
                    Match match = matches[i];
                    string passwordValue = match.Groups[2].Value;

                    // Remove the single quotes or double quotes from the password value, if present
                    string passwordWithoutQuotes = passwordValue.Trim('\'', '\"');

                    // URL encode the password value
                    string encodedPassword = HttpUtility.UrlEncode(passwordWithoutQuotes);

                    // Determine if the password had single quotes or double quotes originally
                    string quoteCharacter = match.Groups[1].Value;
                    //string encodedPasswordWithQuotes = quoteCharacter + encodedPassword + quoteCharacter;

                    // Replace the original password value with the encoded value
                    builder.Replace(passwordValue, encodedPassword, match.Index, match.Length);
                }

                connectionString = builder.ToString();
            }

            return connectionString;
        }

    }
}