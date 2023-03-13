using System;
using System.Collections.Generic;
using Code.Core.Abstractions.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Photon.Pun;
using Photon.Realtime;

namespace Code.Core.Managers
{
    public class LobbyManager: MonoBehaviourPunCallbacks, ILobbyManager, ISingleton
    {
        #region Public Events

        public event Action<TypedLobby> OnLobbyJoin;
        public event Action OnLobbyLeave;
        
        public event Action<List<RoomInfo>> OnRoomListUpdated;

        #endregion

        public void JoinLobby()
        {
            if (PhotonNetwork.InLobby)
            {
                DLogger.Warning(GetType(), nameof(JoinLobby),
                    "The client is already in the lobby!");
                
                return;
            }
            
            PhotonNetwork.JoinLobby();
        }
        
        public void LeaveLobby()
        {
            if (!PhotonNetwork.InLobby)
            {
                DLogger.Warning(GetType(), nameof(JoinLobby),
                    "The client is not in the lobby!");
                
                return;
            }
            
            PhotonNetwork.LeaveLobby();
        }

        #region Photon Lobby Callbacks

        public override void OnJoinedLobby()
        {
            DLogger.Debug(GetType(), nameof(OnJoinedLobby), 
                $"Client joined to lobby!");
            
            OnLobbyJoin?.Invoke(PhotonNetwork.CurrentLobby);
        }

        public override void OnLeftLobby()
        {
            DLogger.Debug(GetType(), nameof(OnJoinedLobby), 
                $"Client leaved from lobby");
            
            OnLobbyLeave?.Invoke();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            DLogger.Debug(GetType(), nameof(OnRoomListUpdate), 
                $"Alive Rooms Count: {roomList.Count}");

            OnRoomListUpdated?.Invoke(roomList);
        }

        #endregion
    }
}