using System;
using System.IO;
using HotelBookingApp.Controllers;
using HotelBookingApp.Services;
using Moq;
using Xunit;

namespace HotelManagementApp.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<BookingService> _mockBookingService;
        private readonly BookingController _bookingController;

        public BookingControllerTests()
        {
            _mockBookingService = new Mock<BookingService>();
            _bookingController = new BookingController(_mockBookingService.Object);
        }

        [Fact]
        public void Start_ValidInput_ExecutesCorrectly()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            var consoleInput = new StringReader("3\n");
            Console.SetIn(consoleInput);

            // Act
            _bookingController.Start();

            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("1. List of available Hotels", output);
            Assert.Contains("2. View Booking by Confirmation Number", output);
            Assert.Contains("3. Exit", output);
        }

        // Add more tests for other methods in BookingController
    }
}
