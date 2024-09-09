using CodeMechanic.Types;
using MySql.Data.MySqlClient;

namespace justdoit_fixer.Pages.Todos;

public static class SqlConnections
{
    public static MySqlConnection CreateConnection() => GetMySQLConnectionString().AsConnection();

    public static MySqlConnection AsConnection(this string connectionString) => new MySqlConnection(connectionString);


    public static string GetMySQLConnectionString()
    {
        var connectionString = new MySqlConnectionStringBuilder()
        {
            Database = Environment.GetEnvironmentVariable("MYSQLDATABASE"),
            Server = Environment.GetEnvironmentVariable("MYSQLHOST"),
            Password = Environment.GetEnvironmentVariable("MYSQLPASSWORD"),
            UserID = Environment.GetEnvironmentVariable("MYSQLUSER"),
            Port = (uint)Environment.GetEnvironmentVariable("MYSQLPORT").ToInt()
        }.ToString();

        if (connectionString.IsEmpty()) throw new ArgumentNullException(nameof(connectionString));
        return connectionString;
    }
}