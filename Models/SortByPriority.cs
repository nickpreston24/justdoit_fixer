namespace justdoit_fixer.Models;

public class SortByPriority : Sort
{
    public override SortDirection Direction { get; set; } = SortDirection.Descending;
    public override bool Enabled { get; set; } = true;
}