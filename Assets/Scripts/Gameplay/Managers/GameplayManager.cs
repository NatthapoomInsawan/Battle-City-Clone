using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        public GameplayNetworkManager GameplayNetworkManager => gameplayNetworkManager;
        public GameplayStateManager GameplayStateManager => gameplayStateManager;

        [Header("Network")]
        [SerializeField] private GameplayNetworkManager gameplayNetworkManager;
        [SerializeField] private GameplayStateManager gameplayStateManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log("Destroy");

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
                gameplayNetworkManager.Server.Connected.AddListener((con) =>{
                    if (gameplayNetworkManager.Server.AllPlayers.Count >= 2)
                        StartGame();
                });     
        }

        private void StartGame()
        {
            gameplayStateManager.SetState(new GameplayStartedState());
        }

    }
}
