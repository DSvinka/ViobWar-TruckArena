using System.Linq;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Game.Abstractions.Controller;
using Code.Game.Views;
using Code.Shared.Constants;

namespace Code.Game.Controllers
{
    public class GameStatsController: IInitialization
    {
        private readonly GameStatsView _gameStatsView;
        private readonly MapView _mapView;

        public GameStatsView GameStatsView => _gameStatsView;
        
        
        public GameStatsController(GameStatsView gameStatsView, MapView mapView)
        {
            _mapView = mapView;
            _gameStatsView = gameStatsView;
            
            _gameStatsView.FreeSpawners = _mapView.SpawnPoints.ToList();
        }
        

        public void Init(bool isMaster)
        {
            if (!isMaster)
                return;
            
            var botsCount = (byte) Singleton<RoomManager>.Instance.CurrentRoom.CustomProperties[PhotonRoomKeys.RoomBotsCountKey];
            var playerCount = (byte) Singleton<RoomManager>.Instance.CurrentRoom.PlayerCount;
            _gameStatsView.AlivePlayersCount = botsCount + playerCount;
        }
    }
}