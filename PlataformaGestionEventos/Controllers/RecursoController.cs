using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using Microsoft.AspNetCore.Authorization;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador")]
public class RecursoController : Controller
{
    private readonly ApplicationDbContext _context;

    public RecursoController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Metodo Get
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var recursos = await _context.Recursos.ToListAsync();
        return View(recursos);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Recurso recurso)
    {
        if (ModelState.IsValid)
        {
            _context.Add(recurso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(recurso);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso == null)
        {
            return NotFound();
        }
        return View(recurso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Recurso recurso)
    {
        if (id != recurso.RecursoId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            _context.Update(recurso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(recurso);
    }

    [HttpGet]
    public async Task<IActionResult> Ver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso == null)
        {
            return NotFound();
        }
        return View(recurso);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso == null)
        {
            return NotFound();
        }
        return View(recurso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var recurso = await _context.Recursos.FindAsync(id);
        if (recurso != null)
        {
            _context.Recursos.Remove(recurso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}