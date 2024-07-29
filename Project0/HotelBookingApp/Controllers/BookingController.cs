using System;
using System.Linq;
using HotelBookingApp.Services; // Ensure this using directive is present
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
            Console.WriteLine("\nWelcome to our Room Booking Portal\n");
            while (true)
            {
                Console.WriteLine("\n1. List of available Hotels");
                Console.WriteLine("2. Manage Existing Bookings");
                Console.WriteLine("3. Exit");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    ListAvailableHotels();
                }
                else if (choice == "2")
                {
                    ManageBookings();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.\n");
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
                        Console.WriteLine("Invalid Hotel ID. Please enter a valid Hotel ID.\n");
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
                                Console.WriteLine("Date must be in the future\n");
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Date must be in correct format\n");
                        }
                    }

                    Console.Write("Enter Check-out Date (MM/dd/yyyy): ");
                    var checkOutDate = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", null);

                    if (_bookingService.CheckRoomAvailability(hotelId, checkInDate, checkOutDate))
                    {
                        Console.Write("Enter Number of Rooms: ");
                        int numberOfRooms = int.Parse(Console.ReadLine());

                        Console.Write("Enter First Name: ");
                        var firstName = Console.ReadLine();

                        Console.Write("Enter Last Name: ");
                        var lastName = Console.ReadLine();

                        var user = _bookingService.GetOrCreateUser(firstName, lastName);
                        string confirmationNumber;

                        if (_bookingService.BookRooms(hotelId, user.UserId, checkInDate, checkOutDate, numberOfRooms, out confirmationNumber))
                        {
                            var totalCost = _bookingService.CalculateTotalCost(hotelId, checkInDate, checkOutDate) * numberOfRooms;
                            Console.WriteLine($"Rooms successfully booked! Your confirmation number is {confirmationNumber}. Please write it down as you will need it to lookup the reservation.\n");
                            Console.WriteLine($"Total due at check-in: {totalCost:C}\n");
                        }
                        else
                        {
                            Console.WriteLine("Failed to book the rooms. Not enough rooms available.\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The selected dates are not available.\n");
                    }

                    Console.WriteLine("\n1. Back to Main Menu");
                    var managementChoice = Console.ReadLine();
                    if (managementChoice == "1")
                    {
                        break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Date must be in correct format\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Please try again.\n");
                }
            }
        }

        private void ManageBookings()
        {
            Console.WriteLine("\nManage Existing Booking:");
            Console.WriteLine("1. View Booking");
            Console.WriteLine("2. Edit Booking");
            Console.WriteLine("3. Cancel Booking");
            Console.WriteLine("4. Back to Main Menu");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                ViewBooking();
            }
            else if (choice == "2")
            {
                EditBooking();
            }
            else if (choice == "3")
            {
                CancelBooking();
            }
            else if (choice == "4")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.\n");
            }
        }

        private void ViewBooking()
        {
            Console.Write("Enter Confirmation Number: ");
            var confirmationNumber = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            var lastName = Console.ReadLine();

            var booking = _bookingService.GetBookingByConfirmationNumberAndLastName(confirmationNumber, lastName);

            if (booking != null)
            {
                Console.WriteLine($"Booking found for {booking.User.FirstName} {booking.User.LastName} at {booking.Room.Hotel.Name}.\n");
                Console.WriteLine($"Check-in Date: {booking.CheckInDate:MM/dd/yyyy}");
                Console.WriteLine($"Check-out Date: {booking.CheckOutDate:MM/dd/yyyy}");
            }
            else
            {
                Console.WriteLine("Booking not found. Please enter a valid confirmation code and last name.\n");
            }
        }

        private void EditBooking()
        {
            Console.Write("Enter Confirmation Number: ");
            var confirmationNumber = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            var lastName = Console.ReadLine();

            var booking = _bookingService.GetBookingByConfirmationNumberAndLastName(confirmationNumber, lastName);

            if (booking != null)
            {
                Console.Write("Enter New Check-in Date (MM/dd/yyyy): ");
                DateTime newCheckInDate;
                while (!DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out newCheckInDate) || newCheckInDate <= DateTime.Now)
                {
                    Console.WriteLine("Date must be in the future and in correct format\n");
                    Console.Write("Enter New Check-in Date (MM/dd/yyyy): ");
                }

                Console.Write("Enter New Check-out Date (MM/dd/yyyy): ");
                DateTime newCheckOutDate;
                while (!DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out newCheckOutDate) || newCheckOutDate <= newCheckInDate)
                {
                    Console.WriteLine("Date must be after check-in date and in correct format\n");
                    Console.Write("Enter New Check-out Date (MM/dd/yyyy): ");
                }

                if (_bookingService.EditBooking(confirmationNumber, newCheckInDate, newCheckOutDate))
                {
                    Console.WriteLine("Booking updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update the booking. Please ensure the new dates are available.\n");
                }
            }
            else
            {
                Console.WriteLine("Booking not found.\n");
            }
        }

        private void CancelBooking()
        {
            Console.Write("Enter Confirmation Number: ");
            var confirmationNumber = Console.ReadLine();

            if (_bookingService.CancelBooking(confirmationNumber))
            {
                Console.WriteLine("Booking canceled successfully.");
            }
            else
            {
                Console.WriteLine("Failed to cancel the booking. Please check the confirmation number.\n");
            }
        }
    }
}