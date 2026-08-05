using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using Microsoft.AspNetCore.Authorization;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador, Asistente")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Asistente"))
        {
            return await DashboardAsistenteAsync();
        }

        ViewBag.TotalSalas = await _context.Salas.CountAsync(s => s.Activo);
        ViewBag.TotalEventos = await _context.Eventos.CountAsync(e => e.Activo);
        ViewBag.TotalAsistentes = await _context.Asistentes.CountAsync(a => a.Activo);
        ViewBag.TotalInscripciones = await _context.Inscripciones.CountAsync(i => i.Activo);
        ViewBag.TotalRecursos = await _context.Recursos.CountAsync(r => r.Activo);

        var ultimosEventos = await _context.Eventos
            .Where(e => e.Activo)
            .OrderByDescending(e => e.EventoId) 
            .Take(3)
            .ToListAsync();

        return View(ultimosEventos);
    }

    private async Task<IActionResult> DashboardAsistenteAsync()
    {
        var userId = _userManager.GetUserId(User);
        var asistente = await _context.Asistentes
            .FirstOrDefaultAsync(a => a.UsuarioId == userId);

        var misInscripciones = asistente == null
            ? new List<Inscripcion>()
            : await _context.Inscripciones
                .Where(i => i.AsistenteId == asistente.AsistenteId && i.Activo)
                .Include(i => i.Evento)
                    .ThenInclude(e => e.Sala)
                .ToListAsync();

        var ahora = DateTime.Now;

        ViewBag.NombreAsistente = asistente?.Nombre ?? User.Identity?.Name;
        ViewBag.TotalMisEventos = misInscripciones.Count;
        ViewBag.TotalProximos = misInscripciones
            .Count(i => i.Evento != null && i.Evento.FechaInicio > ahora);

        ViewBag.TotalDisponibles = await _context.Eventos
            .CountAsync(e => e.Activo && e.FechaInicio > ahora);

        var notificaciones = _context.Notificaciones.Where(n => n.UsuarioId == userId);
        ViewBag.TotalNotificaciones = await notificaciones.CountAsync();
        ViewBag.NotificacionesNoLeidas = await notificaciones.CountAsync(n => !n.Leida);

        var proximosEventos = misInscripciones
            .Where(i => i.Evento != null)
            .OrderBy(i => i.Evento!.FechaInicio)
            .Take(3)
            .ToList();

        return View("IndexAsistente", proximosEventos);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}