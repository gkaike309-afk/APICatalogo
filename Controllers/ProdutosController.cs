using System.Collections;
using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositores;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PagedList;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
<<<<<<< HEAD
[ApiConventionType(typeof(DefaultApiConventions))]
//[ApiExplorerSettings(IgnoreApi = true)]
=======
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private IMapper _mapper;

    public ProdutosController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos(int id)
    {
        var produto = _uof.ProdutoRepository?.GetProdutosPorCategoria(id);
        if (produto is null)
            return NotFound();

        var produtoDto = _mapper.Map<List<ProdutoDTO>>(produto);

        return Ok(produtoDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);
        return ObterProdutos(produtos);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco(
        [FromQuery] ProdutosFiltroPreco produtosFilterParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFilterParameters);

        return ObterProdutos(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(X.PagedList.IPagedList<Produto> produtos)
    {
        if (produtos == null) throw new ArgumentNullException(nameof(produtos));
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };
            
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return  Ok(produtosDto);
    }
    
<<<<<<< HEAD
    /// <summary>
    /// Exibe uma relação dos produtos
    /// </summary>
    /// <returns>retorna uma lista de objetos Produtos</returns>
    
    //[Authorize(Policy ="UserOnly")]
=======
    [Authorize]
    [Authorize(Policy = "UserOnly")]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        var produtos = await _uof.ProdutoRepository.GetAllAsync();
        if (produtos is null)
            return NotFound();

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }
    
<<<<<<< HEAD
    /// <summary>
    /// Obtem o produto pelo seu identificado produtoId
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>um obejto Produto</returns>
    
    [HttpGet("{id}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        if (id == null || id <= 0)
        {
            return BadRequest("Id de produto inválido");
        }
        
=======
    [HttpGet("{id}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
        var produto = _uof.ProdutoRepository?.GetAsync(c => c.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    [HttpPost]
<<<<<<< HEAD
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
=======
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO? produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);
        
        var novoProduto =_uof.ProdutoRepository?.Create(produto);
        await _uof.CommitAsync();
        
        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);
        
        return new CreatedAtRouteResult("ObterProduto",
            new { id = novoProduto?.ProdutoId }, novoProdutoDto);
    }

    [HttpPatch("{id:int}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest>? patchProdutoDto)
    {
        if(patchProdutoDto is null || id <= 0)
            return BadRequest();
        
        var produto = _uof.ProdutoRepository?.GetAsync(p => p.ProdutoId == id);
        
        if (produto is null)
            return NotFound();
        
        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);
        
        patchProdutoDto.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);
        
        _mapper.Map(produtoUpdateRequest, produto);
        
        _uof.ProdutoRepository?.Update(await produto);
        await _uof.CommitAsync();
        
        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }
<<<<<<< HEAD
    
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
=======
    [HttpPut("{id:int}")]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        
            return BadRequest();
        
        var produto = _mapper.Map<Produto>(produtoDto);
        
        var produtoAtualizado = _uof.ProdutoRepository?.Update(produto);
        await _uof.CommitAsync();
        
        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        return Ok(produtoAtualizadoDto);
    }

<<<<<<< HEAD
    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
=======
    [HttpDelete("{id:int}")]
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();
        
        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
        
        return Ok(produtoDeletadoDto);
    }
}