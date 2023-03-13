using System;
using Code.Shared.Models.Menu;
using Photon.Realtime;

namespace Code.Core.Abstractions.Managers
{
    public interface IRoomManager
    {
        event Action<Room> OnRoomJoin;
        event Action OnRoomLeave;
        
        event Action<Player> OnPlayerJoin;
        event Action<Player> OnPlayerLeave;

        event Action<Player> OnMasterClientSwitch;
        
        /// <summary>
        /// Подключает игрока в комнату по коду комнаты.
        /// </summary>
        /// <remarks>
        /// Если клиент уже находится в комнате, то в консоль выйдет предупреждение и клиент отключится от старой комнаты и подключится к указанной.
        /// </remarks>
        /// <param name="code">Код комнаты</param>
        void JoinRoom(string code);

        /// <summary>
        /// Отключает игрока от комнаты 
        /// </summary>
        void LeaveRoom();

        /// <summary>
        /// Подключает игрока в случайную комнату
        /// </summary>
        /// <remarks>
        /// Если игрок уже находится в комнате, то выдаёт предупреждение и отключает от старой комнаты и подключает к новой.
        /// </remarks>
        void JoinRandomRoom();
        
        /// <summary>
        /// Создаёт комнату с указанными параметрами.
        /// </summary>
        /// <remarks>
        /// Если клиент уже находится в комнате, то в консоль выйдет предупреждение и клиент отключится от старой комнаты и подключится к созданной.
        /// </remarks>
        /// <param name="editRoomModel">Параметры комнаты</param>
        /// <returns>Код комнаты</returns>
        string CreateRoom(EditRoomModel editRoomModel);
        
        /// <summary>
        /// Редактирует текущую комнату
        /// </summary>
        /// <remarks>
        ///     <para>Если клиент не находится в комнате, то выведется ошибка в консоль.</para>
        ///     <para>Если клиент не владелец комнаты, то выведется ошибка в консоль.</para>
        /// </remarks>
        /// <param name="editRoomModel">Параметры комнаты, укажите null если не хотите редактировать параметры комнаты</param>
        /// <param name="isOpen">Открыта ли комната</param>
        void EditRoom(EditRoomModel editRoomModel = null, bool isOpen = true);
    }
}