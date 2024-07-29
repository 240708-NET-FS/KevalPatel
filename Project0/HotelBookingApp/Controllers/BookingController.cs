using System;
using HotelBookingApp.Services;
using System.Linq;
using HotelBookingApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Controllers
{
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
            while (true)
            {
                try
                {
                    var hotels = _bookingService.GetAvailableHotels();
                    foreach (var hotel in hotels)
                    {
                        Console.WriteLine($"Hotel ID: {hotel.HotelId}, Name: {hotel.Name}, Address: {hotel.Address}, Price: {hotel.Rooms.First().Price:C}/day");
                    }

                    Console.Write("Enter Hotel ID: ");
                    var hotelId = int.Parse(Console.ReadLine());
                    var selectedHotel = hotels.FirstOrDefault(h => h.HotelId == hotelId);

                    if (selectedHotel == null)
                    {
                        Console.WriteLine("Invalid Hotel ID. Please enter a valid Hotel ID.");
                        continue;
                    }

                    DateTime checkInDate;
                    while (true)
                    {
                        Console.Write("Enter Check-in Date (MM/dd/yyyy): ");
                        var input = Console.ReadLine();
                        if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out checkInDate))
                        {
                            if (checkInDate <= DateTime.Now)
                            {
                                Console.WriteLine("Date must be in the future");
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Date must be in correct format");
                        }
                    }

                    Console.Write("Enter Check-out Date (MM/dd/yyyy): ");
                    var checkOutDate = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", null);

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
                            var totalCost = _bookingService.CalculateTotalCost(hotelId, checkInDate, checkOutDate);
                            Console.WriteLine($"Room successfully booked! Your confirmation number is {confirmationNumber}. Please write it down as you will need it to lookup the reservation.");
                            Console.WriteLine($"Total due at check-in: {totalCost:C}");
                            break;
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
                catch (FormatException)
                {
                    Console.WriteLine("Date must be in correct format");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Please try again.");
                }
            }
        }

        private void ViewBookingByConfirmationNumber()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}. Please try again.");
            }
        }
    }
}
