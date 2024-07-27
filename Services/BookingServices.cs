using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagementApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementApp.Services;

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
            return _context.Rooms.Any(r => r.HotelId == hotelId && r.IsAvailable);
        }

        public bool BookRoom(int hotelId, int userId, DateTime checkInDate, DateTime checkOutDate, string confirmationNumber)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.HotelId == hotelId && r.IsAvailable);
            if (room != null)
            {
                room.IsAvailable = false;
                var booking = new Booking
                {
                    RoomId = room.RoomId,
                    UserId = userId,
                    CheckInDate = checkInDate,
                    CheckOutDate = checkOutDate,
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
    }