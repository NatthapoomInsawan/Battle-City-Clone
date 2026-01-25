using Cysharp.Threading.Tasks;
using Mirage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayManager : NetworkBehaviour
    {
        public event Action<INetworkPlayer> OnPlayerDisconnected;
        public static GameplayManager Instance { get; private set; }

        public GameplayNetworkManager GameplayNetworkManager => gameplayNetworkManager;
        public GameplayStateManager GameplayStateManager => gameplayStateManager;
        public GameplayPlayerManager GameplayPlayerManager => gameplayPlayerManager;
        public GameplayRpcManager GameplayRpcManager => gameplayRpcManager;

        [Header("Network")]
        [SerializeField] private GameplayNetworkManager gameplayNetworkManager;
        [SerializeField] private GameplayStateManager gameplayStateManager;
        [SerializeField] private GameplayPlayerManager gameplayPlayerManager;
        [SerializeField] private GameplayRpcManager gameplayRpcManager;

        [Header("References")]
        [SerializeField] private UIManager uiManager;

        [Header("Settings")]
        [SerializeField] private int maxPlayers = 2;

        [Header("States")]
        [SerializeField] private List<uint> readyPlayerIds = new();

        [ContextMenu("Disconnected")]
        private void TestDisconnected()
        {
            GameplayNetworkManager.LocalPlayer.Disconnect();
        }

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
                gameplayPlayerManager.Init();
                gameplayNetworkManager.Init();
                uiManager.Init();
                gameplayStateManager.Init();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            await UniTask.WaitUntil(() => gameplayRpcManager != null);

            gameplayNetworkManager.Server.Started.AddListener(OnServerStarted);
            gameplayNetworkManager.Server.Disconnected.AddListener(OnServerDisconnected);
            gameplayNetworkManager.Server.OnStopHost.AddListener(OnStoppedHost);
            gameplayNetworkManager.Client.Disconnected.AddListener(OnClientDisconnected);

            Debug.Log("GameplayManager initialized.");
        }

        public void AssignRpcManager(GameplayRpcManager rpcManager)
        {
            gameplayRpcManager = rpcManager;
            BindRpcManagerEvent(rpcManager);
        }

        public void BindRpcManagerEvent(GameplayRpcManager rpcManager)
        {
            rpcManager.OnPlayerDisconnectedEvent += InvokePlayerDisconnected;
            rpcManager.OnrequestStartEvent += AddReadyPlayer;
            rpcManager.OnRequestCancelStartEvent += RemoveReadyPlayer;
        }

        private async void OnServerStarted()
        {
            Debug.Log("Server has started..");
            await UniTask.WaitUntil(() => gameplayNetworkManager.AuthenticatedPlayer == maxPlayers);
            StartGame().Forget();
        }

        private void OnServerDisconnected(INetworkPlayer player)
        {
            if (gameplayNetworkManager.Server.AllPlayers.Count() == 1)
                gameplayStateManager.SetState(new GameplayWaitForPlayerState());

            Debug.Log($"Player Disconnected: {player.Identity.NetId}");

            gameplayRpcManager.PlayerDisconnectedRpc(player);
        }

        private void OnStoppedHost() => gameplayStateManager.SetState(new GameplayWaitForPlayerState());
        private void OnClientDisconnected(ClientStoppedReason reason) 
        {
            if (reason == ClientStoppedReason.RemoteConnectionClosed)
                gameplayStateManager.SetState(new GameplayWaitForPlayerState());
        }


        private async UniTaskVoid StartGame()
        {
            readyPlayerIds.Clear();
            gameplayStateManager.SetState(new GameplayStartedState());

            await UniTask.WaitUntil(() => gameplayStateManager.CurrentState is GameplayOverState && readyPlayerIds.Count == gameplayNetworkManager.Server.AllPlayers.Count());
            
            StartGame().Forget();
        }

        public void RequestRestartGame()
        {
            if (!IsServer)
                gameplayRpcManager.RequestRestartGameRpc(gameplayNetworkManager.LocalPlayer.Identity.NetId);
            else
                AddReadyPlayer(gameplayNetworkManager.LocalPlayer.Identity.NetId);
        }

        public void RequestCancelRestartGame()
        {
            if (!IsServer)
                gameplayRpcManager.RequestCancelStartGameRpc(gameplayNetworkManager.LocalPlayer.Identity.NetId);
            else
                RemoveReadyPlayer(gameplayNetworkManager.LocalPlayer.Identity.NetId);
        }

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

        private void InvokePlayerDisconnected(INetworkPlayer networkPlayer) => OnPlayerDisconnected?.Invoke(networkPlayer);

        private void OnApplicationQuit()
        {
            if (IsServer)
                gameplayNetworkManager.Server.Stop();

            if (gameplayNetworkManager.LocalPlayer != null && gameplayNetworkManager.LocalPlayer.IsConnected)
                gameplayNetworkManager.LocalPlayer.Disconnect();
        }
    }
}
