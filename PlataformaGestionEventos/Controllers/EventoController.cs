using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Data;
using PlataformaGestionEventos.Models;
using PlataformaGestionEventos.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace PlataformaGestionEventos.Controllers;

[Authorize(Roles = "Administrador, Operador")]
public class EventoController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventoController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Metodo Get
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var eventos = await _context.Eventos
            .Include(e => e.Sala)
            .ToListAsync();
        return View(eventos);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        ViewBag.SalaId = new SelectList(_context.Salas, "SalaId", "nombre");
        var recursos = await _context.Recursos.ToListAsync();
        Evento evento = new Evento
        {
            RecursosSeleccionados = recursos.Select(r => new EventoRecursoViewModel
            {
                RecursoId = r.RecursoId,
                Nombre = r.Nombre,
                CantidadDisponible = r.cantidad
            }).ToList()
        };

        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Evento evento)
    {
    if (ModelState.IsValid)
    {
        bool conflicto = await _context.Eventos.AnyAsync(e =>
            e.SalaId == evento.SalaId &&
            evento.FechaInicio < e.FechaFin &&
            evento.FechaFin > e.FechaInicio);
        if (conflicto)
        {
            ModelState.AddModelError("", "La sala ya está reservada en ese horario.");
        }
        else
        {
            foreach (var recurso in evento.RecursosSeleccionados.Where(r => r.Seleccionado))
            {
                var recursoDb = await _context.Recursos.FindAsync(recurso.RecursoId);
                if (recurso.CantidadSeleccionada > recursoDb.cantidad)
                {
                    ModelState.AddModelError("", $"No hay suficiente cantidad para {recursoDb.Nombre}");
                    break;
                }
            }
            if (ModelState.IsValid)
            {
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                foreach (var recurso in evento.RecursosSeleccionados.Where(r => r.Seleccionado))
                {
                    _context.RecursoEvento.Add(new RecursoEvento
                    {
                        EventoId = evento.EventoId,
                        RecursoId = recurso.RecursoId,
                        cantidad = recurso.CantidadSeleccionada
                    });
                    var recursoDb = await _context.Recursos.FindAsync(recurso.RecursoId);
                    recursoDb.cantidad -= recurso.CantidadSeleccionada;
                    if (recursoDb.cantidad <= 0)
                    {
                        recursoDb.Disponible = false;
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
    ViewBag.SalaId = new SelectList(_context.Salas, "SalaId", "nombre", evento.SalaId);
    var recursos = await _context.Recursos.ToListAsync();
    evento.RecursosSeleccionados ??= new List<EventoRecursoViewModel>();
    foreach (var r in recursos)
    {
        if (!evento.RecursosSeleccionados.Any(x => x.RecursoId == r.RecursoId))
        {
            evento.RecursosSeleccionados.Add(new EventoRecursoViewModel
            {
                RecursoId = r.RecursoId,
                Nombre = r.Nombre,
                CantidadDisponible = r.cantidad
            });
        }
    }
    return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var evento = await _context.Eventos
            .Include(e => e.RecursosEvento)
            .FirstOrDefaultAsync(e => e.EventoId == id);
        if (evento == null)
        {
            return NotFound();
        }
        var recursos = await _context.Recursos.ToListAsync();
        evento.RecursosSeleccionados = recursos.Select(r =>
        {
            var asignado = evento.RecursosEvento?
                .FirstOrDefault(re => re.RecursoId == r.RecursoId);

            return new EventoRecursoViewModel
            {
                RecursoId = r.RecursoId,
                Nombre = r.Nombre,
                CantidadDisponible = r.cantidad + (asignado?.cantidad ?? 0),
                Seleccionado = asignado != null,
                CantidadSeleccionada = asignado?.cantidad ?? 0
            };
        }).ToList();
        ViewBag.SalaId = new SelectList(_context.Salas, "SalaId", "nombre", evento.SalaId);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Evento evento)
    {
        if (id != evento.EventoId)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            bool conflicto = await _context.Eventos.AnyAsync(e =>
                e.EventoId != evento.EventoId &&
                e.SalaId == evento.SalaId &&
                evento.FechaInicio < e.FechaFin &&
                evento.FechaFin > e.FechaInicio);
            if (conflicto)
            {
                ModelState.AddModelError("", "La sala ya está reservada en ese horario.");
            }
            else
            {
                _context.Update(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        ViewBag.SalaId = new SelectList(_context.Salas, "SalaId", "nombre", evento.SalaId);
        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Ver(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var evento = await _context.Eventos
            .Include(e => e.Sala)
            .Include(e => e.RecursosEvento)
                .ThenInclude(re => re.Recurso)
            .FirstOrDefaultAsync(e => e.EventoId == id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var evento = await _context.Eventos
            .Include(e => e.Sala)
            .FirstOrDefaultAsync(e => e.EventoId == id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var evento = await _context.Eventos
            .Include(e => e.RecursosEvento)
            .FirstOrDefaultAsync(e => e.EventoId == id);
        if (evento != null)
        {
            foreach (var re in evento.RecursosEvento)
            {
                var recursoDb = await _context.Recursos
                    .FirstOrDefaultAsync(r => r.RecursoId == re.RecursoId);
                if (recursoDb != null)
                {
                    recursoDb.cantidad += re.cantidad;
                    if (recursoDb.cantidad > 0)
                    {
                        recursoDb.Disponible = true;
                    }
                }
            }
            _context.RecursoEvento.RemoveRange(evento.RecursosEvento);
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}