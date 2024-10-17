namespace Comanda.WebApi.Extensions;

/*
   BootstrapExtension serves the purpose of initializing essential configurations
   during the application startup. Specifically, it ensures the existence of a
   administrator user role and creates a admin user if one doesn't exist.
   This is crucial for setting up initial access control and administrative privileges
   before any other operations can proceed. Similar to Django's create admin feature,
   this extension ensures that the application starts with necessary administrative
   capabilities securely in place.
*/

[ExcludeFromCodeCoverage]
internal static class BootstrapExtension
{
    public static async Task Bootstrap(this IApplicationBuilder builder)
    {
        var serviceProvider = builder.ApplicationServices;

        using (var scope = serviceProvider.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AdminSettings>>();

            await EnsureAdministratorRoleAsync(roleManager);
            await EnsureAdminUserAsync(userManager, logger, builder.ApplicationServices);
        }
    }

    private static async Task EnsureAdminUserAsync(UserManager<Account> userManager, ILogger logger, IServiceProvider serviceProvider)
    {
        var adminSettings = serviceProvider.GetRequiredService<IConfiguration>()
            .GetSection("AdminSettings")
            .Get<AdminSettings>();

        if (adminSettings == null)
        {
            logger.LogError("Admin settings are missing from configuration.");
            return;
        }

        var usersInRole = await userManager.GetUsersInRoleAsync("Administrator");

        if (usersInRole.Count > 0)
        {
            logger.LogInformation("Administrator user already exists.");
            return;
        }

        logger.LogWarning("No administrator user found. Creating one.");

        var adminUser = new Account
        {
            UserName = adminSettings.UserName,
            Email = adminSettings.Email,
        };

        var result = await userManager.CreateAsync(adminUser, adminSettings.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            logger.LogInformation("Administrator user ({username} - {email}) successfully created.", adminSettings.UserName, adminSettings.Email);
        }
        else
        {
            logger.LogError("Error creating admin user:");

            foreach (var error in result.Errors)
            {
                logger.LogError($"> {error.Description}");
            }
        }
    }

    private static async Task EnsureAdministratorRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        var roleExists = await roleManager.RoleExistsAsync("Administrator");
        if (!roleExists)
        {
            var adminRole = new IdentityRole("Administrator");
            await roleManager.CreateAsync(adminRole);
        }
    }

    public sealed record AdminSettings
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}