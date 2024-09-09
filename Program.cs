using CodeMechanic.FileSystem;
using Hydro.Configuration;

// using justdoit_fixer.Pages.Todos;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
// builder.Services.AddSingleton<ITodosRepository, TodosRepository>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHydro();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHydro(builder.Environment);
app.MapRazorPages();

app.Run();