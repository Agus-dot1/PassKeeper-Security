using System.Security.Cryptography;
using System.Text;

namespace PassKeeper.Models;

public class MasterKeyModel
{
    public string? HashedKey { get; set; }

    public void SetMasterKey(string masterKeyText)
    {
        HashedKey = Hash(masterKeyText);
    }

    public void InitializeHashedKey(string hashedKeyFromFile)
    {
        HashedKey = hashedKeyFromFile;
    }

    public bool CheckMasterKey(string masterKeyText)
    {
        if (masterKeyText == null || masterKeyText == "") return false;
        return HashedKey == Hash(masterKeyText);
    }

    private static string Hash(string MasterKeyText)
    {
        var data = SHA256.HashData(Encoding.UTF8.GetBytes(MasterKeyText));
        return BitConverter.ToString(data).Replace("-", "").ToLower();
    }
}