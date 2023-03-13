using System.Collections.Generic;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Shared.Constants;
using PlayFab.ClientModels;

namespace Code.Shared.Models
{
    public class PlayerCurrenciesModel
    {
        public readonly CurrencyModel MoneyModel = new (PlayerCurrencyKeys.MoneyCurrency);

        private Dictionary<string, CurrencyModel> _models;
        public IReadOnlyDictionary<string, CurrencyModel> Models => _models;

        private PlayerCurrencyManager _manager;

        public PlayerCurrenciesModel(PlayerCurrencyManager manager)
        {
            _models = new Dictionary<string, CurrencyModel>()
            {
                { MoneyModel.CurrencyKey, MoneyModel }
            };

            MoneyModel.OnBalanceAdd += OnBalanceAdd;
            _manager = manager;
            
            manager.OnAddPlayerCurrencySuccess += OnAddPlayerCurrencySuccess;
        }
        
        public void ReadDictionary(Dictionary<string, int> data)
        {
            foreach (var (currencyKey, balance) in data)
            {
                _models[currencyKey].SetBalance(balance);
            }
        }
        
        private void OnBalanceAdd(string currencyKey, int addedBalance)
        {
            Singleton<PlayerCurrencyManager>.Instance.AddPlayerCurrency(currencyKey, addedBalance);
        }

        private void OnAddPlayerCurrencySuccess(ModifyUserVirtualCurrencyResult result)
        {
            _models[result.VirtualCurrency].SetBalance(result.Balance);
        }
    }
}