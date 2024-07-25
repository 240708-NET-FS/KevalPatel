using System.Collections.Generic;

namespace HotelManagementApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
