using System.Collections.Generic;
using Code.Game.Abstractions.Controller;

namespace Code.Core.Controller
{
    public class GameControllers: IExecute, IInitialization, ILateExecute, ICleanup, IFixedExecute, IAwake, IDrawGizmos
    {
        private readonly List<IInitialization> _initializeControllers;
        private readonly List<IExecute> _executeControllers;
        private readonly List<ILateExecute> _lateControllers;
        private readonly List<ICleanup> _cleanupControllers;
        private readonly List<IFixedExecute> _fixedExecuteControllers;
        private readonly List<IAwake> _awakeExecuteControllers;
        private readonly List<IDrawGizmos> _drawGizmosControllers;

        public GameControllers()
        {
            _initializeControllers = new List<IInitialization>();
            _executeControllers = new List<IExecute>();
            _lateControllers = new List<ILateExecute>();
            _cleanupControllers = new List<ICleanup>();
            _fixedExecuteControllers = new List<IFixedExecute>();
            _drawGizmosControllers = new List<IDrawGizmos>();
        }

        public GameControllers Add(IController controller)
        {
            if (controller is IInitialization initializeController)
            {
                _initializeControllers.Add(initializeController);
            }

            if (controller is IExecute executeController)
            {
                _executeControllers.Add(executeController);
            }

            if (controller is ILateExecute lateExecuteController)
            {
                _lateControllers.Add(lateExecuteController);
            }
            
            if (controller is ICleanup cleanupController)
            {
                _cleanupControllers.Add(cleanupController);
            }

            if (controller is IFixedExecute fixedExecute)
            {
                _fixedExecuteControllers.Add(fixedExecute);
            }

            if (controller is IDrawGizmos drawGizmos)
            {
                _drawGizmosControllers.Add(drawGizmos);
            }
            
            return this;
        }

        public void Remove(IController controller)
        {
            if (controller is IInitialization initializeController)
            {
                _initializeControllers.Remove(initializeController);
            }

            if (controller is IExecute executeController)
            {
                _executeControllers.Remove(executeController);
            }

            if (controller is ILateExecute lateExecuteController)
            {
                _lateControllers.Remove(lateExecuteController);
            }
            
            if (controller is ICleanup cleanupController)
            {
                _cleanupControllers.Remove(cleanupController);
            }

            if (controller is IFixedExecute fixedExecute)
            {
                _fixedExecuteControllers.Remove(fixedExecute);
            }
            
            if (controller is IDrawGizmos drawGizmos)
            {
                _drawGizmosControllers.Add(drawGizmos);
            }
        }

        public void Execute(bool isMaster)
        {
            _executeControllers?.ForEach(x => x.Execute(isMaster));
        }

        public void Init(bool isMaster)
        {
            _initializeControllers.ForEach(x=> x.Init(isMaster));
        }

        public void LateExecute(bool isMaster)
        {
            _lateControllers?.ForEach(x => x.LateExecute(isMaster));
        }

        public void Cleanup(bool isMaster)
        {
            _cleanupControllers?.ForEach(x => x.Cleanup(isMaster));
        }

        public void FixedExecute(bool isMaster)
        {
            _fixedExecuteControllers?.ForEach(x => x.FixedExecute(isMaster));
        }

        public void Awake(bool isMaster)
        {
            _awakeExecuteControllers?.ForEach(x => x.Awake(isMaster));
        }

        public void DrawGizmos()
        {
            _drawGizmosControllers?.ForEach(x => x.DrawGizmos());
        }
    }
}