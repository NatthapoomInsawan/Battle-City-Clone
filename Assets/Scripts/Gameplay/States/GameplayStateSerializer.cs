using Mirage.Serialization;
using System;

namespace BattleCityClone.Gameplay
{
    public static class GameplayStateSerializer 
    {
        const byte WAIT_FOR_PLAYER = 1;
        const byte GAME_STARTED = 2;

        public static void WriteItem(this NetworkWriter writer, GameplayState state)
        {
            switch (state)
            {
                case GameplayWaitForPlayerState waitForplayerState:
                    writer.WriteByte(WAIT_FOR_PLAYER);
                    break;
                case GameplayStartedState startedState:
                    writer.WriteByte(GAME_STARTED);
                    break;
            }
        }

        public static GameplayState ReadItem(this NetworkReader reader)
        {
            byte type = reader.ReadByte();
            switch (type)
            {
                case WAIT_FOR_PLAYER:
                    return new GameplayWaitForPlayerState();
                case GAME_STARTED:  
                    return new GameplayStartedState();
                default:
                    throw new Exception($"Invalid state type {type}");
            }
        }
    }
}
