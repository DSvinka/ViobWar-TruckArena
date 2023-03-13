using UnityEngine;

namespace Code.Shared.Constants.Messages
{
    public static class PhotonMessages
    {
        public const string RoomEditSuccessText = "Room Edit Success";
        public const int RoomEditSuccessLifeTime = 1;
        public static readonly Color RoomEditSuccessColor = Color.green;
        
        public const string YourRoomOwnerText = "You are now the owner of the room";
        public const int YourRoomOwnerLifeTime = 1;
        public static readonly Color YourRoomOwnerColor = Color.cyan;
    }
}