using Inzynierka.Data.Tables;
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
        public DbSet<CruisesParticipants> CruisesParticipants { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Image> Messages { get; set; }

        public AhoyDbContext(DbContextOptions<AhoyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /* base.OnModelCreating(builder);
             builder.Entity<Users>().ToTable("Users");
             builder.Entity<Roles>().ToTable("Roles");
             builder.Entity<Charters>().ToTable("Charters");
             builder.Entity<CruiseJoinRequest>().ToTable("CruiseJoinRequest");
             builder.Entity<Cruises>().ToTable("Cruises");
             builder.Entity<Yachts>().ToTable("Yachts");
             //builder.Entity<YachtTypes>().ToTable("YachtTypes");
             builder.Entity<YachtSale>().ToTable("YachtSale");*/

            builder.Entity<Roles>(eb =>
            {
                eb.Property(r => r.name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasAnnotation("Index", "IX_RoleName"); // Dodanie indeksu na nazwę roli

                eb.Property(r => r.description)
                    .HasMaxLength(255)
                    .HasDefaultValue("No description available."); // Domyślna wartość dla opisu roli

                eb.Property(r => r.certificates)
                    .IsRequired(false);
            });


            builder.Entity<Users>(eb =>
            {
                eb.Property(u => u.username)
                  .IsRequired()
                  .HasMaxLength(255)
                  .IsUnicode(false); // Możemy ustawić brak Unicode dla poprawy wydajności

                eb.Property(u => u.password)
                    .IsRequired()
                    .HasMaxLength(255);

                eb.Property(u => u.email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false) // Ustawienie braku Unicode dla emaili
                    .HasAnnotation("Index", "IX_Email") // Dodanie indeksu na email
                    .HasAnnotation("RegularExpression", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // Walidacja wzorca dla emaila

                eb.Property(u => u.phone_number)
                    .HasMaxLength(20)
                    .HasAnnotation("RegularExpression", @"^\+?\d{1,15}$"); // Dodanie wzorca dla numeru telefonu

                eb.Property(u => u.first_name)
                    .HasMaxLength(100);

                eb.Property(u => u.last_name)
                    .HasMaxLength(100);

                eb.HasIndex(u => u.username).IsUnique(); // Ustawienie unikalności dla username
                eb.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Cruises>(eb =>
            {
                // Relacja z Yacht (wiele Cruises dla jednego Yacht)
                eb.HasOne(c => c.Yacht)
                    .WithMany(y => y.Cruises)
                    .HasForeignKey(c => c.YachtId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacja z Capitan (wiele Cruises dla jednego Capitan - użytkownik)
                eb.HasOne(c => c.Capitan)
                    .WithMany(u => u.Cruises)
                    .HasForeignKey(c => c.CapitanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CruiseJoinRequest>(eb =>
            {
                eb.Property(c => c.status).IsRequired().HasMaxLength(50);
                eb.Property(c => c.date).IsRequired();

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
                eb.Property(c => c.price).IsRequired();
                eb.Property(c => c.currency).HasMaxLength(3).IsRequired();
                eb.Property(c => c.status).HasMaxLength(50);

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
                eb.Property(ys => ys.saleDate).IsRequired();
                eb.Property(ys => ys.price).IsRequired();
                eb.Property(ys => ys.currency).IsRequired().HasMaxLength(3); // Zmieniono na string
                eb.Property(ys => ys.location).HasMaxLength(100);
                eb.Property(ys => ys.availabilityStatus).HasMaxLength(50);
            });


            builder.Entity<Yachts>(eb =>
            {
                // Właściwości podstawowe
                eb.Property(y => y.name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false); // Nazwa jachtu, brak Unicode dla wydajności

                eb.Property(y => y.description)
                    .HasMaxLength(1000); // Opis może być dłuższy, np. do 1000 znaków

                eb.Property(y => y.type)
                    .IsRequired()
                    .HasMaxLength(100); // Typ jachtu (np. żaglowy, motorowy)

                eb.Property(y => y.manufacturer)
                    .HasMaxLength(100); // Producent jachtu

                eb.Property(y => y.model)
                    .HasMaxLength(100); // Model jachtu

                eb.Property(y => y.year)
                    .IsRequired(); // Rok produkcji jachtu

                eb.Property(y => y.length)
                    .IsRequired(); // Długość jachtu

                eb.Property(y => y.width)
                    .IsRequired(); // Szerokość jachtu

                eb.Property(y => y.crew)
                    .IsRequired(); // Liczba członków załogi

                eb.Property(y => y.cabins)
                    .IsRequired(); // Liczba kabin

                eb.Property(y => y.beds)
                    .IsRequired(); // Liczba łóżek

                eb.Property(y => y.toilets)
                    .IsRequired(); // Liczba toalet

                eb.Property(y => y.showers)
                    .IsRequired(); // Liczba pryszniców

                eb.Property(y => y.location)
                    .HasMaxLength(100); // Lokalizacja jachtu

                eb.Property(y => y.availabilityStatus)
                    .HasMaxLength(50)
                    .HasDefaultValue("Available"); // Domyślny status dostępności

                // Zdjęcia jachtu
                eb.Property(y => y.image)
                    .IsRequired(false); // Przechowywanie zdjęć w formacie byte[]
            });

            builder.Entity< CruisesParticipants>(eb =>
            {
                eb.HasKey(cp => new { cp.UsersId, cp.CruisesId });
                eb.HasOne(cp => cp.Users)
                    .WithMany(u => u.CruisesParticipants)
                    .HasForeignKey(cp => cp.UsersId)
                    .OnDelete(DeleteBehavior.Cascade);

                eb.HasOne(cp => cp.Cruises)
                    .WithMany(c => c.CruisesParticipants)
                    .HasForeignKey(cp => cp.CruisesId)
                    .OnDelete(DeleteBehavior.Cascade);
            }); // poprawnie wykonana relacja many-to-many

        }
    }
}
