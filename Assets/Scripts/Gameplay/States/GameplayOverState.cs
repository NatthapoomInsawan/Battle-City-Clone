namespace BattleCityClone.Gameplay
{
    public class GameplayOverState : GameplayState
    {
        public override GameplayState GetNextState() => new GameplayStartedState();
    }
}
