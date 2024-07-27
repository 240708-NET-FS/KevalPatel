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
                Console.WriteLine($"Hotel ID: {hotel.HotelId}, Name: {hotel.Name}, Address: {hotel.Address}, Price: {hotel.Rooms.First().Price:C}/day");
            }

            Console.Write("Enter Hotel ID: ");
            var hotelId = int.Parse(Console.ReadLine());

            Console.Write("Enter Check-in Date (MM/dd/yyyy): ");
            var checkInDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Check-out Date (MM/dd/yyyy): ");
            var checkOutDate = DateTime.Parse(Console.ReadLine());

            if (_bookingService.CheckRoomAvailability(hotelId, checkInDate, checkOutDate))
            {
                Console.Write("Enter First Name: ");
                var firstName = Console.ReadLine();

                Console.Write("Enter Last Name: ");
                var lastName = Console.ReadLine();

                var user = _bookingService.GetOrCreateUser(firstName, lastName);
                var confirmationNumber = _bookingService.GenerateConfirmationNumber();

                if (_bookingService.BookRoom(hotelId, user.UserId, checkInDate, checkOutDate, confirmationNumber))
                {
                    Console.WriteLine($"Room successfully booked! Your confirmation number is {confirmationNumber}");
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

            Console.Write("Enter Last Name: ");
            var lastName = Console.ReadLine();

            var booking = _bookingService.GetBookingByConfirmationNumberAndLastName(confirmationNumber, lastName);
            if (booking != null)
            {
                Console.WriteLine($"Booking found for {booking.User.FirstName} {booking.User.LastName} at {booking.Room.Hotel.Name}.");
                Console.WriteLine($"Check-in Date: {booking.CheckInDate:MM/dd/yyyy}");
                Console.WriteLine($"Check-out Date: {booking.CheckOutDate:MM/dd/yyyy}");
                Console.WriteLine($"Total Cost: {booking.Room.Price * (booking.CheckOutDate - booking.CheckInDate).Days:C}");
            }
            else
            {
                Console.WriteLine("Booking not found.");
            }
        }
    }