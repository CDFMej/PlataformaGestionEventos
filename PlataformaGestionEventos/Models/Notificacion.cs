using System.ComponentModel.DataAnnotations;

namespace PlataformaGestionEventos.Models
{
   
    public class Notificacion
    {
        public int NotificacionId { get; set; }
        public string UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Leida { get; set; } = false;
    }
}
