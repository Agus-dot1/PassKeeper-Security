using System.Security.Cryptography;
using System.Text;

namespace PassKeeper.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        string enteredPasswordHash = HashPassword(enteredPassword);
        return enteredPasswordHash == storedHash;
    }
}