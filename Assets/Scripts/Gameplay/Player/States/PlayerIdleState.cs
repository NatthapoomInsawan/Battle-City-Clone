using System;

namespace BattleCityClone.Gameplay.Player
{
    [Serializable]
    public class PlayerIdleState : PlayerState
    {
        public override PlayerState GetNextState() => null;
    }
}
