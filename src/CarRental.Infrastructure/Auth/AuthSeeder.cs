/// MIT License © 2025 Martín Duhalde + ChatGPT

using Microsoft.AspNetCore.Identity;

namespace CarRental.Infrastructure.Auth;

public static class RoleNames
{
    public const string Admin   /**/ = "Admin";
    public const string User    /**/ = "User";
}

public static class AuthSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        var roleManager /**/ = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager /**/ = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config      /**/ = serviceProvider.GetRequiredService<IConfiguration>();

        string[] roleNames = [RoleNames.Admin, RoleNames.User];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                    logger.LogInformation("✅ Rol '{Role}' creado correctamente.", roleName);
                else
                    logger.LogWarning("⚠️ Error creando rol '{Role}': {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }


        // 👤 Crear Usuario Admin desde configuración o por defecto
        var adminEmail      /**/ = config["SeedUsers:Admin:Email"] ?? "admin@demo.com";
        var adminPassword   /**/ = config["SeedUsers:Admin:Password"] ?? "Admin@123";
        var adminName       /**/ = config["SeedUsers:Admin:FullName"] ?? "Administrador";

        await CrearUsuario(logger, userManager, adminEmail, adminPassword, adminName, RoleNames.Admin);

        // 👤 Usuario estándar de prueba
        /// Swagger: usuario en la documentación para testing (facilitar pruebas desde la UI)
        /// 
        ///     "Email": "john.doe@example.com",
        ///     "Password": "P@ssw0rd123"
        var userEmail       /**/ = config["SeedUsers:User:Email"] ?? "john.doe@example.com";
        var userPassword    /**/ = config["SeedUsers:User:Password"] ?? "P@ssw0rd123";
        var userName        /**/ = config["SeedUsers:User:FullName"] ?? "John Doe";

        await CrearUsuario(logger, userManager, userEmail, userPassword, userName, RoleNames.User);
    }

    private static async Task CrearUsuario(ILogger logger, UserManager<ApplicationUser> userManager,
                                            string email,
                                            string password,
                                            string fullName,
                                            string roleName)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            logger.LogInformation("ℹ️ Usuario ya existente: {Email}", email);
            return;
        }

        var user = new ApplicationUser
        {
            UserName    /**/ = email,
            Email       /**/ = email,
            FullName    /**/ = fullName
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, roleName);
            logger.LogInformation("👤 Usuario por defecto creado ({Role}): {Email}", roleName, email);
        }
        else
        {
            logger.LogWarning("⚠️ Error creando usuario {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

}
