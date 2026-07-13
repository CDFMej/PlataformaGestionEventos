using System.ComponentModel.DataAnnotations;

namespace PlataformaGestionEventos.Models;

public class Asistente
{
    [Key]
    public int AsistenteId { get; set; }

    [Required(ErrorMessage = "Ingrese el nombre")]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Ingrese el correo")]
    [EmailAddress]
    public string Correo { get; set; }

    [Required(ErrorMessage = "Ingrese la identidad")]
    [StringLength(13, ErrorMessage = "La identidad no puede exceder los 13 caracteres")]
    public string Identidad { get; set; }

    [StringLength(12, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 12 dígitos.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo se permiten números.")]
    [Display(Name = "Teléfono")]
    [Required(ErrorMessage = "Ingrese el teléfono")]
    [Phone]
    public string Telefono { get; set; }

    // Navegación
    public ICollection<Inscripcion>? Inscripciones { get; set; }
}