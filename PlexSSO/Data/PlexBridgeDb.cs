using Microsoft.EntityFrameworkCore;
using PlexSSO.Entities;

namespace PlexSSO.Data
{
    /// <summary>
    /// Database context for PlexBridge, managing entities such as KeyMaterial, BridgeSession, IssuedCode, and AuthTxn.
    /// </summary>
    public class PlexBridgeDb(DbContextOptions<PlexBridgeDb> options) : DbContext(options)
    {
        /// <summary>
        /// Gets the DbSet for KeyMaterial entities.
        /// </summary>
        public DbSet<KeyMaterial> Keys => Set<KeyMaterial>();

        /// <summary>
        /// Gets the DbSet for BridgeSession entities.
        /// </summary>
        public DbSet<BridgeSession> Sessions => Set<BridgeSession>();

        /// <summary>
        /// Gets the DbSet for IssuedCode entities.
        /// </summary>
        public DbSet<IssuedCode> Codes => Set<IssuedCode>();

        /// <summary>
        /// Gets the DbSet for AuthTxn entities.
        /// </summary>
        public DbSet<AuthTxn> AuthTxns => Set<AuthTxn>();

        /// <summary>
        /// Gets the DbSet for AdminUser entities.
        /// </summary>
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        /// <summary>
        /// Gets the DbSet for OidcClientApp entities.
        /// </summary>
        public DbSet<OidcClientApp> Clients => Set<OidcClientApp>();

        /// <summary>
        /// Configures the model by setting up keys and indexes for the entities.
        /// </summary>
        /// <param name="b"></param>
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<KeyMaterial>().HasKey(k => k.Id);
            b.Entity<KeyMaterial>().HasIndex(k => k.Kid).IsUnique();

            b.Entity<BridgeSession>().HasKey(s => s.Id);
            b.Entity<BridgeSession>().HasIndex(s => s.CreatedAt);

            b.Entity<IssuedCode>().HasKey(c => c.Code);
            b.Entity<IssuedCode>().HasIndex(c => c.ExpiresAt);

            b.Entity<AuthTxn>().HasKey(t => t.Id);
            b.Entity<AuthTxn>().HasIndex(t => t.CreatedAt);

            b.Entity<AdminUser>().HasKey(x => x.Id);
            b.Entity<AdminUser>().HasIndex(x => x.Username).IsUnique();

            b.Entity<OidcClientApp>().HasKey(x => x.Id);
            b.Entity<OidcClientApp>().HasIndex(x => x.ClientId).IsUnique();
        }
    }
}



