using BattleCityClone.Gameplay.Manager;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerStateController : NetworkBehaviour, IDamagable
    {
        [Header("Settings")]
        [SerializeField] private int maxHealth = 100;
        [SyncVar, SerializeField] private int currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            maxHealth -= damageAmount;
            maxHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (maxHealth <= 0 && IsServer)
                DestroyPlayerRpc();
        }

        private void DestroyPlayerRpc()
        {
            GameplayManager.Instance.GameplayNetworkManager.ServerObjectManager.Destroy(gameObject);
        }

    }
}
