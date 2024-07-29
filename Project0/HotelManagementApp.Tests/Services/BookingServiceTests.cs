using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Repository;
using HotelBookingApp.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HotelManagementApp.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _bookingService = new BookingService(_mockContext.Object);
        }

        [Fact]
        public void GetAvailableHotels_ReturnsHotels()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { HotelId = 1, Name = "Hotel One", Address = "Address One" },
                new Hotel { HotelId = 2, Name = "Hotel Two", Address = "Address Two" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Hotel>>();
            mockSet.As<IQueryable<Hotel>>().Setup(m => m.Provider).Returns(hotels.Provider);
            mockSet.As<IQueryable<Hotel>>().Setup(m => m.Expression).Returns(hotels.Expression);
            mockSet.As<IQueryable<Hotel>>().Setup(m => m.ElementType).Returns(hotels.ElementType);
            mockSet.As<IQueryable<Hotel>>().Setup(m => m.GetEnumerator()).Returns(hotels.GetEnumerator());

            _mockContext.Setup(c => c.Hotels).Returns(mockSet.Object);

            // Act
            var result = _bookingService.GetAvailableHotels();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Hotel One", result[0].Name);
            Assert.Equal("Hotel Two", result[1].Name);
        }

        [Fact]
        public void CheckRoomAvailability_RoomIsAvailable_ReturnsTrue()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { RoomId = 1, HotelId = 1, IsAvailable = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());

            _mockContext.Setup(c => c.Rooms).Returns(mockSet.Object);

            // Act
            var result = _bookingService.CheckRoomAvailability(1, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));

            // Assert
            Assert.True(result);
        }

        // Add more tests for other methods in BookingService
    }
}
