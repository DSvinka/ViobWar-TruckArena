using System;
using System.Collections.Generic;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Shared.Models;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Core.Managers
{
    public class PlayerCurrencyManager: MonoBehaviour, ISingleton
    {
        private void Start()
        {
            _playerCurrenciesModel = new PlayerCurrenciesModel(this);
        }

        private void Update()
        {
            if (!_requestSended && _addPlayerCurrencyRequests.Count != 0)
            {
                var request = _addPlayerCurrencyRequests.Dequeue();
                AddPlayerCurrency(request.VirtualCurrency, request.Amount);
            }
        }

        #region GetPlayerCurrencies

        public event Action<GetUserInventoryResult> OnGetPlayerInventorySuccess;
        public event Action<PlayFabError> OnGetPlayerInventoryFailed;
        
        private PlayerCurrenciesModel _playerCurrenciesModel;
        public PlayerCurrenciesModel PlayerCurrenciesModel => _playerCurrenciesModel;

        public void GetPlayerInventory()
        {
            var request = new GetUserInventoryRequest(); 
            
            PlayFabClientAPI.GetUserInventory(request, OnGetPlayerInventory, OnGetPlayerInventoryError);
        }
        
        private void OnGetPlayerInventory(GetUserInventoryResult result)
        {
            DLogger.Debug(GetType(), nameof(OnGetPlayerInventory), 
                $"Complete!");
            OnGetPlayerInventorySuccess?.Invoke(result);
            
            _playerCurrenciesModel.ReadDictionary(result.VirtualCurrency);
        }

        private void OnGetPlayerInventoryError(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnGetPlayerInventoryError), 
                $"Failed: {errorMessage}");
            OnGetPlayerInventoryFailed?.Invoke(error);
        }

        #endregion
        
        #region AddPlayerCurrency
        
        public event Action<ModifyUserVirtualCurrencyResult> OnAddPlayerCurrencySuccess;
        public event Action<PlayFabError> OnAddPlayerCurrencyFailed;

        private bool _requestSended; 
        private Queue<AddUserVirtualCurrencyRequest> _addPlayerCurrencyRequests = new ();

        public void AddPlayerCurrency(string key, int value)
        {
            var getAccountInfoRequest = new AddUserVirtualCurrencyRequest()
            {
                VirtualCurrency = key,
                Amount = value
            };

            if (_requestSended)
            {
                _addPlayerCurrencyRequests.Enqueue(getAccountInfoRequest);    
            }
            
            PlayFabClientAPI.AddUserVirtualCurrency(getAccountInfoRequest, OnAddPlayerCurrency, OnAddPlayerCurrencyError);
            _requestSended = true;
        }

        private void OnAddPlayerCurrency(ModifyUserVirtualCurrencyResult result)
        {
            DLogger.Debug(GetType(), nameof(OnAddPlayerCurrency), 
                $"Complete! [Currency={result.VirtualCurrency}, Balance={result.Balance}]");
            _requestSended = false;
            OnAddPlayerCurrencySuccess?.Invoke(result);
        }
        
        private void OnAddPlayerCurrencyError(PlayFabError playFabError)
        {
            var errorMessage = playFabError.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnAddPlayerCurrencyError), 
                $"Failed: {errorMessage}");
            _requestSended = false;
            OnAddPlayerCurrencyFailed?.Invoke(playFabError);
        }

        #endregion
    }
}