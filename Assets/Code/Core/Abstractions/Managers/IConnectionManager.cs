using System;
using Photon.Realtime;

namespace Code.Core.Abstractions.Managers
{
    public interface IConnectionManager
    {
        public event Action OnConnect;
        public event Action<DisconnectCause> OnDisconnect;

        bool IsConnected { get; }

        /// <summary>
        /// Если подключение к серверам отсутствует, подключает пользователя к серверам Photon, используя настройки из файла PhotonServerSettings
        /// </summary>
        void Connect();

        /// <summary>
        /// Если подключение к серверам присутствует, отключает пользователя от серверов Photon
        /// </summary>
        void Disconnect();
    }
}