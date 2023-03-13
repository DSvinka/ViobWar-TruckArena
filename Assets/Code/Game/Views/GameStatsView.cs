using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Code.Game.Views
{
    public class GameStatsView: MonoBehaviourPun
    {
        public float AlivePlayersCount;
        public List<Transform> FreeSpawners;
    }
}