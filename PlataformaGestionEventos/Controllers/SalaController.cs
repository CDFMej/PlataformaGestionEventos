using Microsoft.AspNetCore.Mvc;
using PlataformaGestionEventos.Data;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Models;

namespace PlataformaGestionEventos.Controllers;

public class SalaController: Controller
{
    
    private readonly ApplicationDbContext _context;
    public SalaController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Metodo Get
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var salas = await _context.Salas.ToListAsync();
        return View(salas);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Sala sala)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sala);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sala);
    }
    
    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null)
        {
            return NotFound();
        }
        return View(sala);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id,Sala sala)
    {
        if (id != sala.SalaId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            _context.Update(sala);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sala);
    }
    
    [HttpGet]
    public async Task<IActionResult> Ver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null)
        {
            return NotFound();
        }
        return View(sala);
    }
    
    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var sala = await _context.Salas.FindAsync(id);
        if (sala == null)
        {
            return NotFound();
        }
        return View(sala);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var sala = await _context.Salas.FindAsync(id);
        if (sala != null)
        {
            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}