using justdoit_fixer.Models;

namespace justdoit_fixer.Pages.Todos;

public interface ITodosRepository
{
    Task<List<Todo>> GetAll();
}