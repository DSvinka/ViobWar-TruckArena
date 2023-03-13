using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Code.Game.Views.Hud
{
    public class HudView: MonoBehaviour
    {
        public event Action OnLeaveRoomSubmit;

        [SerializeField] private string _windowWrapperElementName = "WindowWrapper";
        [SerializeField] private string _healthCountTextName = "HealthCountText";
        [SerializeField] private string _enemyHealthCountTextName = "EnemyHealthCountText";
        [SerializeField] private string _enemyInfoPanelName = "EnemyInfoPanel";

        [Space, SerializeField] private VisualTreeAsset _loseWindowTemplate;
        [SerializeField] private string _leaveRoomButtonNameInLose = "LeaveRoomButton";
        
        [Space, SerializeField] private VisualTreeAsset _winWindowTemplate;
        [SerializeField] private string _leaveRoomButtonNameInWin = "LeaveRoomButton";
        
        
        private UIDocument _document;
        private VisualElement _windowWrapper;
        
        
        private VisualElement _loseWindow;
        private VisualElement _winWindow;

        private Label _playerHealthCountText;
        private Label _enemyHealthCountText;

        private Button _leaveRoomButtonInLose;
        private Button _leaveRoomButtonInWin;

        private VisualElement _enemyInfoPanel;


        private void Start()
        {
            _document = GetComponent<UIDocument>();
            _windowWrapper = _document.rootVisualElement.Q<VisualElement>(_windowWrapperElementName);

            _playerHealthCountText = _document.rootVisualElement.Q<Label>(_healthCountTextName);
            _enemyHealthCountText = _document.rootVisualElement.Q<Label>(_enemyHealthCountTextName);

            _enemyInfoPanel = _document.rootVisualElement.Q<VisualElement>(_enemyInfoPanelName);

            _loseWindow = _loseWindowTemplate.CloneTree();
            _leaveRoomButtonInLose = _loseWindow.Q<Button>(_leaveRoomButtonNameInLose);
            _leaveRoomButtonInLose.clicked += OnLeaveRoomClick;
            
            _winWindow = _winWindowTemplate.CloneTree();
            _leaveRoomButtonInWin = _winWindow.Q<Button>(_leaveRoomButtonNameInWin);
            _leaveRoomButtonInWin.clicked += OnLeaveRoomClick;
        }

        public void OpenWinWindow()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            _windowWrapper.Add(_winWindow);
        }

        public void OpenLoseWindow()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            _windowWrapper.Add(_loseWindow);
        }

        public void OnLeaveRoomClick()
        {
            OnLeaveRoomSubmit?.Invoke();
        }

        public void SetPlayerHealth(float health)
        {
            _playerHealthCountText.text = health.ToString();
        }

        public void SetEnemyHealth(bool display, float health = 0)
        {
            _enemyInfoPanel.visible = display;
            _enemyHealthCountText.text = health.ToString();
        }
    }
}