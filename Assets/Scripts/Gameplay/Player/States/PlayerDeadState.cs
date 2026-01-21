using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerDeadState : PlayerState
    {
        private GameObject playerGamObject;
        public void Init(GameObject playerGamObject)
        {
            this.playerGamObject = playerGamObject;
        }

        public override void EnterState()
        {
           playerGamObject.SetActive(false);
        }

        public override PlayerState GetNextState() => null;
    }
}
