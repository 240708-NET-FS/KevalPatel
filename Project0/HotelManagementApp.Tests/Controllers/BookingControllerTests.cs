using Moq;
using Xunit;
using System;
using System.Collections.Generic;

public class BookingControllerTests
{
    private readonly Mock<BookingService> _mockService;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _mockService = new Mock<BookingService>();
        _controller = new BookingController(_mockService.Object);
    }

    [Fact]
    public void ListAvailableHotels_PrintsHotelsAndBooksRoomSuccessfully()
    {
        // Arrange
        var hotels = new List<Hotel>
        {
            new Hotel
            {
                HotelId = 1,
                Name = "Test Hotel",
                Address = "123 Test St",
                Rooms = new List<Room>
                {
                    new Room { RoomId = 1, HotelId = 1, IsAvailable = true, Price = 100 }
                }
            }
        };
        _mockService.Setup(s => s.GetAvailableHotels()).Returns(hotels);
        _mockService.Setup(s => s.CheckRoomAvailability(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(true);
        _mockService.Setup(s => s.GetOrCreateUser(It.IsAny<string>(), It.IsAny<string>())).Returns(new User { UserId = 1 });
        _mockService.Setup(s => s.GenerateConfirmationNumber()).Returns("ABC123");
        _mockService.Setup(s => s.BookRoom(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(true);

        // Act
        Console.SetIn(new System.IO.StringReader("1\n1\n07/30/2024\n07/31/2024\nJohn\nDoe\n3\n"));
        _controller.Start();

        // Assert
        _mockService.Verify(s => s.BookRoom(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ViewBookingByConfirmationNumber_PrintsBookingDetails()
    {
        // Arrange
        var booking = new Booking
        {
            ConfirmationNumber = "ABC123",
            User = new User { FirstName = "John", LastName = "Doe" },
            Room = new Room { Hotel = new Hotel { Name = "Test Hotel" }, Price = 100 },
            CheckInDate = new DateTime(2024, 07, 30),
            CheckOutDate = new DateTime(2024, 07, 31)
        };
        _mockService.Setup(s => s.GetBookingByConfirmationNumberAndLastName(It.IsAny<string>(), It.IsAny<string>())).Returns(booking);

        // Act
        Console.SetIn(new System.IO.StringReader("2\nABC123\nDoe\n3\n"));
        _controller.Start();

        // Assert
        _mockService.Verify(s => s.GetBookingByConfirmationNumberAndLastName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
