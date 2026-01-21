using Mirage.Serialization;
using System;

namespace BattleCityClone.Gameplay.Player
{
    public static class PlayerStateSerializer
    {
        const byte IDLE = 1;
        const byte INVINCIBLE = 2;
        const byte DEAD = 3;

        public static void WriteItem(this NetworkWriter writer, PlayerState state)
        {
            switch (state)
            {
                case PlayerIdleState playerIdleState:
                    writer.WriteByte(IDLE);
                    break;
                case PlayerInvincibleState playerInvincibleState:
                    writer.WriteByte(INVINCIBLE);
                    break;
                case PlayerDeadState playerDeadState:
                    writer.WriteByte(DEAD);
                    break;
            }
        }

        public static PlayerState ReadItem(this NetworkReader reader)
        {
            byte type = reader.ReadByte();
            switch (type)
            {
                case IDLE:
                    return new PlayerIdleState();
                case INVINCIBLE:
                    return new PlayerInvincibleState();
                case DEAD:
                    return new PlayerDeadState();
                default:
                    throw new Exception($"Invalid state type {type}");
            }
        }
    }
}
