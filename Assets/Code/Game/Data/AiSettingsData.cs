using UnityEngine;

namespace Code.Game.Data
{
    [CreateAssetMenu(fileName = "AiSettingsData", menuName = "Data/Settings/AiSettingsData", order = 0)]
    public class AiSettingsData : ScriptableObject
    {
        [SerializeField] private string _aiPrefabPath = "";
        [SerializeField] private float _searchDistance = 100;
        [SerializeField] private int _frameCountToSearch = 2;


        public string AiPrefabPath => _aiPrefabPath;
        
        public float SearchDistance => _searchDistance;
        public int FrameCountToSearch => _frameCountToSearch;
    }
}