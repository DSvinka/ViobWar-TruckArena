using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Game.Abstractions.Controller;
using Code.Game.Data;
using Code.Game.Data.Vehicles;
using Code.Game.Models;
using Code.Game.Views;
using Code.Shared.Constants;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Game.Controllers
{
    public class AiController: IExecute, IInitialization, ICleanup
    {
        private readonly VehicleController _vehicleController;
        private readonly TurretsController _turretsController;
        private readonly GameStatsController _gameStatsController; 

        private readonly VehiclesData _vehiclesData;
        private readonly AiSettingsData _aiSettingsData;
        private readonly GameSettingsData _gameSettingsData;
        private readonly Dictionary<VehicleView, AiModel> _agents;
        
        private readonly GameStatsView _gameStatsView;

        private List<Transform> _spawners;

        public IDictionary<VehicleView, AiModel> Agents => _agents;

        public AiController(VehicleController vehicleController, TurretsController turretsController,
            GameStatsController gameStatsController, GameStatsView gameStatsView,
            AiSettingsData aiSettingsData, VehiclesData vehiclesData, GameSettingsData gameSettingsData)
        {
            _turretsController = turretsController;
            _vehicleController = vehicleController;
            _gameStatsController = gameStatsController;

            _vehiclesData = vehiclesData;
            _aiSettingsData = aiSettingsData;
            _gameSettingsData = gameSettingsData;

            _gameStatsView = gameStatsView;

            _agents = new Dictionary<VehicleView, AiModel>();
            _spawners = new List<Transform>();

            var botsCount = (byte) Singleton<RoomManager>.Instance.CurrentRoom.CustomProperties[PhotonRoomKeys.RoomBotsCountKey];
            if (botsCount <= 0)
                return;
            
            for (var i = 0; i < botsCount; i++)
            {
                var freeSpawner = _gameStatsView.FreeSpawners[i];
                _gameStatsView.FreeSpawners.Remove(freeSpawner);
                _spawners.Add(freeSpawner);
            }
        }
        
        public void Init(bool isMaster)
        {
            if (!isMaster)
                return;

            var botsCount = (byte) Singleton<RoomManager>.Instance.CurrentRoom.CustomProperties[PhotonRoomKeys.RoomBotsCountKey];
            if (botsCount <= 0)
                return;
                
            for (var i = 0; i < botsCount; i++)
            {
                var freeSpawner = _spawners[i];
                Spawn(_vehiclesData.Vehicles[Random.Range(0, _vehiclesData.Vehicles.Length - 1)], freeSpawner.position);   
            }
        }

        public void Execute(bool isMaster)
        {
            if (!isMaster)
                return;
            
            if (Time.frameCount % _aiSettingsData.FrameCountToSearch != 0)
                return;
            
            SearchTargets();
            UpdateMovement();
            UpdateTurrets();
        }
        
        public void Cleanup(bool isMaster)
        {
            if (!isMaster)
                return;

            foreach (var (vehicleView, aiModel) in _agents)
            {
                vehicleView.OnSetTurret -= OnSetTurret;
                vehicleView.OnAddDamage -= OnAddDamage;
                vehicleView.OnAddHealth -= OnAddHealth;   
            }
        }

        public void Spawn(VehicleData vehicleData, Vector3 spawnPosition)
        {
            var vehicleModel = _vehicleController.Spawn(spawnPosition, vehicleData);

            var aiObject = PhotonNetwork.Instantiate(_aiSettingsData.AiPrefabPath, vehicleModel.VehicleView.CameraLook.position, Quaternion.identity);
            var aiView = aiObject.GetComponent<AiView>();
            
            aiObject.transform.parent = vehicleModel.VehicleView.CameraLook;
            aiObject.transform.position = vehicleModel.VehicleView.CameraLook.transform.position;
            aiView.RadarObject.position = vehicleModel.VehicleView.CameraLook.transform.position;
            
            var aiModel = new AiModel(aiView)
            {
                VehicleModel = vehicleModel
            };
            
            vehicleModel.VehicleView.OnSetTurret += OnSetTurret;
            vehicleModel.VehicleView.OnAddDamage += OnAddDamage;
            vehicleModel.VehicleView.OnAddHealth += OnAddHealth;
            
            _agents.Add(vehicleModel.VehicleView, aiModel);
        }

        public void Destroy(VehicleView vehicleView)
        {
            var aiView = _agents[vehicleView].AiView;
            
            vehicleView.OnSetTurret -= OnSetTurret;
            vehicleView.OnAddDamage -= OnAddDamage;
            vehicleView.OnAddHealth -= OnAddHealth;
            
            PhotonNetwork.Destroy(aiView.gameObject);
            PhotonNetwork.Destroy(vehicleView.gameObject);

            _agents.Remove(vehicleView);
        }

        private void OnAddHealth(VehicleView vehicleView, float health)
        {
            var vehicleModel = _agents[vehicleView].VehicleModel;
            vehicleModel.Health += health;
            vehicleView.SetDisplayHealth(health);
        }

        private void OnAddDamage(VehicleView vehicleView, VehicleView enemyVehicleView, float damage)
        {
            var vehicleModel = _agents[vehicleView].VehicleModel;
            
            vehicleModel.Health -= damage;
            vehicleView.AudioSource.PlayOneShot(vehicleModel.VehicleData.DamageSound);
            if (_agents[vehicleView].VehicleModel.Health <= 0)
            {
                vehicleView.AudioSource.PlayOneShot(vehicleModel.VehicleData.ExplosionSound);
                DLogger.Debug(GetType(), nameof(OnAddDamage), "Машина Бота Уничтожена!");
                
                if (enemyVehicleView != null)
                    enemyVehicleView.DamagedEnemyDeath(vehicleView);
                
                Destroy(vehicleView);
                _gameStatsController.GameStatsView.AlivePlayersCount -= 1;
                
                return;
            }
            
            vehicleView.SetDisplayHealth(vehicleModel.Health);
        }

        private void OnSetTurret(VehicleView vehicleView, TurretData turretData, VehicleTurretRack vehicleTurretRack)
        {
            var vehicleModel = _agents[vehicleView].VehicleModel;
            
            if (vehicleTurretRack != null)
            {
                _turretsController.Install(vehicleModel, vehicleTurretRack, turretData);
            }
            else
            {
                foreach (var turret in vehicleModel.Turrets)
                {
                    if (turret.Value == null)
                    {
                        _turretsController.Install(vehicleModel, turret.Key, turretData);
                        return;
                    }
                }

                var randomRack = vehicleModel.Turrets.Keys.ToArray()[Random.Range(0, vehicleModel.Turrets.Keys.Count - 1)];
                _turretsController.Remove(vehicleModel, randomRack);
                _turretsController.Install(vehicleModel, randomRack, turretData);
            }
        }

        public void SearchTargets()
        {
            foreach (var (vehicleView, aiModel) in _agents)
            {
                var vehicleTransform = vehicleView.transform;

                if (aiModel.VehicleModel.Turrets.Values.Any(s => s != null))
                {
                    var sphereCastResult = Physics.SphereCastAll(
                        vehicleTransform.position, _aiSettingsData.SearchDistance,
                        vehicleTransform.forward, _aiSettingsData.SearchDistance,
                        _gameSettingsData.VehicleLayerMask
                    );

                    aiModel.Target = null;
                    if (sphereCastResult.Length != 0)
                    {
                        if (sphereCastResult.Length != 0)
                        {
                            Transform target = null;
                            var targetDistance = -1f;

                            foreach (var cast in sphereCastResult)
                            {
                                if (cast.collider.gameObject.GetInstanceID() ==
                                    aiModel.VehicleModel.VehicleView.gameObject.GetInstanceID())
                                    continue;

                                if (targetDistance <= 0)
                                {
                                    target = cast.transform;
                                    targetDistance = Vector3.Distance(vehicleView.transform.position, target.position);
                                    continue;
                                }

                                var castDistance = Vector3.Distance(vehicleView.transform.position,
                                    cast.transform.position);
                                if (targetDistance > castDistance)
                                {
                                    target = cast.transform;
                                    targetDistance = castDistance;
                                }

                                break;
                            }
                            
                            if (targetDistance >= 0)
                            {
                                aiModel.AiView.Agent.SetDestination(target.position);
                                aiModel.Target = target;
                            }
                        }
                    }
                }
                else
                {
                    var sphereCastResult = Physics.SphereCastAll(
                        vehicleTransform.position, _aiSettingsData.SearchDistance,
                        vehicleTransform.forward, _aiSettingsData.SearchDistance,
                        _gameSettingsData.PickupItemLayerMask
                    );

                    aiModel.Target = null;

                    if (sphereCastResult.Length != 0)
                    {
                        Transform target = null;
                        var targetDistance = -1f;
                        
                        foreach (var cast in sphereCastResult)
                        {
                            if (targetDistance < 0)
                            {
                                target = cast.transform;
                                targetDistance = Vector3.Distance(vehicleView.transform.position, cast.transform.position);
                                continue;
                            }

                            var castDistance = Vector3.Distance(vehicleView.transform.position, cast.transform.position);
                            if (targetDistance > castDistance)
                            {
                                target = cast.transform;
                                targetDistance = castDistance;
                            }
                            break;
                        }
                    
                        if (targetDistance >= 0)
                        {
                            aiModel.AiView.Agent.SetDestination(target.position);
                            aiModel.Target = target;
                        }
                    }
                }
            }
        }

        public void UpdateMovement()
        {
            foreach (var (vehicleView, aiModel) in Agents)
            {
                if (aiModel.Target == null)
                    continue;
                        
                var targetPosition = aiModel.Target.position;

                float forwardAmount = 0f;
                float turnAmount = 0f;

                float reachedTargetDistance = 5f;
                float distanceToTarget = Vector3.Distance(vehicleView.transform.position, targetPosition);
                if (distanceToTarget > reachedTargetDistance) {
                    // Still too far, keep going
                    Vector3 dirToMovePosition = (targetPosition - vehicleView.transform.position).normalized;
                    float dot = Vector3.Dot(vehicleView.transform.forward, dirToMovePosition);

                    if (dot > 0) {
                        // Target in front
                        forwardAmount = 1f;

                        float stoppingDistance = 10f;
                        float stoppingSpeed = 20f;
                        
                        if (distanceToTarget < stoppingDistance && aiModel.VehicleModel.Speed > stoppingSpeed) {
                            // Within stopping distance and moving forward too fast
                            forwardAmount = -1f;
                        }
                    } else {
                        // Target behind
                        float reverseDistance = 15f;
                        if (distanceToTarget > reverseDistance) {
                            // Too far to reverse
                            forwardAmount = 1f;
                        } else {
                            forwardAmount = -1f;
                        }
                    }

                    float angleToDir = Vector3.SignedAngle(vehicleView.transform.forward, dirToMovePosition, Vector3.up);

                    if (angleToDir > 0) {
                        turnAmount = 0.5f;
                    } else {
                        turnAmount = -0.5f;
                    }
                } else {
                    // Reached target
                    if (aiModel.VehicleModel.Speed > aiModel.VehicleModel.VehicleData.SpeedMax) {
                        forwardAmount = -1f;
                    } else {
                        forwardAmount = 0f;
                    }
                    turnAmount = 0f;
                }
                
                _vehicleController.UpdateVehicleMovementInput(aiModel.VehicleModel, forwardAmount, turnAmount, true);
            }
        }
        
        public void UpdateTurrets()
        {
            foreach (var (vehicleView, aiModel) in Agents)
            {
                _turretsController.UpdateShootDelay(aiModel.VehicleModel);
                
                if (aiModel.Target != null)
                {
                    aiModel.AiView.transform.position = aiModel.VehicleModel.VehicleView.CameraLook.transform.position;
                    aiModel.AiView.RadarObject.position = aiModel.VehicleModel.VehicleView.CameraLook.transform.position;
                    
                    aiModel.AiView.RadarObject.LookAt(aiModel.Target);
                    _turretsController.UpdateVehicleTurretRotationInput(aiModel.VehicleModel, aiModel.AiView.RadarObject);

                    var direction = aiModel.Target.position - aiModel.AiView.RadarObject.position;
#if UNITY_EDITOR
                    Debug.DrawRay(aiModel.AiView.RadarObject.position, direction, Color.red);
#endif
                    
                    if (Physics.Raycast(aiModel.AiView.RadarObject.position, direction, 
                            _aiSettingsData.SearchDistance, _gameSettingsData.VehicleLayerMask))
                    {
                        _turretsController.Shoot(aiModel.VehicleModel);
                    }
                }
            }
        }
    }
}