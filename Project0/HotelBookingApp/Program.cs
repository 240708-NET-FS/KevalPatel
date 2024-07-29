using System;
using HotelBookingApp.Controllers;
using HotelBookingApp.Repository;
using HotelBookingApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace HotelBookingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("services/appsettings.json")
                        .Build();
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlServer(connectionString);
                })
                .AddScoped<BookingService>()
                .AddScoped<BookingController>()
                .BuildServiceProvider();

            var bookingController = serviceProvider.GetService<BookingController>();
            bookingController.Start();
        }
    }
}
