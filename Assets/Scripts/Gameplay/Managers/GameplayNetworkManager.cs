using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayNetworkManager : NetworkManager
    {
        public int PlayerReady => playerReady;

        [Header("Nerwork Settings")]
        [SerializeField] private string address = "localhost";
        [SerializeField] private ushort port = 7777;

        private int playerReady = 0;

        private void Awake()
        {
            Server.Started.AddListener(OnServerStarted);
            Server.Authenticated.AddListener(OnAuthenticated);
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

            playerReady++;
        }

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
