using Hydro;

namespace justdoit_fixer.Pages.Components;

public class Counter : HydroComponent
{
    public int Count { get; set; }

    public void Add()
    {
        Count++;
    }
}