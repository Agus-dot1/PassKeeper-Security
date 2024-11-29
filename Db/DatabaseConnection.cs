using MySqlConnector;

namespace PassKeeper.Db
{
    public static class DatabaseConnection
    {
        private const string ConnectionString = "";

        public static MySqlConnection GetConnection()
        {
            try
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Couldn't connect to database: {ex.Message}");
                throw;
            }
        }
    }
}
