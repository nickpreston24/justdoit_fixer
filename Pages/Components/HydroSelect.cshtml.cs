using Hydro;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit_fixer.Pages.Components;

public class HydroSelect : HydroComponent
{
    public int Count { get; set; }

    public void Add()
    {
        Count++;
    }

    // private readonly IDatabase _database;
    //
    // public EditUserForm(IDatabase database)
    // {
    //     _database = database;
    //     
    //     Subscribe<SystemMessageEvent>(Handle);
    // }
    //
    // public string UserId { get; set; }
    //
    // [Required]
    // public string Name { get; set; }
    //
    // public override async Task MountAsync()
    // {
    //     var formData = ...; // fetch data from database
    //
    //     Name = formData.Name;
    // }
    //
    // public override void Render()
    // {
    //     ViewBag.IsLongName = Name.Length > 20;
    // }
    //
    // public async Task Save()
    // {
    //     await _database.UpdateUser(UserId, Name); // save the data
    // }
    //
    // public void Handle(SystemMessageEvent message)
    // {
    //     Message = message.Text;
    // }
}