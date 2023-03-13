namespace Code.Game.Abstractions.Controller
{
    public interface ICleanup: IController
    {
        void Cleanup(bool isMaster);
    }
}