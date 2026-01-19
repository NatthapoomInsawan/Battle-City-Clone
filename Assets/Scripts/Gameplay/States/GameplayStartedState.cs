using System;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    [Serializable]
    public class GameplayStartedState : GameplayState
    {
        public override void EnterState()
        {
            Debug.Log("Game Started.");
        }
        public override void ExitState() { }
    }
}
