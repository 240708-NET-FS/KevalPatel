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

