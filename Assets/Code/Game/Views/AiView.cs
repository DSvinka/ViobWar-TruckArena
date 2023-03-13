using UnityEngine;
using UnityEngine.AI;

namespace Code.Game.Views
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiView: MonoBehaviour
    {
        [SerializeField] private Transform _radarObject;
        [SerializeField] private NavMeshAgent _agent;

        public NavMeshAgent Agent => _agent;
        public Transform RadarObject => _radarObject;

        private void Update()
        {
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.nextPosition = transform.position;
        }
    }
}