using Microsoft.AspNetCore.Identity;

namespace PlataformaGestionEventos.Repository;

public interface IUsuarioRepository 
{
    public IEnumerable<IdentityUser> ObtenerUsuarios();
    public IdentityUser ObtenerUsuario(string idUsuario);
    void BloquearUsuario(string usuario);
    void DesbloquearUsuario(string usuario);
    Task<IdentityResult> CrearUsuarioAsync(IdentityUser usuario, string password);
    Task<IdentityResult> ActualizarUsuarioAsync(IdentityUser usuario);
    Task<IdentityResult> EliminarUsuarioAsync(string idUsuario);
}