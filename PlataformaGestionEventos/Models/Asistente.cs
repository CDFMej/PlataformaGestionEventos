using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PlataformaGestionEventos.Models;

public class Asistente
{
    [Key]
    public int AsistenteId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Ingrese el correo")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Por favor, ingrese un correo electrónico válido.")]
    public string Correo { get; set; }

    [Required(ErrorMessage = "Ingrese la identidad")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "La identidad solo debe contener números.")]
    [StringLength(13, MinimumLength = 13,ErrorMessage = "La identidad debe tener exactamente 13 caracteres.")]
    public string Identidad { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El teléfono solo debe contener números.")]
    [StringLength(12, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 12 dígitos.")]
    public string? Telefono { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [ForeignKey("UsuarioId")]
    public IdentityUser? User { get; set; }

    public ICollection<Inscripcion>? Inscripciones { get; set; }
}