using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;
using Dapper;
using justdoit_fixer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit_fixer.Pages.Todos;

public class Index : PageModel
{
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetStuff()
    {
        return Content("hi");
    }

    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        Console.WriteLine(nameof(OnGetAllTodos));
        // if (debug)
        Console.WriteLine($"{name}:{search_term}");
        Stopwatch watch = Stopwatch.StartNew();

        Console.WriteLine(SqlConnections.GetMySQLConnectionString());
        using var connection = SqlConnections.CreateConnection();
        var all_todos = (await connection.QueryAsync<Todo>(
                "select * from todos"
            ))
            .ToList();

        // var all_todos = new Todo().AsList();

        watch.Stop();
        var elapsed = watch.Elapsed;
        return Partial("_TodoTable", all_todos);
    }
}