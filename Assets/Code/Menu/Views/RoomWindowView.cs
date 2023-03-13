using System.Collections.Generic;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Menu.Abstractions;
using Code.Menu.Models;
using Code.Shared.Constants;
using Code.Shared.Enums;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Menu.Views
{
    public class RoomWindowView: BaseWindowView
    {
        [Space, SerializeField] private string _roomPlayersListName = "RoomPlayersList";
        [SerializeField] private string _startGameButtonName = "StartGameButton";
        [SerializeField] private string _leaveButtonName = "LeaveButton";
        
        [Space, SerializeField] private VisualTreeAsset _roomPlayerItemTemplate;
        [SerializeField] private string _playerNicknameLabelName = "PlayerNickname";
        [SerializeField] private string _playerIconElementName = "PlayerIcon";
        [SerializeField] private string _playerIsCreatorIconElementName = "PlayerIsCreatorIcon";
        [SerializeField] private string _playerKickButtonName = "PlayerKickButton";

        private List<RoomPlayerItemModel> _players;
        private ListView _playerListView;

        private Button _startGameButton;
        private Button _leaveButton;

        protected override void Start()
        {
            base.Start();
            
            _players = new List<RoomPlayerItemModel>();

            _playerListView = Window.Q<ListView>(_roomPlayersListName);
            _playerListView.makeItem = () =>
            {
                return _roomPlayerItemTemplate.Instantiate();
            };
            
            _playerListView.bindItem = (visualElement, index) =>
            {
                var item = _players[index];
                visualElement.Q<Label>(_playerNicknameLabelName).text = item.Nickname;
                visualElement.Q<VisualElement>(_playerIconElementName).visible = !item.IsCreator;
                visualElement.Q<VisualElement>(_playerIsCreatorIconElementName).visible = item.IsCreator;
                
                if (PhotonNetwork.IsMasterClient && !item.IsCreator && item.Player != null)
                {
                    var playerKickButton = visualElement.Q<Button>(_playerKickButtonName);
                    playerKickButton.clicked += () => OnPlayerKickButton(item);
                }
            };
            _playerListView.fixedItemHeight = 300;
            _playerListView.itemsSource = _players;


            _startGameButton = Window.Q<Button>(_startGameButtonName);
            _startGameButton.clicked += StartGame;
            
            _leaveButton = Window.Q<Button>(_leaveButtonName);
            _leaveButton.clicked += LeaveRoom;
            
            
            Singleton<RoomManager>.Instance.OnPlayerJoin += OnPlayerJoin;
            Singleton<RoomManager>.Instance.OnPlayerLeave += OnPlayerLeave;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _leaveButton.clicked -= LeaveRoom;
            
            Singleton<RoomManager>.Instance.OnPlayerJoin -= OnPlayerJoin;
            Singleton<RoomManager>.Instance.OnPlayerLeave -= OnPlayerLeave;
        }


        public override void Open()
        {
            base.Open();
            
            _players.Clear();
            
            if (PhotonNetwork.IsMasterClient)
            {
                _players.Add(new RoomPlayerItemModel()
                {
                    Nickname = PhotonNetwork.LocalPlayer.NickName,
                    Player = PhotonNetwork.LocalPlayer,
                    IsCreator = PhotonNetwork.LocalPlayer.IsMasterClient
                });
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players)
                {
                    if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
                        continue;
                    
                    _players.Add(new RoomPlayerItemModel()
                    {
                        Nickname = player.NickName,
                        Player = player,
                        IsCreator = player.IsMasterClient
                    });
                }
            }

            var botsCount = (byte) Singleton<RoomManager>.Instance.CurrentRoom.CustomProperties[PhotonRoomKeys.RoomBotsCountKey];
            if (botsCount != 0)
            {
                for (var i = 0; i < botsCount; i++)
                {
                    _players.Add(new RoomPlayerItemModel()
                    {
                        Nickname = $"Bot {i}",
                        Player = null,
                        IsCreator = false
                    });
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                _startGameButton.visible = true;
            }
            else
            {
                _startGameButton.visible = false;
            }
            
            _playerListView.RefreshItems();
        }


        private void OnPlayerKickButton(RoomPlayerItemModel roomPlayerItemModel)
        {
            Singleton<RoomManager>.Instance.KickPlayer(roomPlayerItemModel.Player);
            _players.Remove(roomPlayerItemModel);
            
            _playerListView.RefreshItems();
        }
        
        private void OnPlayerLeave(Player player)
        {
            var playerIndex = _players.FindIndex(x => x.UserId == player.UserId);
            _players.RemoveAt(playerIndex);
            
            _playerListView.RefreshItems();
        }

        private void OnPlayerJoin(Player player)
        {
            _players.Add(new RoomPlayerItemModel()
            {
                Nickname = player.NickName,
                Player = player,
                IsCreator = player.IsMasterClient
            });
            
            _playerListView.RefreshItems();
        }

        
        private void StartGame()
        {
            DLogger.Debug(GetType(), nameof(StartGame), 
                "Game Start Button Clicked!");
            Singleton<RoomManager>.Instance.EditRoom(null, false);
            PhotonNetwork.LoadLevel((int) EScenesIndexes.Game);
        }
        
        public void LeaveRoom()
        {
            DLogger.Debug(GetType(), nameof(LeaveRoom), 
                "Leave Room Button Clicked!");
            
            WindowWrapper.Remove(Window);
            Singleton<RoomManager>.Instance.LeaveRoom();
        }
    }
}