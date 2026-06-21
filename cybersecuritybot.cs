using MySqlConnector;

namespace CybersecurityChatbot
{

    public static class DatabaseHelper
    {
        //
        private const string Server = "localhost";
        private const string Database = "PoePart3";
        private const string User = "root";
        private const string Password = "";   // 
        private const string Port = "3306";

        public static string ConnectionString =>
            $"Server={Server};Port={Port};Database={Database};User={User};Password={Password};";

        public static MySqlConnection GetConnection() => new MySqlConnection(ConnectionString);


        public static (bool success, string message) TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return (true, $"✅ Connected to MySQL database '{Database}' successfully.");
            }
            catch (MySqlException ex)
            {
                return (false, $"❌ Could not connect to MySQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"❌ Unexpected database error: {ex.Message}");
            }
        }
    }
}
