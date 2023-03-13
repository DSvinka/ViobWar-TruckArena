using System;
using System.Linq;
using Cinemachine;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Game.Abstractions.Controller;
using Code.Game.Data;
using Code.Game.Data.Vehicles;
using Code.Game.Models;
using Code.Game.Views;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Game.Controllers
{
    public class PlayerController: IInitialization, IExecute, ICleanup
    {
        public event Action OnPlayerDeath;
        public event Action<VehicleModel> OnVehicleChange;
        public event Action<VehicleView, bool> OnAimToEnemy;
        public event Action<float> OnHealthChange;
        
        private readonly GameSettingsData _gameSettingsData;
        private readonly PlayerNetworkData _playerNetworkData;
        private readonly PlayerKeymapData _playerKeymapData;

        private readonly VehicleController _vehicleController;
        private readonly TurretsController _turretsController;
        private readonly GameStatsController _gameStatsController;

        private readonly CinemachineFreeLook _cinemachineFreeLook;

        private readonly GameStatsView _gameStatsView;

        private VehicleModel _vehicleModel;

        public PlayerController(
            VehicleController vehicleController, TurretsController turretsController, GameStatsView gameStatsView,
            GameStatsController gameStatsController, CinemachineFreeLook cinemachineFreeLook,
            PlayerNetworkData playerNetworkData, PlayerKeymapData playerKeymapData, GameSettingsData gameSettingsData)
        {
            _gameSettingsData = gameSettingsData;
            _playerNetworkData = playerNetworkData;
            _playerKeymapData = playerKeymapData;

            _vehicleController = vehicleController;
            _turretsController = turretsController;
            _gameStatsController = gameStatsController;

            _cinemachineFreeLook = cinemachineFreeLook;
            _gameStatsView = gameStatsView;
        }
        
        public void Init(bool isMaster)
        {
            var freeSpawner = _gameStatsView.FreeSpawners[Random.Range(0, _gameStatsView.FreeSpawners.Count-1)];
            _gameStatsView.FreeSpawners.Remove(freeSpawner);
            _vehicleModel = _vehicleController.Spawn(freeSpawner.position, _gameSettingsData.DebugDefaultVehicle);
            
            OnVehicleChange?.Invoke(_vehicleModel);
            OnHealthChange?.Invoke(_vehicleModel.Health);

            _cinemachineFreeLook.LookAt = _vehicleModel.VehicleView.CameraLook;
            _cinemachineFreeLook.Follow = _vehicleModel.VehicleView.CameraLook;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _vehicleModel.VehicleView.OnSetTurret += OnSetTurret;
            _vehicleModel.VehicleView.OnAddDamage += OnAddDamage;
            _vehicleModel.VehicleView.OnAddHealth += OnAddHealth;
            _vehicleModel.VehicleView.OnDamagedEnemyDeath += DamagedEnemyDeath;
        }

        private void DamagedEnemyDeath(VehicleView enemy)
        {
            var model = Singleton<PlayerStatisticsManager>.Instance.StatisticsModel;
            model.KillsCount.Value += model.KillsCount.Value == -1 ? 2 : 1;
            Singleton<PlayerStatisticsManager>.Instance.UpdateStatistics(model.KillsCount);
            
            Singleton<PlayerCurrencyManager>.Instance.PlayerCurrenciesModel.MoneyModel.AddBalance(
                _gameSettingsData.KillMoneyAwardCount    
            );
        }

        public void Cleanup(bool isMaster)
        {
            if (_vehicleModel == null)
                return;
            
            _vehicleModel.VehicleView.OnSetTurret -= OnSetTurret;
            _vehicleModel.VehicleView.OnAddDamage -= OnAddDamage;
            _vehicleModel.VehicleView.OnAddHealth -= OnAddHealth;
            _vehicleModel.VehicleView.OnDamagedEnemyDeath -= DamagedEnemyDeath;
        }
        
        private void OnAddHealth(VehicleView vehicleView, float health)
        {
            if (_vehicleModel == null)
                return;
            
            _vehicleModel.Health += health;
            _vehicleModel.VehicleView.SetDisplayHealth(health);
            OnHealthChange?.Invoke(_vehicleModel.Health);
        }

        private void OnAddDamage(VehicleView vehicleView, VehicleView enemyVehicleView, float damage)
        {
            if (_vehicleModel == null)
                return;
            
            _vehicleModel.Health -= damage;

            vehicleView.AudioSource.PlayOneShot(_vehicleModel.VehicleData.DamageSound);
            
            if (_vehicleModel.Health <= 0)
            {
                vehicleView.AudioSource.PlayOneShot(_vehicleModel.VehicleData.ExplosionSound);
                
                DLogger.Debug(GetType(), nameof(OnAddDamage), "Машина Уничтожена!");
                
                var model = Singleton<PlayerStatisticsManager>.Instance.StatisticsModel;
                model.DeathsCount.Value += model.DeathsCount.Value == -1 ? 2 : 1;
                Singleton<PlayerStatisticsManager>.Instance.UpdateStatistics(model.DeathsCount);
                
                Cleanup(false);
                _vehicleModel = null;
                PhotonNetwork.Destroy(vehicleView.gameObject);

                _gameStatsController.GameStatsView.AlivePlayersCount -= 1;
                
                OnPlayerDeath?.Invoke();

                return;
            }
            
            _vehicleModel.VehicleView.SetDisplayHealth(_vehicleModel.Health);
            OnHealthChange?.Invoke(_vehicleModel.Health);
        }

        private void OnSetTurret(VehicleView vehicleView, TurretData turretData, VehicleTurretRack vehicleTurretRack)
        {
            if (vehicleTurretRack != null)
            {
                _turretsController.Install(_vehicleModel, vehicleTurretRack, turretData);
            }
            else
            {
                Singleton<PlayerCurrencyManager>.Instance.PlayerCurrenciesModel.MoneyModel.AddBalance(
                    _gameSettingsData.ItemPickupMoneyAwardCount    
                );
                
                foreach (var turret in _vehicleModel.Turrets)
                {
                    if (turret.Value == null)
                    {
                        _turretsController.Install(_vehicleModel, turret.Key, turretData);
                        return;
                    }
                }

                var randomRack = _vehicleModel.Turrets.Keys.ToArray()[Random.Range(0, _vehicleModel.Turrets.Keys.Count - 1)];
                _turretsController.Remove(_vehicleModel, randomRack);
                _turretsController.Install(_vehicleModel, randomRack, turretData);
            }
        }
        
        public void Execute(bool isMaster)
        {
            if (_vehicleModel == null)
                return;

            UpdateMovementInput();
            UpdateLookingInput();
            UpdateShootingInput();

            if (Input.GetKeyDown(_playerKeymapData.RotateTruckOnStuck))
            {
                _vehicleModel.VehicleView.transform.rotation = Quaternion.identity;
            }

            _turretsController.UpdateShootDelay(_vehicleModel);
        }

        private void UpdateShootingInput()
        {
            if (Input.GetKey(_playerKeymapData.ShootingKey))
            {
                _turretsController.Shoot(_vehicleModel);
            }
        }

        private void UpdateMovementInput()
        {
            _vehicleController.UpdateVehicleMovementInput(
                _vehicleModel,
                Input.GetAxis(_playerKeymapData.VerticalAxis),
                Input.GetAxis(_playerKeymapData.HorizontalAxis),
                false
            );
        }

        private void UpdateLookingInput()
        {
            _turretsController.UpdateVehicleTurretRotationInput(
                _vehicleModel, Camera.main.transform 
            );

            var raycastResult = Physics.RaycastAll(
                Camera.main.transform.position, Camera.main.transform.forward,
                _gameSettingsData.UnitAimingRange, _gameSettingsData.VehicleLayerMask
            );
            if (
                raycastResult.Length != 0 && 
                raycastResult[0].collider.gameObject.GetInstanceID() != _vehicleModel.VehicleView.gameObject.GetInstanceID())
            {
                OnAimToEnemy?.Invoke(raycastResult[0].collider.GetComponent<VehicleView>(), true);
            }
            else
            {
                OnAimToEnemy?.Invoke(null, false);
            }
        }

        public VehicleModel GetVehicleModel()
        {
            return _vehicleModel;
        }
    }
}