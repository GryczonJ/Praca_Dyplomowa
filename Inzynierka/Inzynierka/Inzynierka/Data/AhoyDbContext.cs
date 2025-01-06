using Inzynierka.Data.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Inzynierka.Data
{
    /// <summary>
    /// AhoyDbContext
    /// ApplicationDbContext
    /// </summary>
    public class AhoyDbContext : IdentityDbContext<Users, Roles, Guid>
    {
        public DbSet<Charters> Charters { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<CruiseJoinRequest> CruiseJoinRequest { get; set; }
        public DbSet<Cruises> Cruises { get; set; }
        public DbSet<CruisesParticipants> CruisesParticipants { get; set; }
        public DbSet<FavoriteCruises> FavoriteCruises { get; set; }
        public DbSet<FavoriteYachtsForSale> FavoriteYachtsForSale { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<Reservation> Resservation { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Yachts> Yachts { get; set; }
        public DbSet<YachtSale> YachtSale { get; set; }

        public AhoyDbContext(DbContextOptions<AhoyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Charters>(eb =>
            {
                eb.Property(c => c.price).IsRequired();
                eb.Property(c => c.currency).HasMaxLength(3).IsRequired();
                eb.Property(c => c.status).HasMaxLength(50);
                eb.Property(c => c.location).HasMaxLength(255); // Ustal maksymalną długość dla pola location

                // Relacja wiele-do-jednego z tabelą Yachts
                eb.HasOne(c => c.Yacht)
                    .WithMany(y => y.Charters)
                    .HasForeignKey(c => c.YachtId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacja wiele-do-jednego z tabelą Users (z OwnerId)
                eb.HasOne(c => c.Owner)
                    .WithMany(u => u.Charters)
                    .HasForeignKey(c => c.OwnerId)
                    .OnDelete(DeleteBehavior.NoAction);


                eb.HasMany(c => c.Comments)
                    .WithOne(r => r.Charter)
                    .HasForeignKey(r => r.CharterId)
                    .OnDelete(DeleteBehavior.NoAction);

                eb.HasMany(c => c.Reports)
                    .WithOne(r => r.SuspectCharter)
                    .HasForeignKey(r => r.SuspectCharterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Comments>(eb =>
            {
                eb.Property(c => c.Message)
                   .IsRequired()
                   .HasMaxLength(1000); // Możesz dostosować maksymalną długość według potrzeb

                eb.Property(c => c.CreateDate)
                    .IsRequired();

                eb.Property(c => c.Rating)
                    .IsRequired()
                    .HasDefaultValue(0); // Opcjonalnie, ustaw wartość domyślną dla Rating  

                // Relacja wiele-do-jednego z tabelą Users (Creator)
                eb.HasOne(c => c.Creator) // Komentarz ma jednego twórcę
                    .WithMany(u => u.CommentsAsCreator) // Użytkownik może mieć wiele komentarzy jako twórca
                    .HasForeignKey(c => c.CreatorId) // Klucz obcy do twórcy
                    .OnDelete(DeleteBehavior.NoAction); // Usunięcie użytkownika nie usuwa komentarzyy

                // Relacja wiele-do-jednego z tabelą Users ()
                eb.HasOne(c => c.Profile) // Komentarz należy do jednego profilu
                    .WithMany(u => u.CommentsAsProfile) // Użytkownik może mieć wiele komentarzy jako profil
                    .HasForeignKey(c => c.ProfileId) // Klucz obcy do profilu
                    .OnDelete(DeleteBehavior.NoAction); // Usunięcie użytkownika nie usuwa komentarzy
            });

            builder.Entity<CruiseJoinRequest>(eb =>
            {
                eb.Property(c => c.status).IsRequired().HasMaxLength(50);
                eb.Property(c => c.date).IsRequired();



                // Definiowanie relacji wiele-do-jednego z Cruise
                eb.HasOne(c => c.Cruise)
                  .WithMany(c => c.CruiseJoinRequests)
                  .HasForeignKey(c => c.CruiseId)
                  .OnDelete(DeleteBehavior.NoAction); // Brak kaskadowego usuwania

                // Relacja jeden-do-wielu z Users - osoba składająca prośbę
                eb.HasOne(c => c.User)
                  .WithMany(u=>u.CruiseJoinRequests)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.NoAction); // Brak kaskadowego usuwania

                // Relacja jeden-do-wielu z Users - kapitan
                eb.HasOne(c => c.Capitan)
                  .WithMany(u => u.CruiseJoinRequestsAsCapitan)
                  .HasForeignKey(c => c.CapitanId)
                  .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Cruises>(eb =>
            {
                eb.Property(c => c.name)
                    .IsRequired()
                    .HasMaxLength(200); // Maksymalna długość dla nazwy rejsu, dostosuj wg potrzeb

                eb.Property(c => c.description)
                    .HasMaxLength(1000); // Dostosuj maksymalną długość opisu

                eb.Property(c => c.destination)
                    .HasMaxLength(255); // Maksymalna długość dla celu rejsu

                eb.Property(c => c.start_date)
                    .IsRequired();

                eb.Property(c => c.end_date)
                    .IsRequired();

                eb.Property(c => c.price)
                    .HasColumnType("decimal(18, 2)") // Ustalenie typu kolumny dla ceny
                    .IsRequired();

                eb.Property(c => c.currency)
                    .HasMaxLength(3) // Zwykle waluty mają 3 znaki
                    .IsRequired();

                eb.Property(c => c.status)
                    .HasMaxLength(50); // Możesz dostosować długość statusu

                eb.Property(c => c.maxParticipants)
                    .IsRequired(); // Określenie, że liczba uczestników jest obowiązkowa



                // Relacja z Yacht (wiele Cruises dla jednego Yacht)
                eb.HasOne(c => c.Yacht)
                    .WithMany(y => y.Cruises)
                    .HasForeignKey(c => c.YachtId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacja z Capitan (wiele Cruises dla jednego Capitan - użytkownik)
                eb.HasOne(c => c.Capitan)
                    .WithMany(u => u.Cruises)
                    .HasForeignKey(c => c.CapitanId)
                    .OnDelete(DeleteBehavior.NoAction);

                eb.HasMany(c => c.Reports)
                    .WithOne(r => r.SuspectCruise)
                    .HasForeignKey(r => r.SuspectCruiseId)
                    .OnDelete(DeleteBehavior.Cascade); // Jeśli chcesz, aby usunięcie rejsu powodowało usunięcie raportów

                eb.HasMany(c => c.Comments)
                  .WithOne(cm => cm.Cruises)
                  .HasForeignKey(cm => cm.CruisesId)
                  .OnDelete(DeleteBehavior.NoAction); // Jeśli chcesz, aby usunięcie rejsu powodowało usunięcie komentarzy
            });

            builder.Entity<CruisesParticipants>(eb =>
            {
                eb.HasKey(cp => new { cp.UsersId, cp.CruisesId });



                // Definicja relacji z tabelą Users
                eb.HasOne(cp => cp.Users)
                    .WithMany(u => u.CruisesParticipants)
                    .HasForeignKey(cp => cp.UsersId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Definicja relacji z tabelą Cruises
                eb.HasOne(cp => cp.Cruises)
                    .WithMany(c => c.CruisesParticipants)
                    .HasForeignKey(cp => cp.CruisesId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<FavoriteCruises>(eb =>
            {
                // Definicja klucza głównego jako kombinacja UserId i CruiseId
                eb.HasKey(fc => new { fc.UserId, fc.CruiseId });


                // Relacja z tabelą Users
                eb.HasOne(fc => fc.User)
                    .WithMany(u => u.FavoriteCruises)
                    .HasForeignKey(fc => fc.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacja z tabelą Cruises
                eb.HasOne(fc => fc.Cruise)
                    .WithMany(c => c.FavoriteCruises)
                    .HasForeignKey(fc => fc.CruiseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<FavoriteYachtsForSale>(eb =>
            {
                // Definicja klucza głównego jako kombinacja UserId i YachtSaleId
                eb.HasKey(fy => new { fy.UserId, fy.YachtSaleId });


                // Relacja z tabelą Users
                eb.HasOne(fy => fy.User)
                    .WithMany(u => u.FavoriteYachtsForSale)
                    .HasForeignKey(fy => fy.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacja z tabelą YachtSale
                eb.HasOne(fy => fy.YachtForSale)
                    .WithMany(y => y.FavoriteYachtsForSale)
                    .HasForeignKey(fy => fy.YachtSaleId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Image>(eb =>
            {
                 eb.Property(i => i.link)
                    .IsRequired()  // Zakładając, że link będzie wymagany
                    .HasMaxLength(500);  // Określenie maksymalnej długości linku, np. 500 znaków
                
                
                // Konfiguracja relacji z tabelą Users
                eb.HasMany(i => i.Users)
                    .WithOne(u => u.Photos)
                    .HasForeignKey(u => u.PhotosId)
                    .OnDelete(DeleteBehavior.NoAction);  // Usuwanie kaskadowe - jeśli obraz zostanie usunięty, powiązani użytkownicy będą zaktualizowani

                // Konfiguracja relacji z tabelą Yachts
                eb.HasMany(i => i.Yachts)
                    .WithOne(y => y.Image)
                    .HasForeignKey(y => y.ImageId)
                    .OnDelete(DeleteBehavior.NoAction);  // Usuwanie kaskadowe
            });

            builder.Entity<Notifications>(eb =>
            {
                // Ustawienie właściwości status jako wymaganej i maksymalnej długości 50 znaków
                eb.Property(n => n.status)
                    .IsRequired()
                    .HasMaxLength(50);

                // Ustawienie właściwości message jako wymaganej i maksymalnej długości 500 znaków
                eb.Property(n => n.message)
                    .IsRequired()
                    .HasMaxLength(500);

                // Ustawienie właściwości CreateDate jako wymaganej
                eb.Property(n => n.CreateDate)
                    .IsRequired();

                // Ustawienie właściwości ReadDate jako wymaganej
                eb.Property(n => n.ReadDate);

                // Ustawienie UserId jako wymagane
                eb.Property(n => n.UserId)
                    .IsRequired();


                // Konfiguracja relacji jeden-do-wielu z tabelą Users
                eb.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.NoAction);  // Kaskadowe usuwanie powiązanych powiadomień
            });

            builder.Entity<Reports>(eb =>
            {
                // Ustawienie właściwości status jako wymaganej i maksymalnej długości 50 znaków
                eb.Property(r => r.status)
                    .IsRequired()
                    .HasMaxLength(50);

                // Ustawienie właściwości message jako wymaganej i maksymalnej długości 500 znaków
                eb.Property(r => r.message)
                    .IsRequired()
                    .HasMaxLength(500);

                // Ustawienie właściwości note jako opcjonalnej
                eb.Property(r => r.note)
                    .HasMaxLength(500);

                // Ustawienie właściwości date jako wymaganej
                eb.Property(r => r.date)
                    .IsRequired();


                // Relacja jeden-do-wielu z tabelą Comments
                eb.HasOne(r => r.SuspectComment)
                   .WithMany(c => c.Reports)
                   .HasForeignKey(r => r.SuspectCommentId)
                   .OnDelete(DeleteBehavior.NoAction); // Usunięcie komentarza usuwa powiązane raporty

                // Relacja wiele-do-jednego z tabelą Users (Moderator)
                eb.HasOne(r => r.Moderator)
                  .WithMany(u => u.ModeratorReports)
                  .HasForeignKey(r => r.ModeratorId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie użytkownika nie usuwa raportów

                // Relacja wiele-do-jednego z tabelą Users (SuspectUser)
                eb.HasOne(r => r.SuspectUser)
                  .WithMany(u => u.SuspectUserReports)
                  .HasForeignKey(r => r.SuspectUserId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie użytkownika nie usuwa raportów
            });

            builder.Entity<Reservation>(eb =>
            {
                // Ustawienie właściwości startDate jako wymaganej
                eb.Property(r => r.startDate)
                    .IsRequired();

                // Ustawienie właściwości endDate jako wymaganej
                eb.Property(r => r.endDate)
                    .IsRequired();

                // Ustawienie właściwości status jako wymaganej i maksymalnej długości 50 znaków
                eb.Property(r => r.status)
                    .IsRequired()
                    .HasMaxLength(50);

                // Ustawienie właściwości CharterId i UserId jako wymaganych
                eb.Property(r => r.CharterId).IsRequired();

                eb.Property(r => r.UserId).IsRequired();


                // Relacja jeden-do-wielu z tabelą Charters
                eb.HasOne(r => r.Charter)
                    .WithMany(c => c.Reservations)
                    .HasForeignKey(r => r.CharterId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relacja jeden-do-wielu z tabelą Users
                eb.HasOne(r => r.User)
                    .WithMany(u => u.Reservation)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Roles>(eb =>
            {
                eb.Property(r => r.certificates)
                    .IsRequired(false);

                // Relacja jeden-do-wielu z tabelą Users
               /* eb.HasMany(r => r.Users)
                    .WithOne(u => u.Role) // Każdy użytkownik ma jedną rolę
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.NoAction); // Usuwanie roli nie usuwa użytkowników
*/
                // Relacja jeden-do-wielu z tabelą Reports
                eb.HasMany(r => r.VeryficationReports)
                    .WithOne(rep => rep.DocumentVerification) // Raport ma przypisaną rolę dokumentującą
                    .HasForeignKey(rep => rep.DocumentVerificationId)
                    .OnDelete(DeleteBehavior.NoAction); // Usuwanie roli nie spowoduje usunięcia raportów
                // Relacja jeden-do-wielu z tabelą Reports
                eb.HasMany(r => r.SuspectReports)
                    .WithOne(rep => rep.SuspectRole) // Raport ma przypisaną rolę dokumentującą
                    .HasForeignKey(rep => rep.SuspectRoleId)
                    .OnDelete(DeleteBehavior.NoAction); // Usuwanie roli nie spowoduje usunięcia raportów
            });

            builder.Entity<Users>(eb =>
            {
                /*eb.Property(u => u.username)
                  .IsRequired()
                  .HasMaxLength(255)
                  .IsUnicode(false); // Możemy ustawić brak Unicode dla poprawy wydajności

                eb.Property(u => u.password)
                    .IsRequired()
                    .HasMaxLength(50);

                eb.Property(u => u.email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false) // Ustawienie braku Unicode dla emaili
                    .HasAnnotation("Index", "IX_Email") // Dodanie indeksu na email
                    .HasAnnotation("RegularExpression", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // Walidacja wzorca dla emaila

                eb.Property(u => u.phoneNumber)
                    .HasMaxLength(20)
                    .HasAnnotation("RegularExpression", @"^\+?\d{1,15}$"); // Dodanie wzorca dla numeru telefonu*/

                eb.Property(u => u.firstName)
                    .HasMaxLength(100);

                eb.Property(u => u.lastName)
                    .HasMaxLength(100);

                //eb.HasIndex(u => u.username).IsUnique(); // Ustawienie unikalności dla username
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

                eb.Property(y => y.capacity)
                    .IsRequired(); // Wyporność jachtu

                // Relacja jeden do wielu z tabelą Comments
                eb.HasMany(y => y.Comments)
                  .WithOne(c => c.Yachts)
                  .HasForeignKey(c => c.YachtsId)
                  .OnDelete(DeleteBehavior.Cascade); // Usunięcie Yachts nie usuwa powiązanych Comments

                // Relacja jeden do wielu z tabelą Reports
                eb.HasMany(y => y.Reports)
                  .WithOne(r => r.SuspectYacht)
                  .HasForeignKey(r => r.SuspectYachtId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie Yachts nie usuwa powiązanych Reports
                
                // Konfiguracja relacji z tabelą Users (Owner)
                eb.HasOne(y => y.Owner)
                  .WithMany(u => u.Yachts)
                  .HasForeignKey(y => y.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie użytkownika nie usuwa powiązanych jachtów

            });

            builder.Entity<YachtSale>(eb =>
            {
                //Sold Yachts można dać tabele jechty sprzedane
                // Dodatkowe konfiguracje
                eb.Property(ys => ys.saleDate).IsRequired();
                eb.Property(ys => ys.price).IsRequired();
                eb.Property(ys => ys.currency).IsRequired().HasMaxLength(3); // Zmieniono na string
                eb.Property(ys => ys.location).HasMaxLength(100);
                //eb.Property(ys => ys.availabilityStatus).HasMaxLength(50);
                eb.Property(ys => ys.notes).HasMaxLength(500); // Dodatkowe uwagi - maksymalna długość


                // Relacja jeden do wielu z tabelą Yachts
                eb.HasOne(ys => ys.Yacht)
                  .WithMany(y => y.YachtSale)
                  .HasForeignKey(ys => ys.YachtId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie Yacht usuwa powiązane YachtSales

                // Relacja jeden do wielu z tabelą Users (kupujący)
                eb.HasOne(ys => ys.BuyerUser)
                  .WithMany(u => u.YachtSalesAsSeller)
                  .HasForeignKey(ys => ys.BuyerUserId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie kupującego nie usuwa YachtSale

                // Relacja jeden do wielu z tabelą Users (właściciel)
                eb.HasOne(ys => ys.Owner)
                  .WithMany(u => u.YachtSalesAsBuyer)
                  .HasForeignKey(ys => ys.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie właściciela nie usuwa YachtSale

                // Nowa relacja jeden-do-wielu z tabelą Reports
                eb.HasMany(ys => ys.Reports)
                  .WithOne(r => r.SuspectYachtSale)
                  .HasForeignKey(r => r.SuspectYachtSaleId)
                  .OnDelete(DeleteBehavior.NoAction); // Usunięcie YachtSale nie usuwa powiązanych Reports
            });

            base.OnModelCreating(builder);
            SeedData(builder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            //Przykładowe dane do tabeli DBAnime
/*
            modelBuilder.Entity<Roles>().HasData(
                 new Roles
                 {
                     Id = 0  
                 });*/
        }
    }
}
