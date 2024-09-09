using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit_fixer.Pages.Todos;

public class Index : PageModel
{
    private readonly ITodosRepository todo_repo;
    private bool debug;

    public Index(ITodosRepository todosRepository)
    {
        this.todo_repo = todosRepository;
    }

    public void OnGet()
    {
        this.debug = true;
    }

    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        if (debug) Console.WriteLine($"{name}:{search_term}");
        Stopwatch watch = Stopwatch.StartNew();
        watch.Stop();
        var elapsed = watch.Elapsed;
        var all_todos = await todo_repo.GetAll();
        return Partial("_TodoTable", all_todos);
    }

    public async Task<IActionResult> OnGetTimeElapsed(string search_term, [CallerMemberName] string name = "")
    {
        try
        {
            if (debug) Console.WriteLine($"{name}:{search_term}");
            Stopwatch watch = Stopwatch.StartNew();
            using var connection = SqlConnections.CreateConnection();
            var time = (await connection.QueryAsync<TimeElapsed>(@"select id from TimeElapsed")).ToList();
            watch.Stop();
            var elapsed = watch.Elapsed;
            // return Content($"(mysql view call) total {time.Count} took {elapsed.Milliseconds} ms");

            return Partial("_TimeElapsedTable", time);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}