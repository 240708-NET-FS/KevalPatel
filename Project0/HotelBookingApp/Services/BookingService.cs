using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Hotel> GetAvailableHotels()
        {
            return _context.Hotels.Include(h => h.Rooms).ToList();
        }

        public bool CheckRoomAvailability(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            return _context.Rooms.Any(r => r.HotelId == hotelId && r.IsAvailable && 
                                           !_context.Bookings.Any(b => b.RoomId == r.RoomId &&
                                                                       ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                                                                        (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate) ||
                                                                        (checkInDate <= b.CheckInDate && checkOutDate >= b.CheckOutDate))));
        }

        public bool BookRoom(int hotelId, int userId, DateTime checkInDate, DateTime checkOutDate, string confirmationNumber)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.HotelId == hotelId && r.IsAvailable && 
                                                          !_context.Bookings.Any(b => b.RoomId == r.RoomId &&
                                                                                      ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                                                                                       (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate) ||
                                                                                       (checkInDate <= b.CheckInDate && checkOutDate >= b.CheckOutDate))));
            if (room != null)
            {
                room.IsAvailable = false;
                var booking = new Booking
                {
                    RoomId = room.RoomId,
                    UserId = userId,
                    CheckInDate = checkInDate.Date,
                    CheckOutDate = checkOutDate.Date,
                    ConfirmationNumber = confirmationNumber
                };
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public Booking GetBookingByConfirmationNumberAndLastName(string confirmationNumber, string lastName)
        {
            return _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Where(b => b.ConfirmationNumber == confirmationNumber && b.User.LastName == lastName)
                .FirstOrDefault();
        }

        public string GenerateConfirmationNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public User GetOrCreateUser(string firstName, string lastName)
        {
            var user = _context.Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);
            if (user == null)
            {
                user = new User { FirstName = firstName, LastName = lastName };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            return user;
        }

        public decimal CalculateTotalCost(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.HotelId == hotelId);
            if (room == null) throw new Exception("Room not found.");
            var days = (checkOutDate.Date - checkInDate.Date).Days;
            return room.Price * days;
        }
    }
}
