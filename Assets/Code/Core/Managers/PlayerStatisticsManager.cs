using System;
using System.Collections.Generic;
using Code.Core.Singleton;
using Code.Core.Utils;
using Code.Shared.Constants;
using Code.Shared.Models;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Code.Core.Managers
{
    public class PlayerStatisticsManager: MonoBehaviour, ISingleton
    {
        public event Action<PlayFabError> OnUpdateStatisticsFailed;

        public void UpdateStatistics(StatisticUpdate statisticUpdate)
        {
            var statisticUpdateList = new List<StatisticUpdate>()
            {
                statisticUpdate
            };
            
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = statisticUpdateList
            };
            
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateStatisticsResult, OnUpdateStatisticsError);
        }
        
        public void UpdateStatistics(PlayerStatisticsModel model)
        {
            var statisticUpdateList = new List<StatisticUpdate>();

            foreach (var (key, statisticUpdate) in model.StatisticUpdates)
            {
                if (statisticUpdate.Value != -1)
                {
                    statisticUpdateList.Add(statisticUpdate);
                }
            }
            
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = statisticUpdateList
            };
            
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateStatisticsResult, OnUpdateStatisticsError);
        }
        
        private void OnUpdateStatisticsResult(UpdatePlayerStatisticsResult result)
        {
            DLogger.Debug(GetType(), nameof(OnUpdateStatisticsResult), 
                "Update Statistics - Success");
        }

        private void OnUpdateStatisticsError(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnUpdateStatisticsError), 
                $"UpdateStatistics - Failed: {errorMessage}");
            OnUpdateStatisticsFailed?.Invoke(error);
        }

        
        
        public event Action<List<StatisticValue>> OnGetStatistics;
        public event Action<PlayFabError> OnGetStatisticsFailed;

        private PlayerStatisticsModel _statisticsModel = new ();
        public PlayerStatisticsModel StatisticsModel => _statisticsModel;
        
        public void GetStatistics()
        {
            var request = new GetPlayerStatisticsRequest()
            {
                StatisticNames = PlayerStatisticsKeys.Keys
            };
                
            PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatisticsResult, OnGetStatisticsError);
        }
        
        private void OnGetStatisticsResult(GetPlayerStatisticsResult result)
        {
            DLogger.Debug(GetType(), nameof(OnGetStatisticsResult), 
                "GetStatistics - Success");
            _statisticsModel.ReadStatisticValuesList(result.Statistics);
            OnGetStatistics?.Invoke(result.Statistics);
        }

        private void OnGetStatisticsError(PlayFabError error)
        {
            var errorMessage = error.GenerateErrorReport();
            DLogger.Error(GetType(), nameof(OnGetStatisticsError), 
                $"GetStatistics - Failed: {errorMessage}");
            OnGetStatisticsFailed?.Invoke(error);
        }
    }
}