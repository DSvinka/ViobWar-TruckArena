using System;

namespace Code.Shared.Models
{
    public class CurrencyModel
    {
        public event Action<string, int> OnBalanceAdd;
        
        private string _currencyKey;
        private int _balance;

        public string CurrencyKey => _currencyKey;
        public int Balance => _balance;

        public CurrencyModel(string currencyKey)
        {
            _currencyKey = currencyKey;
        }

        public void AddBalance(int count)
        {
            OnBalanceAdd?.Invoke(_currencyKey, count);
        }

        internal void SetBalance(int newBalance)
        {
            _balance = newBalance;
        }
    }
}