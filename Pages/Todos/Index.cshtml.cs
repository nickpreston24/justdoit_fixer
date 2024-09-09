using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;
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
        watch.Stop();
        var elapsed = watch.Elapsed;
        var all_todos = new Todo().AsList();
        return Partial("_TodoTable", all_todos);
    }
}