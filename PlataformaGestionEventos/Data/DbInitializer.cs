using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PlataformaGestionEventos.Utility;

namespace PlataformaGestionEventos.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync(CNT.Administrador))
        {
            await roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
            await roleManager.CreateAsync(new IdentityRole(CNT.Operador));
            await roleManager.CreateAsync(new IdentityRole(CNT.Asistente));
        }

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
