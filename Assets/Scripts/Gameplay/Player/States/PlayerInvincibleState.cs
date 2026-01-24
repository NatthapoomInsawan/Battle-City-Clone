using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace BattleCityClone.Gameplay.Player
{
    [Serializable]
    public class PlayerInvincibleState : PlayerState
    {
        public float StateDuration => stateDuration;

        PlayerAnimationController animationController;

        private float stateDuration;

        private CancellationTokenSource cancellationTokenSource;

        public void Init(float stateDuration, PlayerAnimationController animationController)
        {
            this.stateDuration = stateDuration;
            this.animationController = animationController;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public override void EnterState()
        {
            animationController.SetInvincible(true);
            DurationTask(cancellationTokenSource.Token).Forget();
        }

        public override void ExitState()
        {
            cancellationTokenSource.Cancel();

            animationController.SetInvincible(false);
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
