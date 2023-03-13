namespace Code.Game.Abstractions.Controller
{
    public interface IAwake: IController
    {
        void Awake(bool isMaster);
    }
}