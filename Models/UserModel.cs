using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using MySqlConnector;
using PassKeeper.Db;
using PassKeeper.Helpers;

namespace PassKeeper.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public MasterKeyModel MasterKey { get; set; }
        public ObservableCollection<PasswordsModel> Passwords { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsRegistered { get; set; }

        public UserModel()
        {
            string dbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases");
            FilePath = Path.Combine(dbDirectory, "database.pks");
            Directory.CreateDirectory(dbDirectory);
            
            CreationDate = DateTime.Now;
            Passwords = new ObservableCollection<PasswordsModel>();
        }

        public void AddPassword(PasswordsModel password)
        {
            Passwords.Add(password);
            SaveToFile();
        }
        public void RemovePassword(PasswordsModel password)
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
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public async Task Register()
        {
            try
            {
                string query = @"INSERT INTO Users (id, email, password_hash, created_at) 
                               VALUES (@Id, @Email, @PasswordHash, @CreationDate)
                               ON DUPLICATE KEY UPDATE 
                                   email = @Email, 
                                   password_hash = @PasswordHash, 
                                   created_at = @CreationDate";

                await using var connection = DatabaseConnection.GetConnection();
                await using var command = new MySqlCommand(query, connection);
                
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@Email", Email);
                command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
                command.Parameters.AddWithValue("@CreationDate", CreationDate);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                Console.WriteLine($"{rowsAffected} rows affected.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task<bool> Login(string email, string password)
        {
            try
            {
                const string query = "SELECT password_hash FROM Users WHERE email = @Email";

                await using var connection = DatabaseConnection.GetConnection();
                await using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var storedPassword = reader["password_hash"].ToString();
                    bool passwordMatch = storedPassword != null && PasswordHasher.VerifyPassword(password, storedPassword);
                    return passwordMatch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
                return false;
            }
            return false;
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
                Console.WriteLine($"Error loading from file: {ex.Message}");
                return null;
            }
        }

    }
}
