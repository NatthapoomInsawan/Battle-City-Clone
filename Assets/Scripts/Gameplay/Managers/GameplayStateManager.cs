using BattleCityClone.Gameplay.Player;
using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    public class GameplayStateManager : NetworkBehaviour
    {
        [SyncVar, SerializeReference] private GameplayState currentState;

        public void Init() 
        {
            SetState(new GameplayWaitForPlayerState());
        }

        public void SetState(GameplayState state)
        {
            if (this == null)
                return;

            if (currentState != null)
                currentState.ExitState();

            currentState = state;

            if (IsServer)
                SendClientStateRpc(currentState);

            currentState.EnterState();
        }

        [ClientRpc(excludeHost = true)]
        private void SendClientStateRpc(GameplayState newState)
        {
            SetState(newState);
        }
    }
}
