using BattleCityClone.Gameplay.Manager;
using Cysharp.Threading.Tasks;
using Mirage;
using System;
using UnityEngine;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerStateController : NetworkBehaviour, IDamagable
    {
        public event Action<PlayerState> OnPlayerStateChanged;
        public event Action<int> OnHealthChanged;
        public PlayerState CurrentState => currentState;
        public PlayerInfo PlayerInfo { get => playerInfo; set { playerInfo = value; } }
        public int CurrentHealth => currentHealth;

        [Header("References")]
        [SerializeField] private PlayerAnimationController playerAnimationController;

        [Header("Player Settings")]
        [SerializeField] private int maxHealth = 100;
        [SyncVar(hook = nameof(OnHealthChangedSync)), SerializeField] private int currentHealth;
        [Header("State Settings")]
        [SerializeField] float invincibleDuration = 3f;

        [Header("Audio")]
        [SerializeField] private AudioSource hitAudioSource;

        [SerializeReference] private PlayerState currentState;

        [SerializeField, SyncVar] private PlayerInfo playerInfo;

        private void Awake()
        {
            Identity.OnAuthorityChanged.AddListener(OnStartAuthority);
        }

        private async void OnStartAuthority(bool changed)
        {
            await UniTask.WaitUntil(()=> GameplayManager.Instance.GameplayRpcManager != null);

            if (changed)
                RequestAuthorityForLocalManagerRpc(GameplayManager.Instance.GameplayRpcManager.Identity, GameplayManager.Instance.GameplayNetworkManager.Client.Player);
        }

        public void Init()
        {
            if (IsServer)
            {
                InitPlayerRpc();
                SetState(new PlayerIdleState());
            }
        }

        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            SetState(new PlayerInvincibleState());

            OnHealthChanged?.Invoke(currentHealth);
            hitAudioSource.Play();

            if (currentHealth <= 0 && IsServer)
                SetState(new PlayerDeadState());
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

            OnPlayerStateChanged?.Invoke(currentState);
        }

        private void InitPlayerState(PlayerState state)
        {
            switch (state)
            {
                case PlayerInvincibleState playerInvincibleState:
                    playerInvincibleState.Init(invincibleDuration, playerAnimationController);
                    break;
                case PlayerDeadState playerDeadState:
                    playerDeadState.Init(gameObject);
                    break;
            }
        }

        private void OnHealthChangedSync(int newHealth)
        {
            currentHealth = newHealth;
            hitAudioSource.Play();
            OnHealthChanged?.Invoke(currentHealth);
        }


        [ClientRpc(excludeHost = true)]
        private void SendClientStateRpc(PlayerState newState)
        {
            SetState(newState);
        }

        [ClientRpc]
        private void InitPlayerRpc()
        {
            currentHealth = maxHealth;
            gameObject.SetActive(true);
        }

        [ServerRpc]
        private void RequestAuthorityForLocalManagerRpc(NetworkIdentity gameManagerIdentity, INetworkPlayer player)
        {
            gameManagerIdentity.AssignClientAuthority(player);
        }
    }
}
