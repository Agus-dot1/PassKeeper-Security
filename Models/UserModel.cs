using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace PassKeeper.Models
{
    public class UserModel
    {
        public string FilePath { get; set; }
        public MasterKeyModel MasterKey { get; set; }
        public ObservableCollection<Passwords> Passwords { get; set; }
        public ApplicationTheme CurrentTheme { get; set; } = ApplicationTheme.Light;


        public UserModel(string filePath, MasterKeyModel masterKey)
        {
            FilePath = filePath;
            MasterKey = masterKey;
            Passwords = new ObservableCollection<Passwords>();
        }

        public void AddPassword(Passwords password)
        {
            Passwords.Add(password);
            SaveToFile();
        }
        public void RemovePassword(Passwords password)
        {

        }

        public void SaveToFile()
        {
            string jsonData = JsonSerializer.Serialize(this);
            File.WriteAllText(FilePath, jsonData);
        }

        public static UserModel? LoadFromFile(string filePath)
        {
            if(!File.Exists(filePath)) return null;


            string jsonData = File.ReadAllText(filePath);
            UserModel? user = JsonSerializer.Deserialize<UserModel>(jsonData);
            return user;
        }





    }
}
