namespace HotelManagementApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public bool IsAvailable { get; set; }
        public Hotel Hotel { get; set; }
    }
}
