using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingApp.Controllers;
using HotelBookingApp.Repository;
using HotelBookingApp.Services;
using Moq;
using Xunit;

namespace HotelBookingApp.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<BookingService> _bookingServiceMock;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _bookingServiceMock = new Mock<BookingService>(Mock.Of<ApplicationDbContext>());
            _controller = new BookingController(_bookingServiceMock.Object);
        }

        [Fact]
        public void Start_ShouldListAvailableHotels()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { HotelId = 1, Name = "Hotel A", Address = "Address A", Rooms = new List<Room> { new Room { RoomId = 1, Price = 100, IsAvailable = true } } }
            };
            _bookingServiceMock.Setup(service => service.GetAvailableHotels()).Returns(hotels);

            // Assert
            _bookingServiceMock.Verify(service => service.GetAvailableHotels(), Times.Once);
        }

        [Fact]
        public void ViewBookingByConfirmationNumber_ShouldReturnBookingDetails()
        {
            // Arrange
            var booking = new Booking
            {
                User = new User { FirstName = "John", LastName = "Doe" },
                Room = new Room { Hotel = new Hotel { Name = "Hotel A" }, Price = 100 },
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(2)
            };
            _bookingServiceMock.Setup(service => service.GetBookingByConfirmationNumberAndLastName("ABC123", "Doe")).Returns(booking);

            // Act


            // Assert
            _bookingServiceMock.Verify(service => service.GetBookingByConfirmationNumberAndLastName("ABC123", "Doe"), Times.Once);
        }
    }
}
