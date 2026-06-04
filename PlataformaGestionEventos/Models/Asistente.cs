using System.ComponentModel.DataAnnotations;

namespace PlataformaGestionEventos.Models;

public class Asistente
{
    [Key]
    public int AsistenteId { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre")]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Ingrese la identidad")]
    [StringLength(20)]
    public string Identidad { get; set; }

    [Required(ErrorMessage = "Ingrese el correo")]
    [EmailAddress]
    public string Correo { get; set; }

    [Required(ErrorMessage = "Ingrese el teléfono")]
    [Phone]
    public string Telefono { get; set; }

    // Navegación
    public ICollection<Inscripcion>? Inscripciones { get; set; }
}