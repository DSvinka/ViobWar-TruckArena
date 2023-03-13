using System.Collections.Generic;
using Code.Shared.Constants;
using PlayFab.ClientModels;

namespace Code.Shared.Models
{
    public class PlayerStatisticsModel
    {
        public readonly StatisticUpdate KillsCount = new () { StatisticName = PlayerStatisticsKeys.KillsCount, Value = -1 };
        public readonly StatisticUpdate DeathsCount = new () { StatisticName = PlayerStatisticsKeys.DeathsCount, Value = -1 };
        
        public readonly StatisticUpdate LossesCount = new () { StatisticName = PlayerStatisticsKeys.LossesCount, Value = -1 };
        public readonly StatisticUpdate WinsCount = new () { StatisticName = PlayerStatisticsKeys.WinsCount, Value = -1 };


        private readonly Dictionary<string, StatisticUpdate> _statisticUpdates;
        public IReadOnlyDictionary<string, StatisticUpdate> StatisticUpdates => _statisticUpdates;

        
        public PlayerStatisticsModel()
        {
            _statisticUpdates = new ()
            {
                { KillsCount.StatisticName, KillsCount },
                { DeathsCount.StatisticName, DeathsCount },
                { LossesCount.StatisticName, LossesCount },
                { WinsCount.StatisticName, WinsCount },
            };
        }

        public void ReadStatisticValuesList(List<StatisticValue> statisticValues)
        {
            foreach (var statisticValue in statisticValues)
            {
                _statisticUpdates[statisticValue.StatisticName].Value = statisticValue.Value;
            }
        }

        /// <summary>
        /// Превращает пустые значения из -1 в 0 и возвращает полученную модель.
        /// </summary>
        public PlayerStatisticsModel ToDisplayValues()
        {
            var model = new PlayerStatisticsModel();
            foreach (var (key, statisticUpdate) in _statisticUpdates)
            {
                model.StatisticUpdates[key].Value = statisticUpdate.Value == -1 ? 0 : statisticUpdate.Value;
            }
            return model;
        }
    }
}