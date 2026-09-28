using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Models;
using APICatalogo.Repositores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.AspNetCore.RateLimiting;
=======
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers;
<<<<<<< HEAD
[Route("[controller]")]
[ApiController]
[EnableRateLimiting("fixedwindow")]
[Produces("application/json")]
//[ApiExplorerSettings(IgnoreApi = true)]
=======

[Route("[controller]")]
[ApiController]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(
        ILogger<CategoriasController> logger, IUnitOfWork uof)
    {
        _logger = logger;
        _uof = uof;
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias =  await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
        var categorias = await _uof.CategoriaRepository.
            GetCategoriasFiltroNomeAsync(categoriasFiltro);
        
        return ObterCategorias(categorias);
    }
    
    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };
            
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDto = categorias.ToCategoriaDTOList();
        return  Ok(categoriasDto);
    }
<<<<<<< HEAD
     
    /// <summary>
    /// Obtem uma lista de objetos Categoria
    /// </summary>
    /// <returns>Uma lista de bbjetos Categoria</returns>
    
    [DisableRateLimiting]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string),  StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
=======
    
    [Authorize]
    [HttpGet]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        var categorias = await _uof.CategoriaRepository.GetAllAsync();
        if (categorias is null)
            return NotFound("Não exsitem categorias...");

        var categoriasDto = categorias.ToCategoriaDTOList();
        return Ok(categoriasDto);
    }

<<<<<<< HEAD
    /// <summary>
    /// Obtem uma categoria pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Uma lista de bbjetos Categoria</returns>

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
=======
    [HttpGet("{id:int}", Name = "ObterCategoria")]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria == null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada...");
            return NotFound($"Categoria com id= {id} não encontrada...");
        }

        var categoriaDto = categoria.ToCategoriaDTO();
        return Ok(categoriaDto);
    }
    
<<<<<<< HEAD
    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    ///
    ///     POST api/categorias
    ///     {
    ///         "categoriaId": 1,
    ///         "nome": categoria1,
    ///         "imagemUrl": "http://teste.net/1.jpg",
    ///     }
    /// </remarks>
    /// <param name="categoriaDto">objeto Categoria</param>
    /// <returns>O objeto Categoria incluida</returns>
    /// <remarks>Retorna um objeto Categoria incluído</remarks>
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
=======
    [HttpPost]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO? categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }
        
        var categoria = categoriaDto.ToCategoria();
        
        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();
        
        var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();
        
        return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
    }
    
    [HttpPut("{id:int}")]
<<<<<<< HEAD
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
=======
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO? categoriaDto)
    {
        if (id != categoriaDto?.CategoriaId)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }
        
        var categoria = categoriaDto.ToCategoria();
        
        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();
        
        var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();
        
        return Ok(categoriaAtualizadaDto);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
<<<<<<< HEAD
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string),  StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
=======
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
        
        if (categoria == null)
        {
            _logger.LogWarning($"Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();
        
        var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();
        
        return Ok(categoriaExcluidaDto);
    }
}