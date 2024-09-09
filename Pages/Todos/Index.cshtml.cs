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


    public Task<List<Todo>> Search(Todo search)
    {
        throw new NotImplementedException();
    }

    public Task<Todo> GetById(int id)
    {
        throw new NotImplementedException();
    }

    // public async Task<int> Create(params Todo[] model)
    // {
    //     return await InsertRow(model.First());
    // }

    public Task Update(int id, Todo model)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Delete(int id)
    {
        Console.WriteLine(id);
        using var connection = SqlConnections.CreateConnection();

        string query = @"
            delete from todos where id = @id
        ";

        var rows = await connection.ExecuteAsync(query, new { id = id });
        return rows;
    }

    public async Task<int> GetRowCount()
    {
        string query = @"
                        select count(id)
                        from todos;";

        using var connection = SqlConnections.CreateConnection();
        var rows = await connection.ExecuteAsync(query);
        return rows;
    }

    public async Task<List<string>> FindTables()
    {
        using var connection = SqlConnections.CreateConnection();

        // todo: you've implemented this somewhere else already.  Go find it and upload it as a myget, then call it here.

        // var tables = await connection.QueryAsync<SQLiteTableInfo>("SELECT * FROM sqlite_master WHERE type='table'");
        // var tableNames = tables.Dump("tables found");
        // return tableNames.ToList();

        return new List<string>(0);
    }

    // private async Task<int> InsertRow(Todo todo)
    // {
    //     try
    //     {
    //         using var connection = SqlConnections.CreateConnection();
    //
    //         string insert_query =
    //             @$"insert into todos (content, priority, status, due) values (@content, @priority, '{TodoStatus.Pending.Name}', @due)";
    //
    //         var extracted_priority = todo
    //             // .Dump("my todo added")
    //             .content
    //             .Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
    //             // .Dump("priori incantum")
    //             .SingleOrDefault();
    //
    //         // extracted_priority.Dump(nameof(extracted_priority));
    //
    //         var results = await Dapper.SqlMapper
    //             .ExecuteAsync(connection, insert_query,
    //                 new
    //                 {
    //                     content = todo.content,
    //                     priority = extracted_priority?.Value ?? 4,
    //                     status = todo.status,
    //                     due = todo.due
    //                 });
    //
    //         return results;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //
    //         throw;
    //     }
    // }
}