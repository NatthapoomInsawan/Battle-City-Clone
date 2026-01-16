using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        public event Action OnGameplayStart;

        public static GameplayManager Instance { get; private set; }

        public GameplayNetworkManager GameplayNetworkManager => gameplayNetworkManager;

        [Header("Network")]
        [SerializeField] private GameplayNetworkManager gameplayNetworkManager;

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
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            try
            {
                await gameplayNetworkManager.Init();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            Debug.Log("GameplayManager initialized.");

            await UniTask.WaitUntil(() => gameplayNetworkManager.Server.AllPlayers.Count >= 2);

            if (gameplayNetworkManager.Server.IsHost)
                StartGame();

        }

        private void StartGame()
        {
            Debug.Log("Game Started.");
            OnGameplayStart?.Invoke();
        }

    }
}
