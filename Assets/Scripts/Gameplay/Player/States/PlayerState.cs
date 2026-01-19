
using System;

namespace BattleCityClone.Gameplay.Player
{
    [Serializable]
    public abstract class PlayerState : IState
    {
        public event Action OnExitState;

        public virtual void EnterState() { }
        public virtual void ExitState() 
        {
            OnExitState?.Invoke();
        }

        public abstract PlayerState GetNextState();
    }
}
