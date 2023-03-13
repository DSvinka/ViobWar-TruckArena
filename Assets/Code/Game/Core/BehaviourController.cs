using Cinemachine;
using Code.Core.Controller;
using Code.Game.Data;
using Code.Game.Views;
using Code.Game.Views.Hud;
using Photon.Pun;
using UnityEngine;

namespace Code.Game.Core
{
    public class BehaviourController: MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;

        [SerializeField, Space] private MapView _mapView;
        [SerializeField] private HudView _hudView;
        [SerializeField] private GameStatsView _gameStatsView;
        
        [SerializeField, Space] private GameData _gameData;
        
        private GameControllers _gameControllers;

        private void Awake()
        {
            _gameControllers = new GameControllers();
            new GameInit(
                _gameControllers, _cinemachineFreeLook, _gameData, 
                _mapView, _hudView, _gameStatsView
            );
           
            _gameControllers.Awake(PhotonNetwork.IsMasterClient);
        }

        private void Start()
        {
            _gameControllers.Init(PhotonNetwork.IsMasterClient);
        }

        private void Update()
        {
            _gameControllers.Execute(PhotonNetwork.IsMasterClient);
        }

        private void FixedUpdate()
        {
            _gameControllers.FixedExecute(PhotonNetwork.IsMasterClient);
        }

        private void LateUpdate()
        {
            _gameControllers.LateExecute(PhotonNetwork.IsMasterClient);
        }

        private void OnDestroy()
        {
            _gameControllers.Cleanup(PhotonNetwork.IsMasterClient);
        }

        private void OnDrawGizmos()
        {
            if (_gameControllers == null)
                return;
            
            _gameControllers.DrawGizmos();
        }
    }
}