using Cinemachine;
using Code.Core.Controller;
using Code.Core.Managers;
using Code.Core.Singleton;
using Code.Game.Controllers;
using Code.Game.Data;
using Code.Game.Views;
using Code.Game.Views.Hud;

namespace Code.Game.Core
{
    public class GameInit
    {
        public GameInit(
            GameControllers gameControllers, CinemachineFreeLook cinemachineFreeLook, GameData gameData,
            MapView mapView, HudView hudView, GameStatsView gameStatsView
        )
        {
            InitSingleton(gameControllers);

            var vehicleController = new VehicleController();
            var turretsController = new TurretsController();
            
            var gameStatsController = new GameStatsController(gameStatsView, mapView);
            gameControllers.Add(gameStatsController);
            
            var aiController = new AiController(
                vehicleController, turretsController, gameStatsController, gameStatsView,
                gameData.AISettingsData, gameData.VehiclesData, gameData.GameSettingsData
            );
            gameControllers.Add(aiController);

            var playerController = new PlayerController(
                vehicleController, turretsController, gameStatsView, gameStatsController, cinemachineFreeLook,
                gameData.PlayerNetworkData, gameData.PlayerKeymapData, gameData.GameSettingsData
            );
            gameControllers.Add(playerController);

            var hudController = new HudController(
                playerController, gameStatsController, hudView,
                gameData.GameSettingsData
            );
            gameControllers.Add(hudController);
        }
        
        private void InitSingleton(GameControllers controllers)
        {
            Singleton<PlayerStatisticsManager>.Instance.GetStatistics();
            controllers.Add(Singleton<SingletonManager>.Instance);
        }
    }
}