using System;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    [Serializable]
    public class GameplayWaitForPlayerState : GameplayState
    {
        public override void EnterState() 
        {
            Debug.Log($"Waiting for player");
        }
        public override void ExitState() { }
    }
}
