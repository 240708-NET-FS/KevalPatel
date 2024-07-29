using HotelBookingApp.Repository;
using HotelBookingApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HotelBookingApp.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed data for testing
            SeedDatabase();

            _service = new BookingService(_context);
        }

        private void SeedDatabase()
        {
            _context.Hotels.Add(new Hotel { /* Initialization */ });
            _context.Rooms.Add(new Room { /* Initialization */ });
            _context.Users.Add(new User { /* Initialization */ });
            _context.Bookings.Add(new Booking { /* Initialization */ });
            _context.SaveChanges();
        }

        [Fact]
        public void GetAvailableHotels_ShouldReturnAllHotels()
        {
            // Act
            var result = _service.GetAvailableHotels();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetBookingByConfirmationNumber_ShouldReturnCorrectBooking()
        {
            // Arrange
            var confirmationNumber = "ABC123";
            var booking = new Booking { ConfirmationNumber = confirmationNumber, /* Initialization */ };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Act
            var result = _service.GetBookingByConfirmationNumber(confirmationNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(confirmationNumber, result.ConfirmationNumber);
        }
    }
}
