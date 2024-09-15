using System.Text.RegularExpressions;
using CodeMechanic.Types;

namespace justdoit.Models;

//
// public class TodoTimeRegex : RegexEnumBase
// {
//     // public static TodoTimeRegex Basic = new TodoTimeRegex(1, nameof(Basic),
//     //     @"(?<value>\d+)\s*?(?<unit>days?|months?|hours?minutes?|w|d|h|hrs?|mins?)", "https://regex101.com/r/7BbzEK/1");
//
// //
//     protected TodoTimeRegex(int id, string name, string pattern, string uri = "") : base(id, name, pattern, uri)
//     {
//     }
// }

public class TodoTime
{
    public int minutes { get; set; } = 0;
    public int hours { get; set; } = 0;

    public int days { get; set; } = 0;

    // public int weeks { get; set; } = 0;
    public int months { get; set; } = 0;
}

//
// public class RegexEnumBase : Enumeration
// {
//     protected RegexEnumBase(int id, string name, string pattern, string uri = "") : base(id, name)
//     {
//         Pattern = pattern;
//         CompiledRegex =
//             new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
//         this.uri = uri;
//     }
//
//     public string uri { get; set; } = string.Empty;
//
//     public Regex CompiledRegex { get; set; }
//     public string Pattern { get; set; }
// }