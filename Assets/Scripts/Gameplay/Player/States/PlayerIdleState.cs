using System;

namespace BattleCityClone.Gameplay.Player
{
    [Serializable]
    public class PlayerIdleState : PlayerState
    {
        public override void EnterState() { }

        public override void ExitState() { }

        public override PlayerState GetNextState() { return null; }
    }
}
