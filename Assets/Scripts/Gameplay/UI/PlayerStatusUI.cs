using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay.Player;
using Mirage;
using TMPro;
using UnityEngine;

namespace BattleCityClone.UI
{
    public class PlayerStatusUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerHealthText;

        private uint playerId;

        public void Init(PlayerStateController playerStateController)
        {
            GameplayManager.Instance.OnPlayerDisconnected += OnPlayerDisconnect;
            playerStateController.OnHealthChanged += OnPlayerHealthChanged;
            
            playerNameText.text = playerStateController.PlayerInfo.Name;
            if (playerStateController.Identity == GameplayManager.Instance.GameplayNetworkManager.LocalPlayer.Identity)
                playerNameText.color = Color.yellow;

            playerId = playerStateController.Identity.NetId;

        }

        private void OnPlayerHealthChanged(int currentHealth)
        {
            playerHealthText.text = $"HP: {currentHealth}";
        }

        private void OnPlayerDisconnect(INetworkPlayer player)
        {
            if (player.Identity.NetId == playerId)
                Destroy(gameObject);
        }

    }
}
