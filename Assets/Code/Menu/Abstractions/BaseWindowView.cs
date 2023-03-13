using Code.Core.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Menu.Abstractions
{
    public abstract class BaseWindowView: MonoBehaviour
    {
        [SerializeField] protected VisualTreeAsset _windowTemplate;
        
        [Space, SerializeField] protected string _windowWrapperElementName = "WindowWrapper";
        [SerializeField] protected string _closeButtonName = "CloseButton";
        
        [Space, SerializeField] protected string _statusMessageText = "StatusMessageText";
        [SerializeField] protected string _statusMessagePanel = "StatusMessagePanel";
        [SerializeField] protected string _statusMessageCloseButton = "StatusMessageCloseButton";
        
        [Space, SerializeField] protected Color _errorStatusMessageColor = Color.red;
        [SerializeField] protected Color _successStatusMessageColor = Color.green;


        protected VisualElement Window;
        
        protected UIDocument WindowDocument;
        protected VisualElement WindowWrapper;
        
        protected Label StatusMessageText;
        protected VisualElement StatusMessagePanel;
        protected Button StatusMessageCloseButton;

        protected Button CloseButton;

        
        protected virtual void Start()
        {
            WindowDocument = GetComponent<UIDocument>();
            WindowWrapper = WindowDocument.rootVisualElement.Q<VisualElement>(_windowWrapperElementName);
            
            Window = _windowTemplate.CloneTree();

            StatusMessageText = Window.Q<Label>(_statusMessageText);
            StatusMessagePanel = Window.Q<VisualElement>(_statusMessagePanel);
            StatusMessageCloseButton = Window.Q<Button>(_statusMessageCloseButton);
            StatusMessageCloseButton.clicked += CloseMessage; 
            
            CloseButton = Window.Q<Button>(_closeButtonName);
            if (CloseButton != null)
            {
                CloseButton.clicked += Close;
            }
            
            CloseMessage();
        }

        protected virtual void OnDestroy()
        {
            StatusMessageCloseButton.clicked -= CloseMessage;
            if (CloseButton != null)
            {
                CloseButton.clicked -= Close;
            }
        }

        protected virtual void CloseMessage()
        {
            OpenMessage("", false, false);
        }

        protected virtual void OpenMessage(string message, bool isError, bool visible = true)
        {
            StatusMessageText.text = message;
            StatusMessagePanel.visible = visible;
            
            if (isError)
            {
                StatusMessagePanel.style.backgroundColor = new StyleColor(_errorStatusMessageColor);
            }
            else
            {
                StatusMessagePanel.style.backgroundColor = new StyleColor(_successStatusMessageColor);
            }
        }
        
        
        public virtual void Open()
        {
            DLogger.Debug(GetType(), nameof(Open), 
                $"Open!");
            
            WindowWrapper.Add(Window);
        }

        public virtual void Close()
        {
            DLogger.Debug(GetType(), nameof(Close), 
                "Close!");

            WindowWrapper.Remove(Window);
            
            foreach (var updateWindow in GetComponents<IUpdateWindow>())
            {
                if (updateWindow.GetType() != GetType())
                    updateWindow.UpdateWindow();
            }
        }
    }
}