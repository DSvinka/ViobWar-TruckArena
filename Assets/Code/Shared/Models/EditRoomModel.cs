namespace Code.Shared.Models.Menu
{
    public class EditRoomModel
    {
        public string RoomName { get; set; }
        public string RoomPassword { get; set; }
        
        public byte RoomMaxPlayers { get; set; }
        public byte RoomBotsCount { get; set; }
    }
}