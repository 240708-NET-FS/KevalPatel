using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Services;
using HotelBookingApp.Repository;

namespace HotelManagementApp.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _service = new BookingService(_mockContext.Object);
        }

        [Fact]
        public void GetAvailableHotels_ReturnsHotels()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { HotelId = 1, Name = "Test Hotel", Address = "123 Test St", Rooms = new List<Room>() }
            };
            var mockSet = CreateMockSet(hotels.AsQueryable());

            _mockContext.Setup(c => c.Hotels).Returns(mockSet.Object);

            // Act
            var result = _service.GetAvailableHotels();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Hotel", result[0].Name);
        }

        [Fact]
        public void CheckRoomAvailability_RoomIsAvailable_ReturnsTrue()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { RoomId = 1, HotelId = 1, IsAvailable = true }
            };
            var mockSet = CreateMockSet(rooms.AsQueryable());

            _mockContext.Setup(c => c.Rooms).Returns(mockSet.Object);

            // Act
            var result = _service.CheckRoomAvailability(1, DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void BookRoom_RoomIsAvailable_BooksRoomAndReturnsTrue()
        {
            // Arrange
            var room = new Room { RoomId = 1, HotelId = 1, IsAvailable = true };
            var mockRooms = CreateMockSet(new List<Room> { room }.AsQueryable());

            _mockContext.Setup(c => c.Rooms).Returns(mockRooms.Object);
            _mockContext.Setup(c => c.Bookings.Add(It.IsAny<Booking>()));

            // Act
            var result = _service.BookRoom(1, 1, DateTime.Now, DateTime.Now.AddDays(1), "ABC123");

            // Assert
            Assert.True(result);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetBookingByConfirmationNumberAndLastName_ValidBooking_ReturnsBooking()
        {
            // Arrange
            var user = new User { UserId = 1, FirstName = "John", LastName = "Doe" };
            var booking = new Booking
            {
                ConfirmationNumber = "ABC123",
                User = user,
                Room = new Room { Hotel = new Hotel { Name = "Test Hotel" } },
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(1)
            };
            var mockSet = CreateMockSet(new List<Booking> { booking }.AsQueryable());

            _mockContext.Setup(c => c.Bookings).Returns(mockSet.Object);

            // Act
            var result = _service.GetBookingByConfirmationNumberAndLastName("ABC123", "Doe");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ABC123", result.ConfirmationNumber);
            Assert.Equal("Doe", result.User.LastName);
        }

        [Fact]
        public void GenerateConfirmationNumber_ReturnsUniqueNumber()
        {
            // Act
            var confirmationNumber = _service.GenerateConfirmationNumber();

            // Assert
            Assert.NotNull(confirmationNumber);
            Assert.Equal(6, confirmationNumber.Length);
        }

        [Fact]
        public void GetOrCreateUser_UserDoesNotExist_CreatesUser()
        {
            // Arrange
            var mockSet = CreateMockSet(new List<User>().AsQueryable());

            _mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Users.Add(It.IsAny<User>()));

            // Act
            var user = _service.GetOrCreateUser("John", "Doe");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        private Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}
