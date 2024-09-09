namespace justdoit_fixer.Models;

public class Todo
{
    public bool is_recurring { set; get; } = false;
    public int id { get; set; } = -1;
    public string uri { get; set; } = string.Empty; // link to an individual record.
    public string content { get; set; } = string.Empty;
    public string description { set; get; }
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public int priority { get; set; } = 4;
    public string priority_css { get; set; } = "warning";

    public DateTime due { get; set; } = DateTime.MinValue; // = Start.Add(Duration);
}