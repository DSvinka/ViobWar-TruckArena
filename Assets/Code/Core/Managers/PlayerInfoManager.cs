using System;
using Code.Core.Singleton;
using Code.Core.Utils;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;

namespace Code.Core.Managers
{
    public class PlayerInfoManager: MonoBehaviourPunCallbacks, ISingleton
    {
        #region Public Events

        public event Action<string> OnNicknameChange;
        
        public event Action<GetAccountInfoResult> OnGetAccountInfoSuccess;
        public event Action<PlayFabError> OnGetAccountInfoFailed;

        #endregion

        private UserAccountInfo _accountInfo;
        public UserAccountInfo AccountInfo => _accountInfo;
        
        
        public void SetNickname(string nickname)
        {
            PhotonNetwork.NickName = nickname;
            DLogger.Debug(GetType(), nameof(SetNickname), 
                $"Nickname changed to {nickname}");

            OnNicknameChange?.Invoke(nickname);
        }
        
        
        #region AccountInfo

        public void GetAccountInfo(string playFabId)
        {
            var getAccountInfoRequest = new GetAccountInfoRequest()
            {
                PlayFabId = playFabId,
            };
            
            PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, OnGetAccountInfo, OnGetAccountInfoError);
        }

        private void OnGetAccountInfo(GetAccountInfoResult result)
        {
            DLogger.Debug(GetType(), nameof(OnGetAccountInfo), 
                $"Complete! [Username={result.AccountInfo.Username}]");
            PhotonNetwork.NickName = result.AccountInfo.Username;
            OnGetAccountInfoSuccess?.Invoke(result);
        }
        
        private void OnGetAccountInfoError(PlayFabError playFabError)
        {
            var errorMessage = playFabError.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnGetAccountInfoError), 
                $"Failed: {errorMessage}");
            OnGetAccountInfoFailed?.Invoke(playFabError);
        }

        #endregion
    }
}