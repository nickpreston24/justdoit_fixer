// using CodeMechanic.RegularExpressions;  NOTE: importing this dll breaks microsoft.  I don't know why.  I think it's because MS somehow interprets 'regularexpressions' as a duplicate dll.

using System.Text.RegularExpressions;
using CodeMechanic.Types;

namespace justdoit_fixer.Models;

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