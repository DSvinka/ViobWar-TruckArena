namespace Code.Game.Abstractions.Controller
{
    public interface IFixedExecute : IController
    {
        void FixedExecute(bool isMaster);
    }
}