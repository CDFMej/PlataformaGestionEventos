using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlataformaGestionEventos.Repository;

namespace ProyectoReservaciones.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public IActionResult Index()
    {
        var usuarios = _usuarioRepository.ObtenerUsuarios();
        return View(usuarios);
    }

    public IActionResult Detalles(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        var usuario = _usuarioRepository.ObtenerUsuario(id);
        if (usuario == null)
        {
            return NotFound();
        }
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Bloquear(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        _usuarioRepository.BloquearUsuario(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Desbloquear(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        _usuarioRepository.DesbloquearUsuario(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(string email,string password,string phoneNumber)
    {
        var usuario = new IdentityUser
        {
            Email = email,
            UserName = email,
            PhoneNumber = phoneNumber
        };
        var resultado = await _usuarioRepository.CrearUsuarioAsync(usuario,password);
        if (resultado.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        foreach (var error in resultado.Errors)
        {
            ModelState.AddModelError("",error.Description);
        }
        return View();
    }

    public IActionResult Editar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        var usuario = _usuarioRepository.ObtenerUsuario(id);
        if (usuario == null)
        {
            return NotFound();
        }
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(IdentityUser usuario)
    {
        var resultado = await _usuarioRepository.ActualizarUsuarioAsync(usuario);
        if (resultado.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        foreach (var error in resultado.Errors)
        {
            ModelState.AddModelError("",error.Description);
        }
        return View(usuario);
    }

    public IActionResult Eliminar(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        var usuario = _usuarioRepository.ObtenerUsuario(id);
        if (usuario == null)
        {
            return NotFound();
        }
        return View(usuario);
    }

    [HttpPost,ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(string id)
    {
        var resultado = await _usuarioRepository.EliminarUsuarioAsync(id);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }
            var usuario = _usuarioRepository.ObtenerUsuario(id);
            return View(usuario);
        }
        return RedirectToAction(nameof(Index));
    }
}