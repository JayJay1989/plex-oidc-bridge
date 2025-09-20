using Microsoft.EntityFrameworkCore;
using PlexSSO.Clients;
using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Interfaces.Clients;
using PlexSSO.Interfaces.Services;
using PlexSSO.Models;
using PlexSSO.Services;

namespace PlexSSO
{
    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();

            services.AddDbContext<PlexBridgeDb>(opt =>
                opt.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection")
                    ?? "Data Source=/app/db/plex-oidc-bridge.db"));


            services.AddScoped<IKeyStore, EfKeyStore>();
            services.AddScoped<ICodeStore, EfCodeStore>();
            services.AddScoped<ISessionStore, EfSessionStore>();
            services.AddScoped<IPlexClient, PlexClient>();

            services.AddSession(o =>
            {
                o.Cookie.Name = "plex-oidc-admin";
                o.IdleTimeout = TimeSpan.FromHours(8);
                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddSingleton(provider =>
            {
                var cfg = new BridgeConfig
                {
                    Issuer = configuration["SSO:Issuer"] ?? "https://localhost:7190",
                    PlexClientIdentifier = configuration["Plex:ClientIdentifier"] ?? "mqe9h8q8rpcltfrcdu9k1n9x",
                    PlexProduct = configuration["Plex:Product"] ?? "Plex-OIDC-Bridge",
                    PlexDevice = configuration["Plex:Device"] ?? "Server",
                    PlexPlatform = configuration["Plex:Platform"] ?? "Linux",
                    PlexVersion = configuration["Plex:Version"] ?? "1.0.0",
                    CodeLifetimeSeconds = 180,
                    IdTokenLifetimeSeconds = 900,
                    AccessTokenLifetimeSeconds = 900
                };
                return cfg;
            });

            services.AddSingleton<ITokenSigningService, TokenSigningService>();

            services.AddHttpClient("PlexAPI", client =>
                {
                    client.BaseAddress = new Uri("https://plex.tv/api/v2/");
                });
        }
        static bool IsAdmin(HttpContext ctx) => ctx.Session.GetInt32("admin-id") != null;

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            InitializeAdminUser(app);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.MapRazorPages();
            app.MapControllers();
        }

        /// <summary>
        /// Applies any pending EF Core database migrations.
        /// </summary>
        /// <param name="app"></param>
        private void InitializeDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PlexBridgeDb>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
            var pending = db.Database.GetPendingMigrations().ToArray();

            if (!pending.Any())
            {
                return;
            }

            logger.LogInformation("Applying {count} migration(s): {migs}", pending.Length, string.Join(',', pending));
            db.Database.Migrate();
        }

        /// <summary>
        /// Ensures that at least one admin user exists, creating one from configuration if necessary.
        /// </summary>
        /// <param name="app"></param>
        private void InitializeAdminUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PlexBridgeDb>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();

            var adminUser = db.AdminUsers.FirstOrDefault();
            var userFromCfg = app.Configuration["Admin:Username"] ?? "admin";
            var pwdFromCfg = app.Configuration["Admin:Password"] ?? "admin";

            if (adminUser == null)
            {
                SimplePasswordHasher.Hash(pwdFromCfg, out var hash, out var salt);
                db.AdminUsers.Add(new AdminUser
                {
                    Username = userFromCfg,
                    PasswordHash = hash,
                    PasswordSalt = salt
                });
                db.SaveChanges();
                logger.LogInformation("Seeded admin user '{User}'.", userFromCfg);
            }

            if (pwdFromCfg == "admin")
            {
                logger.LogWarning("ADMIN PASSWORD is the default 'admin'! Change appsettings:Admin:Password immediately.");
            }
        }
    }
}
