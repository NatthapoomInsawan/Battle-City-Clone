using Mirage;
using System;

namespace BattleCityClone.Gameplay.Manager
{
    public class GameplayRpcManager : NetworkBehaviour
    {
        public event Action<INetworkPlayer> OnPlayerDisconnectedEvent;
        public event Action<uint> OnrequestStartEvent;
        public event Action<uint> OnRequestCancelStartEvent;

        private void Awake()
        {
            Identity.OnAuthorityChanged.AddListener(OnAuthorityChanged);
            GameplayManager.Instance.BindRpcManagerEvent(this);
        }

        private void OnAuthorityChanged(bool changed)
        {
            if (changed)
                GameplayManager.Instance.AssignRpcManager(this);
        }


        [ClientRpc]
        public void PlayerDisconnectedRpc(INetworkPlayer disconnectedPlayer) => OnPlayerDisconnectedEvent?.Invoke(disconnectedPlayer);

        [ServerRpc]
        public void RequestRestartGameRpc(uint netId) => OnrequestStartEvent?.Invoke(netId);

        [ServerRpc]
        public  void RequestCancelStartGameRpc(uint netId) => OnRequestCancelStartEvent?.Invoke(netId);
    }
}
