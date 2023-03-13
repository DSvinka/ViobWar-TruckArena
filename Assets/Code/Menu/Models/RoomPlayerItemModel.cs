using Photon.Realtime;

namespace Code.Menu.Models
{
    public class RoomPlayerItemModel
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        
        public Player Player { get; set; }
        
        public bool IsCreator { get; set; }
    }
}