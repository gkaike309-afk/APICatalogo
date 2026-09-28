using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.DTOs;

[Table("Produtos")] 
 
public class ProdutoDTO
{
    public int ProdutoId { get; set; }
    
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    
    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }
    
    [Required]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    
    public int CategoriaId { get; set; }
<<<<<<< HEAD
=======

>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
}