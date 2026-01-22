using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayNetworkManager : NetworkManager
    {
        public INetworkPlayer LocalPlayer => localPlayer;

        public int AuthenticatedPlayer => authenticatedPlayer;

        [Header("Nerwork Settings")]
        [SerializeField] private string address = "localhost";
        [SerializeField] private ushort port = 7777;

        private int authenticatedPlayer = 0;

        private INetworkPlayer localPlayer;

        private void Awake()
        {
            Server.Started.AddListener(OnServerStarted);
            Server.Authenticated.AddListener(OnAuthenticated);
            Client.Connected.AddListener(OnConnected);
            Client.Disconnected.AddListener(OnDisconnected);
        }

        public async UniTask Init()
        {
            Client.Connect(address, port);

            if (await UniTask.WaitUntil(() => Client.IsConnected).SuppressCancellationThrow())
                return;
        }

        private void OnServerStarted()
        {
            Debug.Log("Server has started..");
        }

        private async void OnAuthenticated(INetworkPlayer player)
        {
            await UniTask.WaitUntil(()=>player.SceneIsReady);

            authenticatedPlayer++;
        }

        private void OnConnected(INetworkPlayer networkPlayer) => localPlayer = networkPlayer;

        private async void OnDisconnected(ClientStoppedReason arg0)
        {
            if (arg0.Equals(ClientStoppedReason.ConnectingTimeout))
            {
                Debug.Log("ConnectingTimeout. start hosting server...");

                await UniTask.WaitUntil(() => !Client.IsConnected);

                try
                {
                    Server.StartServer(Client);
                }
                catch
                {
                    Debug.Log("Try reconnecting again...");
                    Client.Connect(address, port);
                }

            }
        }

    }
}
