using Code.Game.Abstractions;
using Code.Game.Views;
using UnityEngine;

namespace Code.Game.Models
{
    public class AiModel: BaseUnitModel
    {
        private readonly AiView _aiView;


        public AiView AiView => _aiView;
        public VehicleModel VehicleModel { get; set; }
        public Transform Target { get; set; }
        

        public AiModel(AiView aiView)
        {
            _aiView = aiView;
        }
    }
}