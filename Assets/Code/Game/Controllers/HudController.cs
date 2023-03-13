using System.Collections;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Game.Abstractions.Controller;
using Code.Game.Data;
using Code.Game.Views;
using Code.Game.Views.Hud;
using Code.Shared.Enums;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Code.Game.Controllers
{
    public class HudController: IInitialization, ICleanup, IExecute
    {
        private readonly GameStatsController _gameStatsController;
        private readonly PlayerController _playerController;
        private readonly HudView _hudView;

        private bool _windowOpened;
        private GameSettingsData _gameSettingsData;

        public HudController(
            PlayerController playerController, GameStatsController gameStatsController, HudView hudView,
            GameSettingsData gameSettingsData
        )
        {
            _gameStatsController = gameStatsController;
            _gameSettingsData = gameSettingsData;
            
            _playerController = playerController;
            _playerController.OnPlayerDeath += OnPlayerDeath;
            
            _hudView = hudView;
            _hudView.OnLeaveRoomSubmit += OnLeaveRoomSubmit;
        }

        private void OnPlayerDeath()
        {
            if (_windowOpened)
                return;
            
            var model = Singleton<PlayerStatisticsManager>.Instance.StatisticsModel;
            model.LossesCount.Value += model.LossesCount.Value == -1 ? 2 : 1;
            Singleton<PlayerStatisticsManager>.Instance.UpdateStatistics(model.LossesCount);

            _windowOpened = true;
            _hudView.OpenLoseWindow();
        }

        private void OnLeaveRoomSubmit()
        {
            _hudView.StartCoroutine(WaitForDisconnect());
        }
        
        private IEnumerator WaitForDisconnect()
        {
            Singleton<RoomManager>.Instance.LeaveRoom();
            Singleton<ConnectionManager>.Instance.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return 0;
            SceneManager.LoadScene((int) EScenesIndexes.Menu);
        }


        public void Init(bool isMaster)
        {
            _playerController.OnHealthChange += OnHealthChange;
            _playerController.OnAimToEnemy += OnAimToEnemy;

            var vehicleModel = _playerController.GetVehicleModel();
            OnHealthChange(vehicleModel.Health);
        }
        
        public void Cleanup(bool isMaster)
        {
            _playerController.OnHealthChange -= OnHealthChange;
            _playerController.OnAimToEnemy -= OnAimToEnemy;
        }
        

        private void OnHealthChange(float health)
        {
            _hudView.SetPlayerHealth(health);
        }

        private void OnAimToEnemy(VehicleView enemyVehicleView, bool enemyFounded)
        {
            if (enemyFounded)
            {
                _hudView.SetEnemyHealth(true, enemyVehicleView.DisplayHealth);
            }
            else
            {
                _hudView.SetEnemyHealth(false, 0);
            }
        }

        public void Execute(bool isMaster)
        {
            if (_windowOpened)
                return;
            
            if (_gameStatsController.GameStatsView.AlivePlayersCount <= 1)
            {
                if (PhotonNetwork.PlayerList[0].UserId != PhotonNetwork.LocalPlayer.UserId)
                    return;
                
                _windowOpened = true;  
                _hudView.OpenWinWindow();
                
                Singleton<PlayerCurrencyManager>.Instance.PlayerCurrenciesModel.MoneyModel.AddBalance(
                    _gameSettingsData.WinMoneyAwardCount    
                );
                
                var model = Singleton<PlayerStatisticsManager>.Instance.StatisticsModel;
                model.WinsCount.Value += model.WinsCount.Value == -1 ? 2 : 1;;
                Singleton<PlayerStatisticsManager>.Instance.UpdateStatistics(model.WinsCount);
            }
        }
    }
}