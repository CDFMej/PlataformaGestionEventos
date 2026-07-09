using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PlataformaGestionEventos.Utility; // Asegúrate de que esta referencia sea correcta

namespace PlataformaGestionEventos.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Crear Roles si no existen
        if (!await roleManager.RoleExistsAsync(CNT.Administrador))
        {
            await roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
            await roleManager.CreateAsync(new IdentityRole(CNT.Operador));
        }

        // 2. Crear Usuario Administrador si no existe
        var adminEmail = "Admin@admin.com";
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "@Dmin123456789");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, CNT.Administrador);
            }
        }
    }
}
