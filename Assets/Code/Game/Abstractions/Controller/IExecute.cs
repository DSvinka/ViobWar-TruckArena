namespace Code.Game.Abstractions.Controller
{
    public interface IExecute : IController
    {
        void Execute(bool isMaster);
    }
}