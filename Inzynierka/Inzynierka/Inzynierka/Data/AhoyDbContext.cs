using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inzynierka.Data
{
    /// <summary>
    /// AhoyDbContext
    /// ApplicationDbContext
    /// </summary>
    public class AhoyDbContext : IdentityDbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Charters> Charters { get; set; }
        public DbSet<CruiseJoinRequest> CruiseJoinRequest { get; set; }
        public DbSet<Cruises> Cruises { get; set; }
        public DbSet<Yachts> Yachts { get; set; }
        //public DbSet<YachtTypes> YachtTypes { get; set; }
        public DbSet<YachtSale> YachtSale { get; set; }

        public AhoyDbContext(DbContextOptions<AhoyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*base.OnModelCreating(builder);
            builder.Entity<Users>().ToTable("Users");
            builder.Entity<Roles>().ToTable("Roles");
            builder.Entity<Charters>().ToTable("Charters");
            builder.Entity<CruiseJoinRequest>().ToTable("CruiseJoinRequest");
            builder.Entity<Cruises>().ToTable("Cruises");
            builder.Entity<Yachts>().ToTable("Yachts");
            //builder.Entity<YachtTypes>().ToTable("YachtTypes");
            builder.Entity<YachtSale>().ToTable("YachtSale");*/

            builder.Entity<Users>(eb =>
            {
                eb.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Cruises>(eb =>
            {
                eb.HasOne(c => c.Yacht)
                    .WithMany(y => y.Cruises)
                    .HasForeignKey(c => c.YachtId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Cruises>(eb =>
            {
                eb.HasOne(c => c.Capitan)
                    .WithMany(u => u.Cruises)
                    .HasForeignKey(c => c.CapitanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Cruises>(eb =>
            {
                eb.HasMany(c => c.Users)
                    .WithMany(u => u.Cruises);
            });

            builder.Entity<CruiseJoinRequest>(eb =>
            {
                // Definiowanie relacji wiele-do-jednego z Cruise
                eb.HasOne(c => c.Cruise)
                  .WithMany(c => c.CruiseJoinRequests)
                  .HasForeignKey(c => c.CruiseId);

                // Definiowanie relacji jeden-do-jednego z User (osoba składająca prośbę)
                eb.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId);

                // Definiowanie relacji jeden-do-jednego z Capitan
                eb.HasOne(c => c.Capitan)
                  .WithMany()
                  .HasForeignKey(c => c.CapitanId);
            });

            builder.Entity<Charters>(eb =>
            {
                // Relacja do Yachts (wiele Charters dla jednego Yacht)
                eb.HasOne(c => c.Yacht)
                    .WithMany(y => y.Charters)
                    .HasForeignKey(c => c.YachtId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacja do Users (wiele Charters dla jednego User)
                eb.HasOne(c => c.User)
                    .WithMany(u => u.Charters)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Możesz zmienić DeleteBehavior według potrzeb

                eb.HasOne(y => y.Owner)
                .WithMany(u => u.Charters)
                .HasForeignKey(y => y.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

                // Opcjonalnie: inne właściwości, np. walidacje
                eb.Property(c => c.price).IsRequired();
                eb.Property(c => c.currency).HasMaxLength(3); // Możesz użyć string zamiast char dla waluty
                eb.Property(c => c.status).HasMaxLength(50);

            });


            builder.Entity<YachtSale>(eb =>
            {
                eb.HasKey(ys => ys.Id);

                // Relacja jeden do wielu z tabelą Yachts
                eb.HasOne(ys => ys.Yacht)
                  .WithMany(y => y.YachtSale)
                  .HasForeignKey(ys => ys.YachtId)
                  .OnDelete(DeleteBehavior.Cascade); // Usunięcie Yacht usuwa powiązane YachtSales

                // Relacja jeden do wielu z tabelą Users (kupujący)
                eb.HasOne(ys => ys.BuyerUser)
                  .WithMany(u => u.YachtSale)
                  .HasForeignKey(ys => ys.BuyerUserId)
                  .OnDelete(DeleteBehavior.Restrict); // Usunięcie kupującego nie usuwa YachtSale

                // Relacja jeden do wielu z tabelą Users (właściciel)
                eb.HasOne(ys => ys.Owner)
                  .WithMany(u => u.YachtSale)
                  .HasForeignKey(ys => ys.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict); // Usunięcie właściciela nie usuwa YachtSale

                //Sold Yachts można dać tabele jechty sprzedane

                // Dodatkowe konfiguracje
                eb.Property(ys => ys.date).IsRequired();
                eb.Property(ys => ys.price).IsRequired();
                eb.Property(ys => ys.currency).IsRequired().HasMaxLength(3); // Zmieniono na string
                eb.Property(ys => ys.location).HasMaxLength(100);
                eb.Property(ys => ys.availabilityStatus).HasMaxLength(50);
            });

        }
    }
}
