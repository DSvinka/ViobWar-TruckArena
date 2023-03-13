using System;
using PlayFab;
using PlayFab.ClientModels;

namespace Code.Core.Abstractions.Managers
{
    public interface IAuthManager
    {
        event Action OnLogout;
        
        event Action<LoginResult> OnLoginSuccess;
        event Action<PlayFabError> OnLoginFailed;
        
        event Action<RegisterPlayFabUserResult> OnRegisterSuccess;
        event Action<PlayFabError> OnRegisterFailed;
        
        static string Username { get; }
        static string PlayFabId { get; }
        
        /// <summary>
        /// Забывает аккаунт пользователя (выход из аккаунта)
        /// </summary>
        void Logout();

        /// <summary>
        /// Авторизует пользователя в Playfab по логину и паролю
        /// </summary>
        void Login(string username, string password);
        
        /// <summary>
        /// Авторизует пользователя в Playfab в качестве гостя
        /// </summary>
        void LoginAsGuest();

        /// <summary>
        /// Регистрирует пользователя в Playfab
        /// </summary>
        void Register(string username, string password, string email);
    }
}