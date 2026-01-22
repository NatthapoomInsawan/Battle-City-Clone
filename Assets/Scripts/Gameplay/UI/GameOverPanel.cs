using BattleCityClone.Gameplay;
using BattleCityClone.Gameplay.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCityClone.UI
{
    public class GameOverPanel : BaseUI
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI stateText;

        private PlayerInputAction playerInputAction;

        private bool isReady;

        public override void Init()
        {
            playerInputAction = new PlayerInputAction();
            
            GameplayManager.Instance.GameplayStateManager.OnStateChanged += UIStateCheck;
            
            playerInputAction.UI.Confirm.performed += OnConfirmButton;
            playerInputAction.UI.Cancel.performed += OnCancelButton;

            UpdateStateText();
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
            isReady = false;
            UpdateStateText();
            base.Open();
        }

        public override void Close()
        {
            playerInputAction.Disable();
            base.Close();
        }

        private void OnConfirmButton(InputAction.CallbackContext callBack)
        {
            if (isReady)
                return;

            GameplayManager.Instance.RequestRestartGame();
            isReady = true;
            UpdateStateText();
        }

        private void OnCancelButton(InputAction.CallbackContext callBack)
        {
            if (!isReady)
                return;

            GameplayManager.Instance.RequestCancelRestartGame();
            isReady = false;
            UpdateStateText();
        }

        private void UpdateStateText()
        {
            stateText.text = isReady ? "READY" : "NOT READY";
            stateText.color = isReady ? Color.green : Color.red;
        }

    }
}
