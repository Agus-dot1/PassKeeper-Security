using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassKeeper.Models
{
    public class UserModel
    {
        public string FilePath { get; set; }
        public MasterKeyModel MasterKey { get; set; }

        public UserModel(string FilePath, MasterKeyModel MasterKey)
        {
            this.FilePath = FilePath;
            this.MasterKey = MasterKey;
        }
    }
}
