namespace ProyectoReservaciones.Models;

using System.ComponentModel.DataAnnotations;


public class Recurso
{
    [Key]
    public int RecursoId { get; set; }
    [Required(ErrorMessage = "Ingrese el nombre del recurso")]
    [StringLength(100)]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "Ingrese el tipo de recurso")]
    [StringLength(100)]
    public string Tipo { get; set; }
    public bool Disponible { get; set; } = true;

    public int cantidad { get; set; }
    public ICollection<RecursoEvento>? RecursosEvento { get; set; }
}