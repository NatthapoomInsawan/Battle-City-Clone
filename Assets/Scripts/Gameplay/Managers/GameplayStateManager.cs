using Mirage;
using System;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    public class GameplayStateManager : NetworkBehaviour
    {
        public GameplayState CurrentState => currentState;
        public event Action<GameplayState> OnStateChanged;

        [SerializeReference] private GameplayState currentState;

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
            OnStateChanged?.Invoke(currentState);
        }

        [ClientRpc(excludeHost = true)]
        private void SendClientStateRpc(GameplayState newState)
        {
            SetState(newState);
        }
    }
}
