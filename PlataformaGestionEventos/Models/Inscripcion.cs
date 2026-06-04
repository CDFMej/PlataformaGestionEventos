using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoReservaciones.Models;

public class Inscripcion
{
    [Key]
    public int InscripcionId { get; set; }

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required]
    public string Estado { get; set; } = "Confirmado";

 
    public int EventoId { get; set; }

    [ForeignKey("EventoId")]
    public Evento? Evento { get; set; }


    public int AsistenteId { get; set; }

    [ForeignKey("AsistenteId")]
    public Asistente? Asistente { get; set; }
}
