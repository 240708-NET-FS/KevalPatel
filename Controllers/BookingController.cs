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
                Console.WriteLine("2. View Booking by Confirmation Number");
                Console.WriteLine("3. Exit");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    ListAvailableHotels();
                }
                else if (choice == "2")
                {
                    ViewBookingByConfirmationNumber();
                }
                else if (choice == "3")
                {
                    break;
                }
            }
        }

        private void ListAvailableHotels()
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
                Console.Write("Enter User Name: ");
                var userName = Console.ReadLine();

                var user = _bookingService.GetUserByName(userName);
                if (user == null)
                {
                    Console.WriteLine("User not found. Creating a new user.");
                    user = new User { Name = userName };
                    _bookingService.AddUser(user);
                }

                var bookingSuccess = _bookingService.BookRoom(hotelId, user.UserId, checkInDate, checkOutDate);
                if (bookingSuccess)
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

        private void ViewBookingByConfirmationNumber()
        {
            Console.Write("Enter Confirmation Number: ");
            var confirmationNumber = Console.ReadLine();

            var booking = _bookingService.GetBookingByConfirmationNumber(confirmationNumber);

            if (booking != null)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}");
                Console.WriteLine($"Hotel ID: {booking.Room.Hotel.HotelId}");
                Console.WriteLine($"Room ID: {booking.RoomId}");
                Console.WriteLine($"User ID: {booking.UserId}");
                Console.WriteLine($"Check-in Date: {booking.CheckInDate}");
                Console.WriteLine($"Check-out Date: {booking.CheckOutDate}");
                Console.WriteLine($"Confirmation Number: {booking.ConfirmationNumber}");
            }
            else
            {
                Console.WriteLine("Booking not found.");
            }
        }
    }
