using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador")]
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
        var asistentes = await _context.Asistentes.ToListAsync();
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
            _context.Asistentes.Remove(asistente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}