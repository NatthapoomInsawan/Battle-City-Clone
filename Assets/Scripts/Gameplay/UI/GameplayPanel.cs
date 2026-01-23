using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay;

namespace BattleCityClone.UI
{
    public class GameplayPanel : BaseUI
    {
        public override void Init()
        {
            GameplayManager.Instance.GameplayStateManager.OnStateChanged += UIStateCheck;
        }

        private void UIStateCheck(GameplayState currentGameState)
        {
            if (currentGameState is GameplayStartedState)
                Open();
            else
                Close();
        }
    }
}
