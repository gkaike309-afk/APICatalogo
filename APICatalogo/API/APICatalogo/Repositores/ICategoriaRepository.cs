using APICatalogo.Controllers;
using APICatalogo.Models;
using X.PagedList;

namespace APICatalogo.Repositores;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<IPagedList<Categoria>> GetCategoriasAsync(
        CategoriasParameters categoriasParams);

    Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(
        CategoriasFiltroNome categoriasParams);
    
}