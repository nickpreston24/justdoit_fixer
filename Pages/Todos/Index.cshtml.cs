using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeMechanic.Diagnostics;
using Dapper;
using justdoit_fixer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit_fixer.Pages.Todos;

public class Index : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    [BindProperty(SupportsGet = true)] public Todo Todo { get; set; } = new Todo() { };

    [BindProperty(SupportsGet = true)] public string Email { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string Content { get; set; } = string.Empty;

    public string[] ViewNames { get; set; } = new[] { "_TimeElapsedTable" };

    public Index(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task OnGet()
    {
    }

    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        // Console.WriteLine(nameof(OnGetAllTodos));

        // string json = await GetMyInternalAPISampleTodos();
        // Console.WriteLine("sample api todos :>> " + json);

        // if (debug)
        Console.WriteLine($"{name}:{search_term}");
        Stopwatch watch = Stopwatch.StartNew();

        using var connection = SqlConnections.CreateConnection();

        Func<Todo, bool> filters = todo =>
            // !todo.is_archived
            // || todo.is_enabled
            // && 
            !todo.status.ToLower().Equals("done");

        var all_todos = (
                await connection.QueryAsync<Todo>(
                    "select * from todos"
                ))
            .Where(filters)
            .ToList();

        // var all_todos = new Todo().AsList();

        watch.Stop();
        var elapsed = watch.Elapsed;
        return Partial("_TodoTable", all_todos);
    }

    /// <summary>
    /// for testing my railway api
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<string> GetMyInternalAPISampleTodos()
    {
        // get sample api todos:
        try
        {
            // var client = _httpClientFactory.CreateClient();
            var client = new HttpClient();
            var response = await client.GetAsync("https://justdoitapi-production.up.railway.app/todos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
        Console.WriteLine(name);
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

    public async Task<IActionResult> OnPostAddTodo(string content = "")
    {
        string query = @"insert into todos (content) values (@content) ";

        Todo.Dump("adding new todo");
        Console.WriteLine("content = " + content);

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

    public async Task<IActionResult> OnGetBumpTodo(int id = 0, string days = "7d")
    {
        return default;
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