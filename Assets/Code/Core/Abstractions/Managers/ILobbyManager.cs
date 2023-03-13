using System;
using System.Collections.Generic;
using Photon.Realtime;

namespace Code.Core.Abstractions.Managers
{
    public interface ILobbyManager
    {
        event Action<TypedLobby> OnLobbyJoin;
        event Action OnLobbyLeave;
        
        event Action<List<RoomInfo>> OnRoomListUpdated;

        /// <summary>
        /// Подключается к лобби
        /// </summary>
        void JoinLobby();

        /// <summary>
        /// Отключается от лобби
        /// </summary>
        void LeaveLobby();
    }
}