using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Menu.Abstractions;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UIElements;
using WebSocketSharp;
using Button = UnityEngine.UIElements.Button;

namespace Code.Menu.Views
{
    public class AuthWindowView: BaseWindowView
    {
        [Space, SerializeField] private string _loginButtonName = "LoginButton";
        [SerializeField] private string _registerButtonName = "RegisterButton";
        
        [Space, SerializeField] private string _loginUsernameInputName = "LoginUsernameInput";
        [SerializeField] private string _loginPasswordInputName = "LoginPasswordInput";
        
        [Space, SerializeField] private string _registerEmailInputName = "RegisterEmailInput";
        [SerializeField] private string _registerUsernameInputName = "RegisterUsernameInput";
        [SerializeField] private string _registerPasswordInputName = "RegisterPasswordInput";
        [SerializeField] private string _registerPasswordRepeatInputName = "RegisterPasswordRepeatInput";
        
        
        private Button _loginButton;
        private Button _registerButton;
        
        private TextField _loginUsernameInput;
        private TextField _loginPasswordInput;
        
        private TextField _registerEmailInput;
        private TextField _registerUsernameInput;
        private TextField _registerPasswordInput;
        private TextField _registerPasswordRepeatInput;

        
        protected override void Start()
        {
            base.Start();

            _loginButton = Window.Q<Button>(_loginButtonName);
            _loginButton.clicked += OnLoginSubmit;
            
            _registerButton = Window.Q<Button>(_registerButtonName);
            _registerButton.clicked += OnRegisterSubmit;
            
            _loginUsernameInput = Window.Q<TextField>(_loginUsernameInputName);
            _loginPasswordInput = Window.Q<TextField>(_loginPasswordInputName);

            _registerEmailInput = Window.Q<TextField>(_registerEmailInputName);
            _registerUsernameInput = Window.Q<TextField>(_registerUsernameInputName);
            _registerPasswordInput = Window.Q<TextField>(_registerPasswordInputName);
            _registerPasswordRepeatInput = Window.Q<TextField>(_registerPasswordRepeatInputName);
            
            Singleton<AuthManager>.Instance.OnLoginSuccess += OnLoginSuccess;
            Singleton<AuthManager>.Instance.OnLoginFailed += OnAuthFailed;
            
            Singleton<AuthManager>.Instance.OnRegisterSuccess += OnRegisterSuccess;
            Singleton<AuthManager>.Instance.OnRegisterFailed += OnAuthFailed;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Singleton<AuthManager>.Instance.OnLoginSuccess -= OnLoginSuccess;
            Singleton<AuthManager>.Instance.OnLoginFailed -= OnAuthFailed;
            
            Singleton<AuthManager>.Instance.OnRegisterSuccess -= OnRegisterSuccess;
            Singleton<AuthManager>.Instance.OnRegisterFailed -= OnAuthFailed;
        }
        

        private void OnLoginSubmit()
        {
            if (_loginUsernameInput.text.IsNullOrEmpty())
            {
                OpenMessage("Введите свой никнейм", true);
            }
            
            if (_loginPasswordInput.text.IsNullOrEmpty())
            {
                OpenMessage("Введите свой пароль", true);
            }

            Singleton<AuthManager>.Instance.Login(
                _loginUsernameInput.text,
                _loginPasswordInput.text
            );
        }
        
        private void OnRegisterSubmit()
        {
            if (_registerUsernameInput.text.IsNullOrEmpty())
            {
                OpenMessage("Введите свой никнейм", true);
                return;
            }
            
            if (_registerEmailInput.text.IsNullOrEmpty())
            {
                OpenMessage("Введите свою почту", true);
                return;
            }
            
            if (_registerPasswordInput.text.IsNullOrEmpty())
            {
                OpenMessage("Введите свой пароль", true);
                return;
            }
            
            if (_registerPasswordRepeatInput.text.IsNullOrEmpty())
            {
                OpenMessage("Повторите свой пароль", true);
                return;
            }
            
            
            if (!_registerPasswordInput.text.Equals(_registerPasswordRepeatInput.text))
            {
                OpenMessage("Пароли не совпадают", true);
                return;
            }
            
            
            Singleton<AuthManager>.Instance.Register(
                _registerEmailInput.text,
                _registerUsernameInput.text,
                _registerPasswordInput.text
            );
        }


        private void OnAuthFailed(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            OpenMessage(errorMessage, true);
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Close();
        }
        
        private void OnLoginSuccess(LoginResult result)
        {
            Close();
        }
    }
}