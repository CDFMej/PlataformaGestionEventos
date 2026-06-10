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
        ViewBag.TotalSalas = await _context.Salas.CountAsync();
        ViewBag.TotalEventos = await _context.Eventos.CountAsync();
        ViewBag.TotalAsistentes = await _context.Asistentes.CountAsync();
        ViewBag.TotalInscripciones = await _context.Inscripciones.CountAsync();
        ViewBag.TotalRecursos = await _context.Recursos.CountAsync();

        var ultimosEventos = await _context.Eventos
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