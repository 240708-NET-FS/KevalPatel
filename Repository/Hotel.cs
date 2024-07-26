using System.Collections.Generic;

namespace HotelManagementApp.Repository;

        public class Hotel
        {
            public int HotelId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public List<Room> Rooms { get; set; }
        }

