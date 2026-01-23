using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay;

namespace BattleCityClone.UI
{
    public class WaitForPlayerPanel : BaseUI
    {
        public override void Init()
        {
            GameplayManager.Instance.GameplayStateManager.OnStateChanged += UIStateCheck;
        }

        private void UIStateCheck(GameplayState currentGameState)
        {
            if (currentGameState is GameplayWaitForPlayerState)
                Open();
            else
                Close();
        }   
    }
}
