using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    [Serializable]
    public class PlayerInvincibleState : PlayerState
    {
        public PlayerStateController PlayerStateController => PlayerStateController;
        public float StateDuration => stateDuration;

        PlayerStateController stateController;

        private float stateDuration;

        private CancellationTokenSource cancellationTokenSource;

        public void Init(float stateDuration, PlayerStateController stateController)
        {
            this.stateDuration = stateDuration;
            this.stateController = stateController;
            cancellationTokenSource = new CancellationTokenSource();
        }


        public override void EnterState()
        {
            //for testing
            stateController.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            DurationTask(cancellationTokenSource.Token).Forget();
        }

        public override void ExitState()
        {
            cancellationTokenSource.Cancel();
            
            //for testing
            if (stateController == null)
                return;

            stateController.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            base.ExitState();
        }

        private async UniTaskVoid DurationTask(CancellationToken cancellationToken)
        {
            if (await UniTask.Delay(TimeSpan.FromSeconds(stateDuration),  cancellationToken: cancellationToken).SuppressCancellationThrow())
                return;
            ExitState();
        }

        public override PlayerState GetNextState()
        {
            return new PlayerIdleState();
        }
    }
}
