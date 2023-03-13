using System.Collections.Generic;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Menu.Abstractions;
using Code.Menu.Models;
using Code.Shared.Constants;
using Code.Shared.Models.Menu;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;

namespace Code.Menu.Views
{
    public class LobbyWindowView: BaseWindowView, IUpdateWindow
    {
        [SerializeField, Space] private string _popupWrapperName = "PopupWrapper";
        [SerializeField] private string _roomsListViewName = "RoomsList";
        [SerializeField] private string _openCreateRoomPopupButtonName = "OpenCreateRoomPopupButton";
        [SerializeField] private string _createRoomButtonName = "CreateRoomButton";

        [SerializeField, Space] private VisualTreeAsset _createRoomPopupTemplate;
        [SerializeField] private string _roomCloseButtonName = "CloseButton";
        [SerializeField] private string _roomNameInputName = "RoomNameInput";
        [SerializeField] private string _roomSizeInputName = "RoomSizeInput";
        [SerializeField] private string _roomBotsSizeInputName = "RoomBotsSizeInput";
        [SerializeField] private string _roomPasswordInputName = "RoomPasswordInput";
        
        [SerializeField, Space] private VisualTreeAsset _lobbyRoomItemTemplate;
        [SerializeField] private string _roomTitleName = "RoomTitle";
        [SerializeField] private string _roomPlayerCountName = "RoomPlayerCount";
        [SerializeField] private string _roomStatusTitleName = "RoomStatusTitle";
        [SerializeField] private string _roomJoinButtonName = "RoomJoinButton";
        
        private VisualElement _createRoomPopup;
        
        private List<LobbyRoomItemModel> _rooms;
        private ListView _roomsListView;
        
        private VisualElement _popupWrapper;

        private Button _openCreateRoomPopupButton;
        private Button _createRoomButton;

        private Button _roomCloseButton;
        private TextField _roomNameInput;
        private SliderInt _roomMaxPlayerCountSlider;
        private SliderInt _roomBotsCountSlider;
        private TextField _roomPasswordInput;


        protected override void Start()
        {
            base.Start();
            
            _rooms = new List<LobbyRoomItemModel>();
   
            _popupWrapper = WindowDocument.rootVisualElement.Q<VisualElement>(_popupWrapperName);
            
            _createRoomPopup = _createRoomPopupTemplate.CloneTree();
            
            _roomNameInput = _createRoomPopup.Q<TextField>(_roomNameInputName);
            _roomMaxPlayerCountSlider = _createRoomPopup.Q<SliderInt>(_roomSizeInputName);
            _roomBotsCountSlider = _createRoomPopup.Q<SliderInt>(_roomBotsSizeInputName);
            _roomPasswordInput = _createRoomPopup.Q<TextField>(_roomPasswordInputName);
            _roomCloseButton = _createRoomPopup.Q<Button>(_roomCloseButtonName);

            _roomCloseButton.clicked += CreateRoomClose;
            
            _roomsListView = Window.Q<ListView>(_roomsListViewName);
            _roomsListView.makeItem = () =>
            {
                return _lobbyRoomItemTemplate.Instantiate();
            };
            
            _roomsListView.bindItem = (visualElement, index) =>
            {
                var item = _rooms[index];
                visualElement.Q<Label>(_roomTitleName).text = item.Title;
                visualElement.Q<Label>(_roomPlayerCountName).text = $"{item.PlayerCurrentCount} / {item.PlayerMaxCount}";
                visualElement.Q<Label>(_roomStatusTitleName).text = item.IsPublic ? "Открытый" : "Закрытый";
                visualElement.Q<Button>(_roomJoinButtonName).clicked += () => OnRoomJoinButton(item.Code);
            };

            _roomsListView.fixedItemHeight = 100;
            _roomsListView.itemsSource = _rooms;

            _openCreateRoomPopupButton = Window.Q<Button>(_openCreateRoomPopupButtonName);
            _openCreateRoomPopupButton.clicked += OpenCreateRoom;
            
            _createRoomButton = _createRoomPopup.Q<Button>(_createRoomButtonName);
            _createRoomButton.clicked += CreateRoom;
            
            Singleton<ConnectionManager>.Instance.OnConnect += OnConnected;
            Singleton<LobbyManager>.Instance.OnRoomListUpdated += OnRoomListUpdated;
            
            if (!Singleton<ConnectionManager>.Instance.IsConnected)
                Singleton<ConnectionManager>.Instance.Connect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _rooms.Clear();

            _roomCloseButton.clicked -= CreateRoomClose;
            
            _openCreateRoomPopupButton.clicked -= OpenCreateRoom;
            _createRoomButton.clicked -= CreateRoom;

            Singleton<ConnectionManager>.Instance.OnConnect -= OnConnected;
            Singleton<LobbyManager>.Instance.OnRoomListUpdated -= OnRoomListUpdated;
        }


        private void OnRoomJoinButton(string code)
        {
            Singleton<RoomManager>.Instance.JoinRoom(code);
        }
        
        private void OnConnected()
        {
            Singleton<LobbyManager>.Instance.JoinLobby();
        }

        private void OnRoomListUpdated(List<RoomInfo> roomInfos)
        {
            _rooms.Clear();
            foreach (var roomInfo in roomInfos)
            {
                _rooms.Add(new LobbyRoomItemModel()
                {
                    Code = roomInfo.Name,
                    Title = roomInfo.CustomProperties[PhotonRoomKeys.RoomNameKey].ToString(),
                    IsPublic = roomInfo.CustomProperties[PhotonRoomKeys.RoomPasswordKey].ToString().IsNullOrEmpty(),
                    PlayerCurrentCount = roomInfo.PlayerCount,
                    PlayerMaxCount = roomInfo.MaxPlayers,
                });
            }
            _roomsListView.RefreshItems();
        }

        private void OpenCreateRoom()
        {
            DLogger.Debug(GetType(), nameof(OpenCreateRoom), 
                "CreateRoom Popup Open!");
            
            _popupWrapper.Add(_createRoomPopup);
        }
        
        private void CreateRoom()
        {
            DLogger.Debug(GetType(), nameof(CreateRoom), 
                "CreateRoom Submit!");
            
            Singleton<RoomManager>.Instance.CreateRoom(new EditRoomModel()
            {
                RoomName = _roomNameInput.value,
                RoomPassword = _roomPasswordInput.value,
                RoomMaxPlayers = (byte) _roomMaxPlayerCountSlider.value,
                RoomBotsCount = (byte) _roomBotsCountSlider.value
            });

            _popupWrapper.Remove(_createRoomPopup);
        }

        private void CreateRoomClose()
        {
            _popupWrapper.Remove(_createRoomPopup);
        }
        
        public override void Open()
        {
            base.Open();
            UpdateWindow();
        }

        public void UpdateWindow()
        {
            if (!WindowWrapper.Contains(Window))
                return;
            
            if (Singleton<AuthManager>.Instance.PlayFabId == null)
            {
                Close();
                
                var authWindowView = GetComponent<AuthWindowView>();
                authWindowView.Open();
                return;
            }
            
            Singleton<PlayerStatisticsManager>.Instance.GetStatistics();
        }
    }
}