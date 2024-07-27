using System;
using HotelManagementApp.Controllers;
using HotelManagementApp.Repository;
using HotelManagementApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementApp;
    class Program
    {
        static void Main(string[] args)
        {
            // Set up the configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("services/appsettings.json")
                .Build();

            // Set up the service collection
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlServer(connectionString);
                })
                .AddScoped<BookingService>()
                .AddScoped<BookingController>()
                .BuildServiceProvider();

            // Get the BookingController instance from the service provider
            var bookingController = serviceProvider.GetService<BookingController>();

            // Start the application
            bookingController.Start();
        }
    }