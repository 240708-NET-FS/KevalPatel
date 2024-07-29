using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using HotelBookingApp.Repository;
using HotelBookingApp.Services;
using System.Linq;

namespace HotelBookingApp.Tests.Services
{
    [TestClass]
    public class BookingServiceTests
    {
        private ApplicationDbContext _context;
        private BookingService _bookingService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "HotelBookingTestDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _bookingService = new BookingService(_context);

            // Seed data
            _context.Hotels.Add(new Hotel { HotelId = 1, Name = "Test Hotel", Address = "123 Test St" });
            _context.Rooms.Add(new Room { RoomId = 1, HotelId = 1, IsAvailable = true, Price = 100m });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public void TestGetAvailableHotels()
        {
            var hotels = _bookingService.GetAvailableHotels();
            Assert.AreEqual(1, hotels.Count);
            Assert.AreEqual("Test Hotel", hotels[0].Name);
        }

        [TestMethod]
        public void TestCheckRoomAvailability()
        {
            var isAvailable = _bookingService.CheckRoomAvailability(1, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.IsTrue(isAvailable);
        }

        [TestMethod]
        public void TestBookRoom()
        {
            var user = _bookingService.GetOrCreateUser("John", "Doe");
            var confirmationNumber = _bookingService.GenerateConfirmationNumber();
            var success = _bookingService.BookRoom(1, user.UserId, DateTime.Today, DateTime.Today.AddDays(1), confirmationNumber);

            Assert.IsTrue(success);
            var booking = _context.Bookings.Include(b => b.Room).Include(b => b.User).FirstOrDefault();
            Assert.IsNotNull(booking);
            Assert.AreEqual(user.UserId, booking.UserId);
            Assert.AreEqual(confirmationNumber, booking.ConfirmationNumber);
        }

        [TestMethod]
        public void TestGetBookingByConfirmationNumberAndLastName()
        {
            var user = _bookingService.GetOrCreateUser("John", "Doe");
            var confirmationNumber = _bookingService.GenerateConfirmationNumber();
            _bookingService.BookRoom(1, user.UserId, DateTime.Today, DateTime.Today.AddDays(1), confirmationNumber);

            var booking = _bookingService.GetBookingByConfirmationNumberAndLastName(confirmationNumber, "Doe");
            Assert.IsNotNull(booking);
            Assert.AreEqual(user.UserId, booking.UserId);
            Assert.AreEqual(confirmationNumber, booking.ConfirmationNumber);
        }
    }
}
