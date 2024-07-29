using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HotelBookingApp.Repository;

public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("services/appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("services/appsettings.json")
                    .Build();

                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                builder.UseSqlServer(connectionString);

                return new ApplicationDbContext(builder.Options);
            }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>().HasKey(h => h.HotelId);
            modelBuilder.Entity<Room>().HasKey(r => r.RoomId);
            modelBuilder.Entity<Booking>().HasKey(b => b.BookingId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.FirstName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.LastName)
                .IsUnique();

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { HotelId = 1, Name = "Hilton NYC", Address = "123 Street A" },
                new Hotel { HotelId = 2, Name = "HolidayInn NYC", Address = "456 Street B" }
            );

            for (int i = 1; i <= 50; i++)
            {
                modelBuilder.Entity<Room>().HasData(
                    new Room { RoomId = i, HotelId = 1, IsAvailable = true, Price = 125m }
                );
            }

            for (int i = 51; i <= 100; i++)
            {
                modelBuilder.Entity<Room>().HasData(
                    new Room { RoomId = i, HotelId = 2, IsAvailable = true, Price = 75m }
                );
            }
        }
    }