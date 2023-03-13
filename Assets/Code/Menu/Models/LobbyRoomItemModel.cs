namespace Code.Menu.Models
{
    public class LobbyRoomItemModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        
        public int PlayerCurrentCount { get; set; }
        public int PlayerMaxCount { get; set; }
        
        public bool IsPublic { get; set; }
    }
}