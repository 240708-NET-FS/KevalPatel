using System.Collections.Generic;

namespace HotelManagementApp.Repository;

    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }


