using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Code.Menu.Views
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(LobbyWindowView), typeof(ProfileWindowView), typeof(SettingsWindowView))]
    [RequireComponent(typeof(RoomWindowView), typeof(AuthWindowView))]
    public class MainMenuView: MonoBehaviour
    {
        [SerializeField] private string _windowWrapperName = "WindowWrapper";
        [SerializeField, Space] private string _multiplayerButtonName = "MultiplayerButton";
        [SerializeField] private string _profileButtonName = "ProfileButton";
        [SerializeField] private string _settingsButtonName = "SettingsButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        
        
        private UIDocument _document;
        private VisualElement _windowWrapper;
        
        private AuthWindowView _authWindowView;
        private RoomWindowView _roomWindowView;
        private LobbyWindowView _lobbyWindowView;
        private ProfileWindowView _profileWindowView;
        private SettingsWindowView _settingsWindowView;
        
        private Button _lobbyWindowButton;
        private Button _profileWindowButton;
        private Button _settingsWindowButton;
        private Button _exitButton;

        private void Start()
        {
            if (!Singleton<ConnectionManager>.IsInited)
            {
                Singleton<ConnectionManager>.Init("ConnectionManager", false);
            }

            if (!Singleton<LobbyManager>.IsInited)
            {
                Singleton<LobbyManager>.Init("LobbyManager", false);
            }

            if (!Singleton<RoomManager>.IsInited)
            {
                Singleton<RoomManager>.Init("RoomManager", false);
            }
            
            if (!Singleton<AuthManager>.IsInited)
            {
                Singleton<AuthManager>.Init("AuthManager", false);
            }
            
            if (!Singleton<PlayerCurrencyManager>.IsInited)
            {
                Singleton<PlayerCurrencyManager>.Init("PlayerCurrencyManager", false);
            }
            
            if (!Singleton<PlayerInfoManager>.IsInited)
            {
                Singleton<PlayerInfoManager>.Init("PlayerInfoManager", false);
            }
            
            if (!Singleton<PlayerStatisticsManager>.IsInited)
            {
                Singleton<PlayerStatisticsManager>.Init("PlayerStatisticsManager", false);
            }
            
            
            Singleton<ConnectionManager>.Instance.Connect();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            _document = GetComponent<UIDocument>();
            _windowWrapper = _document.rootVisualElement.Q<VisualElement>(_windowWrapperName);

            _roomWindowView = GetComponent<RoomWindowView>();
            _lobbyWindowView = GetComponent<LobbyWindowView>();
            _profileWindowView = GetComponent<ProfileWindowView>();
            _settingsWindowView = GetComponent<SettingsWindowView>();
            
            _lobbyWindowButton = _document.rootVisualElement.Q<Button>(_multiplayerButtonName);
            _profileWindowButton = _document.rootVisualElement.Q<Button>(_profileButtonName);
            _settingsWindowButton = _document.rootVisualElement.Q<Button>(_settingsButtonName);
            _exitButton = _document.rootVisualElement.Q<Button>(_exitButtonName);
            
            _lobbyWindowButton.clicked += _lobbyWindowView.Open;
            _profileWindowButton.clicked += _profileWindowView.Open;
            _settingsWindowButton.clicked += _settingsWindowView.Open;
            _exitButton.clicked += Exit;
            
            
            Singleton<RoomManager>.Instance.OnRoomJoin += OnRoomJoin;
            Singleton<RoomManager>.Instance.OnRoomLeave += OnRoomLeave;
        }

        private void OnDestroy()
        {
            _lobbyWindowButton.clicked -= _lobbyWindowView.Open;
            _profileWindowButton.clicked -= _profileWindowView.Open;
            _settingsWindowButton.clicked -= _settingsWindowView.Open;
            _exitButton.clicked -= Exit;
            
            
            Singleton<RoomManager>.Instance.OnRoomJoin -= OnRoomJoin;
            Singleton<RoomManager>.Instance.OnRoomLeave -= OnRoomLeave;
        }
        

        private void OnRoomJoin(Room room)
        {
            DLogger.Debug(GetType(), nameof(OnRoomJoin), 
                "Room Join!");
            
            _windowWrapper.Clear();
            _roomWindowView.Open();
        }

        private void OnRoomLeave()
        {
            DLogger.Debug(GetType(), nameof(OnRoomLeave), 
                "Room Leave!");
            
            _windowWrapper.Clear();
        }

        private void Exit()
        {
            DLogger.Debug(GetType(), nameof(Exit), 
                "Exit Button Clicked!");
            
            Application.Quit();
        }
    }
}