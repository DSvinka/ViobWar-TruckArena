using Code.Game.Enums;
using UnityEngine;

namespace Code.Game.Data.Vehicles
{
    [CreateAssetMenu(fileName = "TurretData", menuName = "Data/Vehicles/TurretData", order = 0)]
    public class TurretData: ScriptableObject
    {
        [SerializeField] private string _turretPrefabPath;
        [SerializeField] private AudioClip _shootingSound;
        [SerializeField] private ETurretRackSize _turretRackSize;

        [SerializeField] private float _damage = 5f;
        [SerializeField] private float _shootMaxDistance = 100f;
        
        [SerializeField] private float _standRotationSpeed = 5f;
        [SerializeField] private float _gunRotationSpeed = 5f;
        
        [SerializeField] private int _maxAmmoCount = 60;
        [SerializeField] private float _shootingDelaySeconds = 0.2f;
        [SerializeField] private float _reloadTimeSeconds = 5f;

        
        public string TurretPrefabPath => _turretPrefabPath;
        public AudioClip ShootingSound => _shootingSound;
        public ETurretRackSize TurretRackSize => _turretRackSize;

        public float Damage => _damage;
        public float ShootMaxDistance => _shootMaxDistance;
        
        public float StandRotationSpeed => _standRotationSpeed;
        public float GunRotationSpeed => _gunRotationSpeed;

        public int MaxAmmoCount => _maxAmmoCount;
        public float ShootingDelaySeconds => _shootingDelaySeconds;
        public float ReloadTimeSeconds => _reloadTimeSeconds;
    }
}