using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace PassKeeper.Models;

public class UserModel
{
    public string FilePath { get; set; }
    public MasterKeyModel MasterKey { get; set; }
    public ObservableCollection<PasswordModel> Passwords { get; set; }


    public UserModel(MasterKeyModel masterKey)
    {
        var dbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases");
        FilePath = Path.Combine(dbDirectory, "database.pks");
        Directory.CreateDirectory(dbDirectory);

        MasterKey = masterKey;
        Passwords = new ObservableCollection<PasswordModel>();
    }

    public void AddPassword(PasswordModel passwordModel)
    {
        Passwords.Add(passwordModel);
        SaveToFile();
    }

    public void RemovePassword(PasswordModel passwordModel)
    {
        Passwords.Remove(passwordModel);
        SaveToFile();
    }

    public void SaveToFile()
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(this);
            File.WriteAllText(FilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public static UserModel? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return null;


        try
        {
            var jsonData = File.ReadAllText(filePath);
            var user = JsonSerializer.Deserialize<UserModel>(jsonData);
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            return null;
        }
    }
}