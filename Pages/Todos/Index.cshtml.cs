using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using justdoit_fixer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit_fixer.Pages.Todos;

public class Index : PageModel
{
    public Todo Todo { get; set; } = new Todo() { };
    public string[] ViewNames { get; set; } = new[] { "TimeElapsed" };


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        Console.WriteLine(nameof(OnGetAllTodos));
        // if (debug)
        Console.WriteLine($"{name}:{search_term}");
        Stopwatch watch = Stopwatch.StartNew();

        using var connection = SqlConnections.CreateConnection();
        var all_todos = (await connection.QueryAsync<Todo>(
                "select * from todos"
            ))
            .Where(t => !t.is_archived && t.is_enabled)
            .ToList();

        // var all_todos = new Todo().AsList();

        watch.Stop();
        var elapsed = watch.Elapsed;
        return Partial("_TodoTable", all_todos);
    }


    /// <summary>
    /// Renders a Mysql view requested from the frontend
    /// </summary>
    /// <param name="view_name"></param>
    /// <param name="result_type"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnGetRenderView(
        string view_name
        , Type result_type
        , bool debug = false
        , [CallerMemberName] string name = ""
    )
    {
        try
        {
            Stopwatch watch = Stopwatch.StartNew();
            using var connection = SqlConnections.CreateConnection();
            var view_results = (await connection.QueryAsync(@"select id from TimeElapsed")).ToList();
            watch.Stop();
            var elapsed = watch.Elapsed;

            return Partial(view_name, view_results);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> OnPostAddTodo()
    {
        string query = @"insert into todos (content) values (@content) ";
        int rows = 0;
        using var connection = SqlConnections.CreateConnection();
        rows = await connection.ExecuteAsync(query, new Todo
        {
            content = Todo.content
        });

        return Content($"added {rows} rows.");
    }

    public async Task<IActionResult> OnPostRemoveTodo()
    {
        Console.WriteLine(nameof(OnPostRemoveTodo));
        int rows = -1;
        return Content($"removed {rows} rows.");
    }

    public async Task<IActionResult> OnGetMarkDone(int id = 0, bool value = false)
    {
        Console.WriteLine(nameof(OnGetMarkDone));
        Console.WriteLine("Id: >> " + id);
        Console.WriteLine("toggle value: >> " + value);

        var now = DateTime.UtcNow;
        var last_modified = now;

        using var connection = SqlConnections.CreateConnection();
        string query =
            @"update todos 
            set status = 'done' 
            , last_modified = @last_modified
            where id = @id";

        int affected = await connection.ExecuteAsync(query, new
        {
            last_modified = last_modified,
            id = id
        });
        string message = $"{affected} row affected.";

        Console.WriteLine(message);
        return Content(message);


        // string html = @"""
        //                 <input
        //                     hx-get
        //                     hx-page='Index'
        //                     hx-page-handler='MarkDone'
        //                     type='checkbox' checked class='checkbox'/>
        //             """;
    }
}