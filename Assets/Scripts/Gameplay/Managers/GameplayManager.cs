using Cysharp.Threading.Tasks;
using Mirage;
using System;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayManager : NetworkBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        public GameplayNetworkManager GameplayNetworkManager => gameplayNetworkManager;
        public GameplayStateManager GameplayStateManager => gameplayStateManager;

        [Header("Network")]
        [SerializeField] private GameplayNetworkManager gameplayNetworkManager;
        [SerializeField] private GameplayStateManager gameplayStateManager;

        [Header("Settings")]
        [SerializeField] private int maxPlayers = 2;

        [Header("States")]
        [SerializeField] private int requestRestartPlayer = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("Game Manager Init");
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            try
            {
                gameplayStateManager.Init();
                await gameplayNetworkManager.Init();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            Debug.Log("GameplayManager initialized.");

            if (gameplayNetworkManager.Server.IsHost)
            {
                await UniTask.WaitUntil(()=> gameplayNetworkManager.AuthenticatedPlayer == maxPlayers);
                StartGame().Forget();
            }    
        }

        private async UniTaskVoid StartGame()
        {
            gameplayStateManager.SetState(new GameplayStartedState());

            await UniTask.WaitUntil(() => gameplayStateManager.CurrentState is GameplayOverState && requestRestartPlayer == maxPlayers);

            requestRestartPlayer = 0;

            StartGame().Forget();
        }

        public void RequestRestartGame()
        {
            if (Identity.HasAuthority && IsClient)
                RequestRestartGameRpc();
        }

        public void RequestCancelRestartGame()
        {
            if (Identity.HasAuthority && IsClient)
                RequestCancelStartGameRpc();
        }


        [ServerRpc]
        private void RequestRestartGameRpc() => requestRestartPlayer++;

        [ServerRpc]
        private void RequestCancelStartGameRpc() => requestRestartPlayer--;

    }
}
