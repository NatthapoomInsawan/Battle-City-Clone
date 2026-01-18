
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    public interface IStaggerable 
    {
        public void Stagger(Vector2 direction, float knockbackRate, float knockbackDuration);
    }
}
