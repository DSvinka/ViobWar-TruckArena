using Photon.Pun;
using UnityEngine;

namespace Code.Game.Views
{
    public class TurretView: MonoBehaviourPun
    {
        [SerializeField] private AudioSource _audioSource;
        
        [Space, SerializeField] private Transform _gun;
        [SerializeField] private Transform _stand;
        
        [Space, SerializeField] private Transform _shootPoint;
        [SerializeField] private ParticleSystem _shootEffect;


        public AudioSource AudioSource => _audioSource;

        public Transform Gun => _gun;
        public Transform Stand => _stand;
        
        public Transform ShootPoint => _shootPoint;
        public ParticleSystem ShootEffect => _shootEffect;
    }
}