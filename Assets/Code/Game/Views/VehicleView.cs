using System;
using System.Collections.Generic;
using Code.Game.Data.Vehicles;
using Code.Game.Enums;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Game.Views
{
    public class VehicleView : MonoBehaviourPun
    {
        #region Settings
        
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Transform _cameraLook;
        
        [SerializeField] private VehicleWheelAxis[] _wheelsAxis;
        [SerializeField] private VehicleTurretRack[] _turretRacks;
        
        
        public AudioSource AudioSource => _audioSource;
        public Transform CameraLook => _cameraLook;
        
        public IReadOnlyList<VehicleWheelAxis> WheelsAxis => _wheelsAxis;
        public IReadOnlyList<VehicleTurretRack> TurretRacks => _turretRacks;

        #endregion
        
        
        public event Action<VehicleView, TurretData, VehicleTurretRack> OnSetTurret;
        public event Action<VehicleView, VehicleView, float> OnAddDamage;
        public event Action<VehicleView, float> OnAddHealth;

        public event Action<VehicleView> OnDamagedEnemyDeath;


        private float _displayHealth;
        public float DisplayHealth => _displayHealth;

        
        public void AddHealth(float health)
        {
            OnAddHealth?.Invoke(this, health);
        }
        
        public void AddDamage(VehicleView enemy, float damage)
        {
            OnAddDamage?.Invoke(this, enemy, damage);
        }
        
        public void AddDamage(float damage)
        {
            OnAddDamage?.Invoke(this, null, damage);
        }

        public void DamagedEnemyDeath(VehicleView enemy)
        {
            OnDamagedEnemyDeath?.Invoke(enemy);
        }

        public void SetTurret(TurretData turretData, VehicleTurretRack vehicleTurretRack)
        {
            OnSetTurret?.Invoke(this, turretData, vehicleTurretRack);
        }
        
        public void SetTurretToRandomRack(TurretData turretData)
        {
            OnSetTurret?.Invoke(this, turretData, null);
        }

        public void SetDisplayHealth(float health)
        {
            _displayHealth = health;
        }
    }

    [Serializable]
    public class VehicleTurretRack
    {
        [Header("Настройка")] 
        [SerializeField] private Transform _turretInstallPoint;
        [SerializeField] private ETurretRackSize _turretRackSize;


        public Transform TurretInstallPoint => _turretInstallPoint;
        public ETurretRackSize TurretRackSize => _turretRackSize;
    }

    [Serializable]
    public class VehicleWheelAxis
    {
        [Header("Компоненты")]
        [SerializeField] private WheelCollider[] _wheels;

        [Space, Header("Настройка")]
        [SerializeField] private bool _isMotor;
        [FormerlySerializedAs("_isTurning")] [SerializeField] private bool _isSteering;


        public IReadOnlyList<WheelCollider> Wheels => _wheels;

        public bool IsMotor => _isMotor;
        public bool IsSteering => _isSteering;
    }
}