using System;
using HotelManagementApp.Services;
using System.Linq;
using HotelManagementApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementApp.Controllers;

    public class BookingController
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("1. List of available Hotels");
                Console.WriteLine("2. Exit");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    var hotels = _bookingService.GetAvailableHotels();
                    foreach (var hotel in hotels)
                    {
                        Console.WriteLine($"Hotel ID: {hotel.HotelId}, Name: {hotel.Name}, Address: {hotel.Address}");
                    }

                    Console.Write("Enter Hotel ID: ");
                    var hotelId = int.Parse(Console.ReadLine());

                    Console.Write("Enter Check-in Date (yyyy-MM-dd): ");
                    var checkInDate = DateTime.Parse(Console.ReadLine());

                    Console.Write("Enter Check-out Date (yyyy-MM-dd): ");
                    var checkOutDate = DateTime.Parse(Console.ReadLine());

                    if (_bookingService.CheckRoomAvailability(hotelId, checkInDate, checkOutDate))
                    {
                        Console.Write("Enter User ID: ");
                        var userId = int.Parse(Console.ReadLine());

                        if (_bookingService.BookRoom(hotelId, userId, checkInDate, checkOutDate))
                        {
                            Console.WriteLine("Room successfully booked!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to book the room.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The selected dates are not available.");
                    }
                }
                else if (choice == "2")
                {
                    break;
                }
            }
        }
    }

