using System;
using System.Collections.Generic;

namespace HotelBookingApp.Repository;

    public class Room
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public virtual Hotel Hotel { get; set; }
    }