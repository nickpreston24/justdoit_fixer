using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace justdoit_fixer.Pages.Todos;

public class Index : PageModel
{
    public void OnGet()
    {
    }
}

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

public interface ITodosRepository
{
    Task<List<Todo>> GetAll();
}

public class Todo
{
    public bool is_recurring { set; get; } = false;
    public int id { get; set; } = -1;
    public string uri { get; set; } = string.Empty; // link to an individual record.
    public string content { get; set; } = string.Empty;
    public string description { set; get; }
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;

    public DateTime due { get; set; } = DateTime.MinValue; // = Start.Add(Duration);
}

public class TodosRepository : ITodosRepository
{
    public async Task<List<Todo>> GetAll()
    {
        var connectionString = SqlConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string select_query = @"
            select id, content, due, status, priority
            from todos;
        ";

        using var grabby_connection = new MySqlConnection(connectionString);

        var todos = (await connection.QueryAsync<Todo>(select_query)).ToList();

        return todos;
    }
}