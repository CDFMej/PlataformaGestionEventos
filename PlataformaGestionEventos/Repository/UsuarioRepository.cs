using Microsoft.AspNetCore.Identity;
using PlataformaGestionEventos.Data;

namespace PlataformaGestionEventos.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    public UsuarioRepository(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
        
    }

    public IEnumerable<IdentityUser> ObtenerUsuarios()
    {
        return _context.Users.ToList();
    }

    public IdentityUser ObtenerUsuario(string idUsuario)
    {
        return _context.Users.FirstOrDefault(x => x.Id == idUsuario);
    }

    public void BloquearUsuario(string idUsuario)
    {
        var usuario = _context.Users.FirstOrDefault(x => x.Id == idUsuario);
        if (usuario != null)
        {
            usuario.LockoutEnd = DateTime.Now.AddYears(100);
            _context.SaveChanges();
        }
    }

    public void DesbloquearUsuario(string idUsuario)
    {
        var usuario = _context.Users.FirstOrDefault(x => x.Id == idUsuario);
        if (usuario != null)
        {
            usuario.LockoutEnd = DateTime.Now;
            _context.SaveChanges();
        }
    }
    
    public async Task<IdentityResult> CrearUsuarioAsync(IdentityUser usuario, string password)
    {
        return await _userManager.CreateAsync(usuario, password);
    }
    
    public async Task<IdentityResult> ActualizarUsuarioAsync(IdentityUser usuario)
    {
        var usuarioExistente = await _userManager.FindByIdAsync(usuario.Id);
        if (usuarioExistente == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
        }
        usuarioExistente.Email = usuario.Email;
        usuarioExistente.UserName = usuario.Email; 
        usuarioExistente.PhoneNumber = usuario.PhoneNumber;

        return await _userManager.UpdateAsync(usuarioExistente);
    }
    
    public async Task<IdentityResult> EliminarUsuarioAsync(string idUsuario)
    {
        var usuario = await _userManager.FindByIdAsync(idUsuario);
        if (usuario == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
        }

        return await _userManager.DeleteAsync(usuario);
    }
}