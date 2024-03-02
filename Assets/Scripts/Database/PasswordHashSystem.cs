using System.Security.Cryptography;
using System.Text;

public class PasswordHashSystem
{
    // Password hashing system
    public static string HashPassword(string password)
    {
        // Create SHA256 hash
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Compute hash from UTF-8 encoded password
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to hexadecimal string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                // Convert each byte of the hash to a two-digit hexadecimal representation
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
