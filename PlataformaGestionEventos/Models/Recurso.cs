namespace PlataformaGestionEventos.Models;

using System.ComponentModel.DataAnnotations;


public class Recurso
{
    [Key]
    public int RecursoId { get; set; }
    [Required(ErrorMessage = "Ingrese el nombre del recurso")]
    [StringLength(100)]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "Ingrese el tipo de recurso")]
    [StringLength(100, ErrorMessage = "El tipo no puede exceder los 100 caracteres")]
    public string Tipo { get; set; }
    public bool Disponible { get; set; } = true;
    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
    public int cantidad { get; set; }
    public ICollection<RecursoEvento>? RecursosEvento { get; set; }
}