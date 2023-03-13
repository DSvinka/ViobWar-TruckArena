using UnityEngine;

namespace Code.Game.Data
{
    [CreateAssetMenu(fileName = "PlayerKeymapData", menuName = "Data/Player/PlayerKeymapData", order = 0)]
    public class PlayerKeymapData : ScriptableObject
    {
        [SerializeField] private string _horizontalAxis = "Horizontal";
        [SerializeField] private string _verticalAxis = "Vertical";
        
        [Space, SerializeField] private KeyCode _shootingKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode _rotateTruckOnStuck = KeyCode.O;
        

        public string HorizontalAxis => _horizontalAxis;
        public string VerticalAxis => _verticalAxis;
        
        public KeyCode ShootingKey => _shootingKey;
        public KeyCode RotateTruckOnStuck => _rotateTruckOnStuck;
    }
}