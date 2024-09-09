using System.Text.RegularExpressions;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;
using Dapper;
using justdoit_fixer.Pages.Todos;
using MySql.Data.MySqlClient;

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

    public async Task<int> Create(params Todo[] model)
    {
        return await InsertRow(model.First());
    }

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

    private async Task<int> InsertRow(Todo todo)
    {
        try
        {
            using var connection = SqlConnections.CreateConnection();

            string insert_query =
                @$"insert into todos (content, priority, status, due) values (@content, @priority, '{TodoStatus.Pending.Name}', @due)";

            var extracted_priority = todo
                // .Dump("my todo added")
                .content
                .Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
                // .Dump("priori incantum")
                .SingleOrDefault();

            // extracted_priority.Dump(nameof(extracted_priority));

            var results = await Dapper.SqlMapper
                .ExecuteAsync(connection, insert_query,
                    new
                    {
                        content = todo.content,
                        priority = extracted_priority?.Value ?? 4,
                        status = todo.status,
                        due = todo.due
                    });

            return results;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }
}


public interface ITodosRepository
{
    Task<List<Todo>> GetAll();
    Task<List<Todo>> Search(Todo search);
    Task<Todo> GetById(int id);
    Task<int> Create(params Todo[] model);
    Task Update(int id, Todo model);
    Task<int> Delete(int id);
    Task<int> GetRowCount();
    Task<List<string>> FindTables();
}


public record Todo
{
    public static Todo Create(string description)
    {
        Priority extracted_priority = description
                                          .Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
                                          .SingleOrDefault()
                                      ?? "4";

        // todo:
        var now = DateTime.Now;
        // Due extracted_due_date = description
        //                              .Extract<Due>(TodoPriorityRegex.Basic.CompiledRegex)
        //                              .SingleOrDefault()
        //                          ?? now;

        // var extracted_status = description.Extract<TodoStatus>(@"(?<status>(pending))").SingleOrDefault() ?? "pending";

        // extracted_status.Dump("status");

        // due = extracted_due_date
        return new Todo()
        {
            content = description,
            priority = extracted_priority.Value,
            //		status = TodoStatus.Pending
        };
    }

    public bool is_recurring { set; get; } = false;
    public int id { get; set; } = -1;
    public string uri { get; set; } = string.Empty; // link to an individual record.
    public string content { get; set; } = string.Empty;
    public string description { set; get; }
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public TodoStatus Status => status;

    public string[] labels => content
        .Extract<TodoLabel>(@"(?<name>@\w+)")
        .Select(label => label.name)
        .ToArray(); // https://regex101.com/r/UPGuX2/1

    public int priority { get; set; } = 4;

    public DateTime due { get; set; } = DateTime.MinValue; // = Start.Add(Duration);
    public TimeSpan duration { get; set; } = TimeSpan.FromMinutes(15);
    public DateTime start { get; set; }
    public DateTime end { get; set; }

    public DateTime created_at { get; set; } = DateTime.MinValue;
    public DateTime last_modified { get; set; } = DateTime.MinValue;

    public string status_css
    {
        get
        {
            var options = new Dictionary<TodoStatus, string>()
            {
                [TodoStatus.Done] = "success",
                [TodoStatus.WIP] = "warning",
                [TodoStatus.Postponed] = "red-500",
                [TodoStatus.Pending] = "info",
                [TodoStatus.Unknown] = "red-500",
            };

            var found = options.TryGetValue(status, out var value);
            // Console.WriteLine("value is " + value);
            return found ? value : throw new Exception($"status '{status}' found");
        }
    }

    public string priority_css
    {
        get
        {
            var value = priority;
            switch (value)
            {
                case 1:
                    return "error";
                case 2:
                    return "warning";
                case 3:
                    return "info";
                case 4:
                default:
                    return "ghost";
            }
        }
    }
}

public record TodoLabel
{
    public string name { get; set; } = string.Empty;
}


public static class TodoExtensions
{
    //...
}

public class Due
{
    public string date { get; set; } = string.Empty;
    public string due_string { get; set; }

    // [JsonProperty("string")] public string @string { get; set; }

    // public string friendly {get;set;} = "Feb 12";
    public string lang { get; set; } = "en";
    public string is_recurring { get; set; } = "false";

    public DateTime datetime => date.ToDateTime(fallback: DateTime.MinValue).Value;

    public string friendly_date => datetime.ToFriendlyDateString();
    public string humanized_age => datetime.HumanizeAge();
    public string humanized => datetime.Humanize().ToMaybe().IfNone("Unknown");
}

public class TodoStatus : Enumeration
{
    public static TodoStatus Done = new TodoStatus(1, nameof(Done));
    public static TodoStatus Pending = new TodoStatus(2, nameof(Pending));
    public static TodoStatus WIP = new TodoStatus(3, nameof(WIP));
    public static TodoStatus Postponed = new TodoStatus(4, nameof(Postponed));
    public static TodoStatus Unknown = new TodoStatus(5, nameof(Unknown));

    public TodoStatus(int id, string name) : base(id, name)
    {
    }

    public static implicit operator TodoStatus(string status)
    {
        if (status.IsEmpty())
            return Unknown;
        var
            found = TodoStatus
                .GetAll<TodoStatus>()
                .SingleOrDefault(x => x.Name.Equals(status, StringComparison.CurrentCultureIgnoreCase));
        return found;
    }
}


public record Priority
{
    public string raw_text { get; set; } = string.Empty; // e.g. p1
    public string friendly_name => $"Priority {Value}"; // e.g. 'Priority 1'
    public int Value { get; set; } = -1;
    public static implicit operator Priority(string priority) => new Priority(priority);
    public static implicit operator Priority(int priority) => new Priority(priority.ToString());
}


public class TodoPriorityRegex : RegexEnumBase
{
    public static TodoPriorityRegex Basic =
        new TodoPriorityRegex(1, nameof(Basic), @"(?<raw_text>(priority\s*|p)(?<Value>[1-4]))",
            "https://regex101.com/r/twefSL/1"); // return that part to autozone tomorrow p1

    protected TodoPriorityRegex(int id, string name, string pattern, string uri = "") : base(id, name, pattern, uri)
    {
    }
}

public class RegexEnumBase : Enumeration
{
    protected RegexEnumBase(int id, string name, string pattern, string uri = "") : base(id, name)
    {
        Pattern = pattern;
        CompiledRegex =
            new System.Text.RegularExpressions.Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        this.uri = uri;
    }

    public string uri { get; set; } = string.Empty;

    public Regex CompiledRegex { get; set; }
    public string Pattern { get; set; }
}