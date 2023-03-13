namespace Code.Game.Abstractions.Controller
{
    public interface ILateExecute : IController
    {
        void LateExecute(bool isMaster);
    }
}