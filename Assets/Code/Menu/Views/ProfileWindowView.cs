using System.Collections.Generic;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Menu.Abstractions;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Menu.Views
{
    [RequireComponent(typeof(AuthWindowView))]
    public class ProfileWindowView: BaseWindowView, IUpdateWindow
    {
        [SerializeField, Space] private string _roundsCountLabelName = "RoundsCount";
        [SerializeField] private string _winsRoundsCountLabelName = "WinRoundsCount";
        [SerializeField] private string _lossesRoundsCountLabelName = "LoseRoundsCount";
        
        [SerializeField, Space] private string _killsCountLabelName = "KillsCount";
        [SerializeField] private string _deathsCountLabelName = "DeathsCount";

        [SerializeField, Space] private string _moneyCountLabelName = "MoneyCount";


        private Label _roundsCountLabel;
        private Label _winsRoundsCountLabel;
        private Label _lossesRoundsCountLabel;
        
        private Label _killsCountLabel;
        private Label _deathsCountLabel;

        private Label _moneyCountLabel;
        
        
        protected override void Start()
        {
            base.Start();

            _roundsCountLabel = Window.Q<Label>(_roundsCountLabelName);
            _winsRoundsCountLabel = Window.Q<Label>(_winsRoundsCountLabelName);
            _lossesRoundsCountLabel = Window.Q<Label>(_lossesRoundsCountLabelName);
            
            _killsCountLabel = Window.Q<Label>(_killsCountLabelName);
            _deathsCountLabel = Window.Q<Label>(_deathsCountLabelName);

            _moneyCountLabel = Window.Q<Label>(_moneyCountLabelName);
            
            Singleton<PlayerStatisticsManager>.Instance.OnGetStatistics += OnGetStatistics;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Singleton<PlayerStatisticsManager>.Instance.OnGetStatistics -= OnGetStatistics;
        }
        

        public void OnGetStatistics(List<StatisticValue> statisticValues)
        {
            var model = Singleton<PlayerStatisticsManager>.Instance.StatisticsModel.ToDisplayValues();

            _roundsCountLabel.text = $"{model.WinsCount.Value + model.LossesCount.Value}";
            _winsRoundsCountLabel.text = $"{model.WinsCount.Value}";
            _lossesRoundsCountLabel.text = $"{model.LossesCount.Value}";
            
            _killsCountLabel.text = $"{model.KillsCount.Value}" ;
            _deathsCountLabel.text = $"{model.DeathsCount.Value}";
        }
        

        public override void Open()
        {
            base.Open();
            UpdateWindow();
        }

        public void UpdateWindow()
        {
            if (!WindowWrapper.Contains(Window))
                return;
            
            if (Singleton<AuthManager>.Instance.PlayFabId == null)
            {
                Close();
                
                var authWindowView = GetComponent<AuthWindowView>();
                authWindowView.Open();
                return;
            }
            
            Singleton<PlayerStatisticsManager>.Instance.GetStatistics();

            _moneyCountLabel.text = Singleton<PlayerCurrencyManager>.Instance.PlayerCurrenciesModel.MoneyModel.Balance.ToString();
        }
    }
}