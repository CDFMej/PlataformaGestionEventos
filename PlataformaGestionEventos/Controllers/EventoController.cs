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
        var salas = await _context.Salas.ToListAsync();

        ViewBag.SalaId = new SelectList(
            salas.Select(s => new {
                Id = s.SalaId,
                Display = $"{s.nombre} (Capacidad: {s.capacidad} personas)"
            }),
            "Id",
            "Display"
        );
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
                var sala = await _context.Salas.FindAsync(evento.SalaId);

                if (sala != null && evento.CapacidadMaxima > sala.capacidad)
                {
                    ModelState.AddModelError("CapacidadMaxima",
                        $"La capacidad del evento ({evento.CapacidadMaxima}) no puede superar la capacidad de la sala ({sala.capacidad} personas).");
                }

                bool conflicto = await _context.Eventos.AnyAsync(e =>
                e.SalaId == evento.SalaId &&
                evento.FechaInicio < e.FechaFin &&
                evento.FechaFin > e.FechaInicio);
            if (conflicto)
            {
                ModelState.AddModelError("", "La sala ya está reservada en ese horario.");
            }

            if (evento.RecursosSeleccionados != null)
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
                }



            if (ModelState.IsValid)
            {
                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();
                if (evento.RecursosSeleccionados != null)
                {
                    foreach (var recurso in evento.RecursosSeleccionados.Where(r => r.Seleccionado))
                    {
                        _context.RecursoEvento.Add(new RecursoEvento
                        {
                            EventoId = evento.EventoId,
                            RecursoId = recurso.RecursoId,
                            cantidad = recurso.CantidadSeleccionada
                        });

                        var recursoDb = await _context.Recursos.FindAsync(recurso.RecursoId);
                        if (recursoDb != null)
                        {
                            recursoDb.cantidad -= recurso.CantidadSeleccionada;
                            if (recursoDb.cantidad <= 0)
                            {
                                recursoDb.Disponible = false;
                            }
                        }

                    }
                    await _context.SaveChangesAsync();
                    
                }
                return RedirectToAction(nameof(Index));
            }
            
    }

        var salas = await _context.Salas.ToListAsync();
        ViewBag.SalaId = new SelectList(
            salas.Select(s => new {
                Id = s.SalaId,
                Display = $"{s.nombre} (Capacidad: {s.capacidad} personas)"
            }), "Id", "Display", evento.SalaId);
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
        var salas = await _context.Salas.ToListAsync();
        ViewBag.SalaId = new SelectList(
            salas.Select(s => new {
                Id = s.SalaId,
                Display = $"{s.nombre} (Capacidad: {s.capacidad} personas)"
            }),
            "Id",
            "Display",
            evento.SalaId 
        );
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

    var sala = await _context.Salas.FindAsync(evento.SalaId);
    if (sala != null && evento.CapacidadMaxima > sala.capacidad)
    {
        ModelState.AddModelError("CapacidadMaxima", $"La capacidad excede el límite de la sala ({sala.capacidad}).");
    }

    bool conflicto = await _context.Eventos.AnyAsync(e =>
    e.EventoId != evento.EventoId &&
    e.SalaId == evento.SalaId &&
    evento.FechaInicio < e.FechaFin &&
    evento.FechaFin > e.FechaInicio);
    if (conflicto)
    {
        ModelState.AddModelError("", "La sala ya está reservada en ese horario.");
    }

    if(ModelState.IsValid)
    {
        var recursosViejos = await _context.RecursoEvento
            .Where(re => re.EventoId == evento.EventoId)
            .ToListAsync();
        foreach (var antiguo in recursosViejos)
        {
            var recursoDb = await _context.Recursos.FindAsync(antiguo.RecursoId);
            if (recursoDb != null)
            {
                recursoDb.cantidad += antiguo.cantidad;
                recursoDb.Disponible = true;
            }
        }
        _context.RecursoEvento.RemoveRange(recursosViejos);
        await _context.SaveChangesAsync();
        bool errorStock = false;
        foreach (var recurso in evento.RecursosSeleccionados.Where(r => r.Seleccionado))
        {
            var recursoDb = await _context.Recursos.FindAsync(recurso.RecursoId);
            if (recursoDb == null || recurso.CantidadSeleccionada > recursoDb.cantidad)
            {
                ModelState.AddModelError("", $"No hay suficiente cantidad para {recursoDb?.Nombre ?? "el recurso"}. Disponible real: {recursoDb?.cantidad}");
                errorStock = true;
                break;
            }
        }
        if (errorStock)
        {
            return await RecargarVistaEditar(evento);
        }
        _context.Update(evento);
        foreach (var recurso in evento.RecursosSeleccionados.Where(r => r.Seleccionado))
        {
            _context.RecursoEvento.Add(new RecursoEvento
            {
                EventoId = evento.EventoId,
                RecursoId = recurso.RecursoId,
                cantidad = recurso.CantidadSeleccionada
            });
            var recursoDb = await _context.Recursos.FindAsync(recurso.RecursoId);
            if (recursoDb != null)
            {
                recursoDb.cantidad -= recurso.CantidadSeleccionada;
                if (recursoDb.cantidad <= 0)
                {
                    recursoDb.Disponible = false;
                }
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    return await RecargarVistaEditar(evento);
}
private async Task<IActionResult> RecargarVistaEditar(Evento evento)
{
        var salas = await _context.Salas.ToListAsync();
        ViewBag.SalaId = new SelectList(
            salas.Select(s => new {
                Id = s.SalaId,
                Display = $"{s.nombre} (Capacidad: {s.capacidad} personas)"
            }), "Id", "Display", evento.SalaId);
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
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DevolverRecursos(int id)
    {
        var recursosEvento = await _context.RecursoEvento
            .Where(re => re.EventoId == id)
            .ToListAsync();
        if (recursosEvento.Any())
        {
            foreach (var re in recursosEvento)
            {
                var recursoDb = await _context.Recursos.FindAsync(re.RecursoId);
                if (recursoDb != null)
                {
                    recursoDb.cantidad += re.cantidad;
                    recursoDb.Disponible = true;
                }
            }
            _context.RecursoEvento.RemoveRange(recursosEvento);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}