using BattleCityClone.Gameplay.Manager;
using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    public class Bullet : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rigidbody2d;

        [Header("Setttings")]
        [SerializeField] private float speed = 500f;

        private void FixedUpdate()
        {
            rigidbody2d.linearVelocity = transform.up * speed * Time.fixedDeltaTime;
        }
    }

}
