using System;

namespace Code.Core.Abstractions.Managers
{
    public interface IPlayerInfoManager
    {
        event Action<string> OnNicknameChange;

        /// <summary>
        /// Изменяет никнейм игрока
        /// </summary>
        /// <param name="nickname">Новый никнейм</param>
        void SetNickname(string nickname);
    }
}