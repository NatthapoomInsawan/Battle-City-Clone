using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay.Player;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

namespace BattleCityClone.Gameplay
{
    [Serializable]
    public class GameplayStartedState : GameplayState
    {
        public override async void EnterState()
        {
            foreach (var player in GameplayManager.Instance.GameplayNetworkManager.Server.AllPlayers)
                player.Identity.gameObject.GetComponent<PlayerStateController>().Init();

            await UniTask.WaitUntil(() => GameplayManager.Instance.GameplayNetworkManager.Server.AllPlayers.Count(player => player.Identity.gameObject.activeSelf) == 1);

            ExitState();
        }

        public override GameplayState GetNextState() => new GameplayOverState();
    }
}
