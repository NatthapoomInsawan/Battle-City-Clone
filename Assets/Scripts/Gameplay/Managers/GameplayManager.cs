using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkManager networkManager;

    private void Awake()
    {
        networkManager.Server.Started.AddListener(OnServerStarted);
        networkManager.Client.Disconnected.AddListener(OnDisconnected);
        networkManager.Server.Disconnected.AddListener(OnClientDisconnectedOnServer);
    }

    private void Start()
    {
        networkManager.Client.Connect("localhost");
    }

    private void OnServerStarted()
    {
        Debug.Log("Server has started. Initializing gameplay systems...");
    }

    private async void OnDisconnected(ClientStoppedReason arg0)
    {
        if (arg0.Equals(ClientStoppedReason.ConnectingTimeout))
        {
            Debug.Log("ConnectingTimeout. start hosting server...");

            await UniTask.WaitUntil(()=>!networkManager.Client.IsConnected);
            networkManager.Server.StartServer(networkManager.Client);
        }
    }

    private void OnClientDisconnectedOnServer(INetworkPlayer networkPlayer)
    {
        networkManager.ServerObjectManager.DestroyCharacter(networkPlayer);
    }

}
