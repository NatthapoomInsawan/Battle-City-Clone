using System;

namespace BattleCityClone.Gameplay
{
    [Serializable]
    public abstract class GameplayState : IState
    {
        public virtual void EnterState() { }
        public virtual void ExitState() { }
    }
}
