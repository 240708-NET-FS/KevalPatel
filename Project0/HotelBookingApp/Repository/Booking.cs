using System;
using System.Collections.Generic;


namespace HotelBookingApp.Repository;

    public class Booking
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string ConfirmationNumber { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
    }