using BattleCityClone.Gameplay.Manager;
using Cysharp.Threading.Tasks;
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

    }
}
