using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using System.Security.Claims;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador, Asistente")]
public class AsistenteController : Controller
{
    private readonly ApplicationDbContext _context;

    public AsistenteController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Metodo Get
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var asistentes = await _context.Asistentes
            .Where(a => a.Activo)
            .ToListAsync();
        return View(asistentes);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Asistente asistente)
    {
        if (ModelState.IsValid)
        {
            _context.Add(asistente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(asistente);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var asistente = await _context.Asistentes.FindAsync(id);
        if (asistente == null)
        {
            return NotFound();
        }
        return View(asistente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Asistente asistente)
    {
        if (id != asistente.AsistenteId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            _context.Update(asistente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(asistente);
    }

    [HttpGet]
    public async Task<IActionResult> Ver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var asistente = await _context.Asistentes
            .Include(a => a.Inscripciones)
                .ThenInclude(i => i.Evento)
            .FirstOrDefaultAsync(a => a.AsistenteId == id);
        if (asistente == null)
        {
            return NotFound();
        }
        return View(asistente);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var asistente = await _context.Asistentes.FindAsync(id);
        if (asistente == null)
        {
            return NotFound();
        }
        return View(asistente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var asistente = await _context.Asistentes.FindAsync(id);
        if (asistente != null)
        {
            asistente.Activo = false;
            _context.Asistentes.Update(asistente);

            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Asistente")]
    public async Task<IActionResult> MisNotificaciones()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notificaciones = await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .ToListAsync();

        foreach (var noti in notificaciones.Where(n => !n.Leida))
        {
            noti.Leida = true;
        }
        await _context.SaveChangesAsync();

        return View(notificaciones);
    }
}
