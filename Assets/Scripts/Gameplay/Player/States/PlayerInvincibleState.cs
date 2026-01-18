using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerInvincibleState : IState
    {
        public event Action OnInvicibleEnd;

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


        public void EnterState()
        {
            //for testing
            stateController.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            DurationTask(cancellationTokenSource.Token).Forget();
        }

        public void ExitState()
        {
            cancellationTokenSource.Cancel();
            
            //for testing
            if (stateController == null)
                return;

            stateController.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }

        private async UniTaskVoid DurationTask(CancellationToken cancellationToken)
        {
            if (await UniTask.Delay(TimeSpan.FromSeconds(stateDuration),  cancellationToken: cancellationToken).SuppressCancellationThrow())
                return;
            OnInvicibleEnd?.Invoke();
            ExitState();
        }
    }
}
