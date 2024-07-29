using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelBookingApp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var expected = true;

            // Act
            var actual = true;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
        }
    }
}
