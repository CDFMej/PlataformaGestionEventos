using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlataformaGestionEventos.ViewModels;
using PlataformaGestionEventos.Models;

namespace PlataformaGestionEventos.Models;

public class Evento
{
    [Key]
    public int EventoId { get; set; }
    [Required(ErrorMessage = "Ingrese el nombre del evento")]
    [StringLength(150)]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "Ingrese la descripción")]
    [StringLength(500)]
    public string Descripcion { get; set; }
    [Required(ErrorMessage = "Ingrese la fecha de inicio")]
    public DateTime FechaInicio { get; set; }
    [Required(ErrorMessage = "Ingrese la fecha de finalización")]
    public DateTime FechaFin { get; set; }
    [Required(ErrorMessage = "Ingrese la cantidad máxima de asistentes")]
    [Range(1, 50000)]
    public int CapacidadMaxima { get; set; }
    [Display(Name = "Sala")]
    public int SalaId { get; set; }
    [ForeignKey("SalaId")]
    public Sala? Sala { get; set; }
    public ICollection<Inscripcion>? Inscripciones { get; set; }
    public ICollection<RecursoEvento>? RecursosEvento { get; set; }
    [NotMapped]
    public List<EventoRecursoViewModel>? RecursosSeleccionados { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
     
        if (FechaInicio < DateTime.Now)
        {
            yield return new ValidationResult(
                "La fecha de inicio no puede ser anterior al día de hoy.",
                new[] { nameof(FechaInicio) });
        }


        if (FechaFin <= FechaInicio)
        {
            yield return new ValidationResult(
                "La fecha de fin debe ser posterior a la fecha de inicio.",
                new[] { nameof(FechaFin) });
        }
    }
}