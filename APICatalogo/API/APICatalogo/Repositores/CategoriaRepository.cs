using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;

namespace APICatalogo.Repositores;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    { }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(
        CategoriasParameters categoriasParams)
    {
        var categoriasOrdenadas = _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.CategoriaId);

        var resultado = await categoriasOrdenadas.ToPagedListAsync(
            categoriasParams.PageNumber,
            categoriasParams.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(
        CategoriasFiltroNome categoriasParams)
    {
        var categorias = _context.Categorias
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(categoriasParams.Nome))
        {
            categorias = categorias.Where(c =>
                c.Nome.Contains(categoriasParams.Nome));
        }

        var categoriasFiltradas = await categorias.ToPagedListAsync(
            categoriasParams.PageNumber,
            categoriasParams.PageSize);

        return categoriasFiltradas;
    }
}