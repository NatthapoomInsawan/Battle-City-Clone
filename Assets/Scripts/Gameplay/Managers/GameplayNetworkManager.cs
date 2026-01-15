using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

public class GameplayNetworkManager : NetworkManager
{
    private string address = "localhost";
    private ushort port = 7777;

    private bool isInit;
    private void Awake()
    {
        Server.Started.AddListener(OnServerStarted);
        Client.Disconnected.AddListener(OnDisconnected);
    }

    public async UniTask Init()
    {
        Client.Connect(address, port);

        if (await UniTask.WaitUntil(() => Client.IsConnected).SuppressCancellationThrow())
            return;

        if (await UniTask.WaitUntil(() => isInit).SuppressCancellationThrow())
            return;
    }


    private void OnServerStarted()
    {
        Debug.Log("Server has started..");
        isInit = true;
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
