
using BattleCityClone.Gameplay.Manager;

namespace BattleCityClone.Gameplay
{
    public class GameplayOverState : GameplayState
    {
        public override void EnterState()
        {
            foreach (var player in GameplayManager.Instance.GameplayNetworkManager.Server.AllPlayers)
                player.Identity.gameObject.SetActive(true);
        }
        public override GameplayState GetNextState() => new GameplayStartedState();
    }
}
