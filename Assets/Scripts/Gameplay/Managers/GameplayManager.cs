using Cysharp.Threading.Tasks;
using Mirage;
using System;
using System.Collections.Generic;
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
        [SerializeField] private List<uint> readyPlayerIds = new();

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
                await UniTask.WaitUntil(() => gameplayNetworkManager.AuthenticatedPlayer == maxPlayers);
                StartGame().Forget();
            }
        }

        private async UniTaskVoid StartGame()
        {
            readyPlayerIds.Clear();
            gameplayStateManager.SetState(new GameplayStartedState());

            await UniTask.WaitUntil(() => gameplayStateManager.CurrentState is GameplayOverState && readyPlayerIds.Count == maxPlayers);
            
            StartGame().Forget();
        }

        public void RequestRestartGame()
        {
            if (!IsServer)
                RequestRestartGameRpc(gameplayNetworkManager.LocalPlayer.Identity.NetId);
            else
                AddReadyPlayer(gameplayNetworkManager.LocalPlayer.Identity.NetId);
        }

        public void RequestCancelRestartGame()
        {
            if (!IsServer)
                RequestCancelStartGameRpc(gameplayNetworkManager.LocalPlayer.Identity.NetId);
            else
                RemoveReadyPlayer(gameplayNetworkManager.LocalPlayer.Identity.NetId);
        }


        [ServerRpc]
        private void RequestRestartGameRpc(uint netId) => AddReadyPlayer(netId);

        [ServerRpc]
        private void RequestCancelStartGameRpc(uint netId) => RemoveReadyPlayer(netId);

        private void AddReadyPlayer(uint netId)
        {
            if (readyPlayerIds.Contains(netId))
                return;
            else
                readyPlayerIds.Add(netId);
        }

        private void RemoveReadyPlayer(uint netId)
        {
            if (!readyPlayerIds.Contains(netId))
                return;
            else
                readyPlayerIds.Remove(netId);
        }

    }
}
