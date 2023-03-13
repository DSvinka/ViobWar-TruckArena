namespace Code.Game.Abstractions.Controller
{
    public interface IInitialization : IController
    {
        void Init(bool isMaster);
    }
}