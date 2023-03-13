using System;
using Code.Core.Abstractions.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Shared.Constants;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Core.Managers
{
    public class AuthManager : MonoBehaviour, IAuthManager, ISingleton
    {
        #region Public Events

        public event Action OnLogout;
        
        public event Action<LoginResult> OnLoginSuccess;
        public event Action<PlayFabError> OnLoginFailed;

        public event Action<RegisterPlayFabUserResult> OnRegisterSuccess;
        public event Action<PlayFabError> OnRegisterFailed;

        #endregion

        private string _playFabId;
        private string _username;

        public string PlayFabId => _playFabId;
        public string Username => _username;

        

        public void Logout()
        {
            PlayFabClientAPI.ForgetAllCredentials();

            _playFabId = null;
            _username = null;
            
            DLogger.Debug(GetType(), nameof(Logout), 
                "Logout Complete!");
            OnLogout?.Invoke();
        }

        #region Authentication

        public void Login(string username, string password)
        {
            _username = username;
            
            var registerRequest = new LoginWithPlayFabRequest()
            {
                Username = username,
                Password = password,
            };
            
            PlayFabClientAPI.LoginWithPlayFab(registerRequest, OnLogin, OnLoginError);
        }

        public void LoginAsGuest()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = PlayfabConstants.DefaultTitleId;
            }
            
            var needCreation = PlayerPrefs.HasKey(PlayerPrefsKeys.AuthGuestGuidKey);
            var id = PlayerPrefs.GetString(PlayerPrefsKeys.AuthGuestGuidKey, Guid.NewGuid().ToString());

            _username = id;
            
            var loginRequest = new LoginWithCustomIDRequest()
            {
                CustomId = id,
                CreateAccount = !needCreation
            };
            
            PlayFabClientAPI.LoginWithCustomID(loginRequest,
                success =>
                {
                    PlayerPrefs.SetString(PlayerPrefsKeys.AuthGuestGuidKey, id);
                    OnLogin(success);
                }, OnLoginError);
        }
        
        private void OnLogin(LoginResult loginResult)
        {
            DLogger.Debug(GetType(), nameof(OnLogin), 
                "Complete!");
            _playFabId = loginResult.PlayFabId;
            Singleton<PlayerInfoManager>.Instance.GetAccountInfo(_playFabId);
            Singleton<PlayerCurrencyManager>.Instance.GetPlayerInventory();
            OnLoginSuccess?.Invoke(loginResult);
        }

        private void OnLoginError(PlayFabError playFabError)
        {
            var errorMessage = playFabError.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnLoginError), 
                $"Failed: {errorMessage}");
            OnLoginFailed?.Invoke(playFabError);
        }

        #endregion
        
        
        #region Registration

        public void Register(string email, string username, string password)
        {
            _username = username;

            var registerRequest = new RegisterPlayFabUserRequest()
            {
                Email = email,
                Username = username, 
                Password = password,
            };
            
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegister, OnRegisterError);
        }

        private void OnRegister(RegisterPlayFabUserResult registerResult)
        {
            DLogger.Debug(GetType(), nameof(OnRegister), 
                $"Register Complete! [Username={registerResult.Username}]");
            PhotonNetwork.NickName = registerResult.Username;
            _playFabId = registerResult.PlayFabId;
            OnRegisterSuccess?.Invoke(registerResult);
        }
        
        private void OnRegisterError(PlayFabError playFabError)
        {
            var errorMessage = playFabError.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnRegisterError), 
                $"Register Failed: {errorMessage}");
            OnRegisterFailed?.Invoke(playFabError);
        }

        #endregion
    }
}
