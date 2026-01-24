using BattleCityClone.Gameplay;
using BattleCityClone.Gameplay.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleCityClone.UI
{
    public class LoginPanel : BaseUI
    {
        [Header("References")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button connectButton;

        public override void Init()
        {
            Open();

            GameplayManager.Instance.GameplayStateManager.OnStateChanged += OnGameplayStateChange;

            connectButton.onClick.AddListener(async () =>
            {
                connectButton.interactable = false;
                buttonText.text = "CONNECTING..";

                PlayerInfo playerInfo = new PlayerInfo() 
                {
                    Name = nameInputField.text,
                };

                await GameplayManager.Instance.GameplayNetworkManager.ConnectToServer(playerInfo);

                Close();
            });
        }

        public override void Open()
        {
            buttonText.text = "CONNECT";
            connectButton.interactable = true;

            base.Open();
        }

        private void OnGameplayStateChange(GameplayState gameplayState)
        {
            if (gameplayState is not GameplayWaitForPlayerState || GameplayManager.Instance.IsServer)
                return;

            Open();
        }
    }
}
