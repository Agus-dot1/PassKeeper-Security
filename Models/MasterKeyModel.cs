using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PassKeeper.Models
{
    public class MasterKeyModel
    {
        public string HashedKey { get; private set; }

        public void SetMasterKey(string MasterKeyText)
        {
            HashedKey = Hash(MasterKeyText);
        }
        public void InitializeHashedKey(string hashedKeyFromFile)
        {
            HashedKey = hashedKeyFromFile;
        }
        public bool CheckMasterKey(string MasterKeyText)
        {
            if (MasterKeyText == null  || MasterKeyText == "") return false;
            return HashedKey == Hash(MasterKeyText);
        }

        private string Hash(string MasterKeyText)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(MasterKeyText));
                return BitConverter.ToString(data).Replace("-", "").ToLower();
            }
        }
    }
}
