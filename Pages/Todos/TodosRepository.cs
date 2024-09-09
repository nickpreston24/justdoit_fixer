using Dapper;
using justdoit_fixer.Models;
using MySql.Data.MySqlClient;

namespace justdoit_fixer.Pages.Todos;

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