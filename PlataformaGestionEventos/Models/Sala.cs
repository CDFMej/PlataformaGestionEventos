using System.ComponentModel.DataAnnotations;

namespace PlataformaGestionEventos.Models;

public class Sala
{
    [Key]
    public int SalaId { get; set; }
    [Required(ErrorMessage = "Ingrese un nombre"),StringLength(100,ErrorMessage = "Maximo 100 caracteres")]
    public string nombre { get; set; }
    [Required(ErrorMessage = "Ingrese la capacidad de la sala")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a 0")]
    public int capacidad { get; set; }
    [Required(ErrorMessage = "Ingrese la direccion de la sala")]
    public string ubicacion { get; set; }
}
