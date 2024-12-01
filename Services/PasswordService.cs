using System.Collections.ObjectModel;
using System.Data;
using MySqlConnector;
using PassKeeper.Db;

namespace PassKeeper.Services;
using Models;

public class PasswordService
{
    public async Task AddPassword(PasswordsModel password, string userId)
    {
        string query = @"INSERT INTO Passwords (id, site_name, username, site_url, user_id, note, icon, password_hash)
                         VALUES (@id, @Name, @Username, @Url, @UserId ,@Note, @Icon, @GeneratedPassword)";
        
        await using var connection = DatabaseConnection.GetConnection();
        await using var command = new MySqlCommand(query, connection);
        try
        {
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@id", password.Id);
            command.Parameters.AddWithValue("@Name", password.Name);
            command.Parameters.AddWithValue("@Username", password.Username);
            command.Parameters.AddWithValue("@Url", password.Url);
            command.Parameters.AddWithValue("@Note", password.Notes);
            command.Parameters.AddWithValue("@Icon", password.Icon);
            command.Parameters.AddWithValue("@GeneratedPassword", password.Password);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<ObservableCollection<PasswordsModel>> LoadPasswordsAsync(string userId)
    {
        var passwords = new ObservableCollection<PasswordsModel>();
        string query = "SELECT id, site_name, username, password_hash, site_url, note FROM Passwords WHERE user_id = @UserId";

        try
        {
            await using var connection = DatabaseConnection.GetConnection();
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                passwords.Add(new PasswordsModel
                {
                    Id = reader.GetString("Id"),
                    Name = reader.GetString("Name"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Url = reader["Url"] as string,
                    Notes = reader["Notes"] as string
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading passwords: {ex.Message}");
        }

        return passwords;
    }
}