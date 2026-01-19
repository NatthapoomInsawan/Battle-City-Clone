using BattleCityClone.Gameplay.Manager;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerStateController : NetworkBehaviour, IDamagable
    {
        public PlayerState CurrentState => currentState;

        [Header("Player Settings")]
        [SerializeField] private int maxHealth = 100;
        [SyncVar, SerializeField] private int currentHealth;

        [Header("State Settings")]
        [SerializeField] float invincibleDuration = 3f;

        [SerializeReference] private PlayerState currentState;

        private void Awake()
        {
            currentHealth = maxHealth;
            SetState(new PlayerIdleState());
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            SetState(new PlayerInvincibleState());

            if (currentHealth <= 0 && IsServer)
                DestroyPlayer();
        }

        private void SetState(PlayerState state)
        {
            if (this == null)
                return;

            if (currentState != null)
                currentState.ExitState();

            currentState = state;

            InitPlayerState(currentState);

            if (IsServer)
                SendClientStateRpc(currentState);

            currentState.EnterState();
            
            if (currentState.GetNextState() != null)
            {
                currentState.OnExitState += () =>
                {
                    PlayerState nextState = currentState.GetNextState();
                    currentState = null;
                    SetState(nextState);
                };
            }
                
        }

        private void InitPlayerState(PlayerState state)
        {
            switch (state)
            {
                case PlayerInvincibleState playerInvincibleState:
                    playerInvincibleState.Init(invincibleDuration, this);
                    break;
            }
        }

        [ClientRpc(excludeHost = true)]
        private void SendClientStateRpc(PlayerState newState)
        {
            SetState(newState);
        }

        private void DestroyPlayer()
        {
            GameplayManager.Instance.GameplayNetworkManager.ServerObjectManager.Destroy(gameObject);
        }

    }
}
