using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoReservaciones.Data;
using ProyectoReservaciones.Models;

namespace ProyectoReservaciones.Controllers;

public class InscripcionController : Controller
{
    private readonly ApplicationDbContext _context;
    public InscripcionController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Metodo Get
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var inscripciones = await _context.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .ToListAsync();
        return View(inscripciones);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.EventoId = new SelectList(_context.Eventos, "EventoId", "Nombre");
        ViewBag.AsistenteId = new SelectList(_context.Asistentes, "AsistenteId", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            _context.Add(inscripcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.EventoId = new SelectList(_context.Eventos, "EventoId", "Nombre", inscripcion.EventoId);
        ViewBag.AsistenteId = new SelectList(_context.Asistentes, "AsistenteId", "Nombre", inscripcion.AsistenteId);
        return View(inscripcion);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion == null)
        {
            return NotFound();
        }
        ViewBag.EventoId = new SelectList(_context.Eventos, "EventoId", "Nombre", inscripcion.EventoId);
        ViewBag.AsistenteId = new SelectList(_context.Asistentes, "AsistenteId", "Nombre", inscripcion.AsistenteId);
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Inscripcion inscripcion)
    {
        if (id != inscripcion.InscripcionId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            _context.Update(inscripcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.EventoId = new SelectList(_context.Eventos, "EventoId", "Nombre", inscripcion.EventoId);
        ViewBag.AsistenteId = new SelectList(_context.Asistentes, "AsistenteId", "Nombre", inscripcion.AsistenteId);
        return View(inscripcion);
    }

    [HttpGet]
    public async Task<IActionResult> Ver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var inscripcion = await _context.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .FirstOrDefaultAsync(i => i.InscripcionId == id);
        if (inscripcion == null)
        {
            return NotFound();
        }
        return View(inscripcion);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var inscripcion = await _context.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .FirstOrDefaultAsync(i => i.InscripcionId == id);

        if (inscripcion == null)
        {
            return NotFound();
        }
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var inscripcion = await _context.Inscripciones.FindAsync(id);
        if (inscripcion != null)
        {
            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}
