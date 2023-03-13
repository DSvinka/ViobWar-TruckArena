using System.Collections.Generic;

namespace Code.Shared.Constants
{
    public class PlayerStatisticsKeys
    {
        public const string KillsCount = "kills_count";
        public const string DeathsCount = "deaths_count";
        
        public const string LossesCount = "losses_count";
        public const string WinsCount = "wins_count";

        public static readonly List<string> Keys = new ()
        {
            KillsCount, DeathsCount, LossesCount, WinsCount
        };
    }
}