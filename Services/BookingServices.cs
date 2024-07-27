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
            return _context.Hotels.ToList();
        }

        public bool CheckRoomAvailability(int hotelId, DateTime checkInDate, DateTime checkOutDate)
        {
            var availableRooms = _context.Rooms
                .Where(r => r.HotelId == hotelId && r.IsAvailable)
                .ToList();

            // Assuming we have some logic to check if rooms are available in the date range
            return availableRooms.Any();
        }

        public bool BookRoom(int hotelId, int userId, DateTime checkInDate, DateTime checkOutDate)
        {
            var room = _context.Rooms
                .FirstOrDefault(r => r.HotelId == hotelId && r.IsAvailable);

            if (room == null) return false;

            var booking = new Booking
            {
                RoomId = room.RoomId,
                UserId = userId,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                ConfirmationNumber = GenerateConfirmationNumber()
            };

            room.IsAvailable = false;
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return true;
        }

        public Booking GetBookingByConfirmationNumber(string confirmationNumber)
        {
            return _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefault(b => b.ConfirmationNumber == confirmationNumber);
        }

        public User GetUserByName(string name)
        {
            return _context.Users.FirstOrDefault(u => u.Name == name);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private string GenerateConfirmationNumber()
        {
            return Guid.NewGuid().ToString(); // Generate a unique confirmation number
        }
    }
