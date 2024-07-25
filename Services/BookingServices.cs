using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagementApp.Repository;
using HotelManagementApp.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementApp.Services
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

        public bool CheckRoomAvailability(int hotelId, DateTime checkIn, DateTime checkOut)
        {
            var hotel = _context.Hotels.Include(h => h.Rooms).FirstOrDefault(h => h.HotelId == hotelId);
            if (hotel == null) return false;

            foreach (var room in hotel.Rooms)
            {
                var bookings = _context.Bookings.Where(b => b.RoomId == room.RoomId && 
                ((checkIn >= b.CheckInDate && checkIn <= b.CheckOutDate) || 
                (checkOut >= b.CheckInDate && checkOut <= b.CheckOutDate))).ToList();

                if (bookings.Count == 0 && room.IsAvailable) return true;
            }

            return false;
        }

        public bool BookRoom(int hotelId, int userId, DateTime checkIn, DateTime checkOut)
        {
            var hotel = _context.Hotels.Include(h => h.Rooms).FirstOrDefault(h => h.HotelId == hotelId);
            if (hotel == null) return false;

            var availableRoom = hotel.Rooms.FirstOrDefault(r => r.IsAvailable);
            if (availableRoom == null) return false;

            var booking = new Booking
            {
                RoomId = availableRoom.RoomId,
                UserId = userId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut
            };

            _context.Bookings.Add(booking);
            availableRoom.IsAvailable = false;
            _context.SaveChanges();

            return true;
        }

        public List<Booking> GetUserBookings(int userId)
        {
            return _context.Bookings.Where(b => b.UserId == userId).ToList();
        }
    }
}
