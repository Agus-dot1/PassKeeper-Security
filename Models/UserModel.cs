using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Wpf.Ui.Appearance;

namespace PassKeeper.Models
{
    public class UserModel
    {
        public string FilePath { get; set; }
        public MasterKeyModel MasterKey { get; set; }
        public ObservableCollection<Passwords> Passwords { get; set; }


        public UserModel(MasterKeyModel masterKey)
        {
            string dbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases");
            FilePath = Path.Combine(dbDirectory, "database.pks");
            Directory.CreateDirectory(dbDirectory);

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
            Passwords.Remove(password);
            SaveToFile();
        }

        public void SaveToFile()
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(this);
                File.WriteAllText(FilePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
            }
        }

        public static UserModel? LoadFromFile(string filePath)
        {
            if(!File.Exists(filePath)) return null;


            try
            {
                string jsonData = File.ReadAllText(filePath);
                UserModel? user = JsonSerializer.Deserialize<UserModel>(jsonData);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo: {ex.Message}");
                return null;
            }
        }





    }
}
