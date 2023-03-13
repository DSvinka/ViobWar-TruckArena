using UnityEngine;

namespace Code.Game.Data.Vehicles
{
    [CreateAssetMenu(fileName = "VehicleData", menuName = "Data/Vehicles/VehicleData", order = 0)]
    public class VehicleData: ScriptableObject
    {
        [SerializeField] private string _vehiclePrefabPath;
        [SerializeField] private AudioClip _engineSound;
        [SerializeField] private AudioClip _damageSound;
        [SerializeField] private AudioClip _explosionSound;

        [Space, SerializeField] private float _health = 500f;
        
        [Space, SerializeField] private float _speedMax = 70f;
        [SerializeField] private float _speedMin = -50f;
        [Space, SerializeField] private float _acceleration = 30f;
        [SerializeField] private float _brakeSpeed = 100f;
        [SerializeField] private float _reverseSpeed = 30f;
        [SerializeField] private float _idleSlowdown = 10f;
        
        [Space, SerializeField] private float _motorTorque = 50000;
        [SerializeField] private float _steerSpeed = 80;
        [SerializeField] private float _steerAngle = 45;


        public string VehiclePrefabPath => _vehiclePrefabPath;
        public AudioClip EngineSound => _engineSound;
        public AudioClip DamageSound => _damageSound;
        public AudioClip ExplosionSound => _explosionSound;

        public float Health => _health;
        
        public float MotorTorque => _motorTorque;
        public float SteerSpeed => _steerSpeed;
        public float SteerAngle => _steerAngle;

        public float SpeedMax => _speedMax;
        public float SpeedMin => _speedMin;
        public float Acceleration => _acceleration;
        public float BrakeSpeed => _brakeSpeed;
        public float ReverseSpeed => _reverseSpeed;
        public float IdleSlowdown => _idleSlowdown;
    }
}