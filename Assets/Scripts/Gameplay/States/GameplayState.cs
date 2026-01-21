using BattleCityClone.Gameplay.Player;
using System;

namespace BattleCityClone.Gameplay
{
    [Serializable]
    public abstract class GameplayState : IState
    {
        public event Action OnExitState;
        public virtual void EnterState() { }
        public virtual void ExitState()=> OnExitState?.Invoke();
        public abstract GameplayState GetNextState();
    }
}
