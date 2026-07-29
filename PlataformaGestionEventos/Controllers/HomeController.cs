using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using Microsoft.AspNetCore.Authorization;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
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

    public IActionResult Privacy()
    {
        return View();
    }
}