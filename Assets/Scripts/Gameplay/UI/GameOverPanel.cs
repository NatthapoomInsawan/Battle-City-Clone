using BattleCityClone.Gameplay;
using BattleCityClone.Gameplay.Manager;
using UnityEngine.InputSystem;

namespace BattleCityClone.UI
{
    public class GameOverPanel : BaseUI
    {
        private PlayerInputAction playerInputAction;

        private bool isReady;

        public override void Init()
        {
            playerInputAction = new PlayerInputAction();
            
            GameplayManager.Instance.GameplayStateManager.OnStateChanged += UIStateCheck;
            
            playerInputAction.UI.Confirm.performed += OnConfirmButton;
            playerInputAction.UI.Cancel.performed += OnCancelButton;
        }

        private void UIStateCheck(GameplayState currentGameState)
        {
            if (currentGameState is GameplayOverState)
                Open();
            else
                Close();
        }

        public override void Open()
        {
            playerInputAction.Enable();
            base.Open();
        }

        private void OnConfirmButton(InputAction.CallbackContext callBack)
        {
            if (isReady)
                return;

            GameplayManager.Instance.RequestRestartGame();
        }

        private void OnCancelButton(InputAction.CallbackContext callBack)
        {
            if (!isReady)
                return;

            GameplayManager.Instance.RequestCancelRestartGame();
        }

    }
}
