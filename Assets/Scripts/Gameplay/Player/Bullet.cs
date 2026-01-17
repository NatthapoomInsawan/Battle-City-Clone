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
        [SerializeField] private int damage = 10;
        [SerializeField] private float speed = 500f;
        [SerializeField] private float lifeTimeInSeconds = 3.5f;

        private Transform ignoreCollisionTransform;

        public void Init(Transform ignoreCollisionTransform)
        {
            this.ignoreCollisionTransform = ignoreCollisionTransform;
        }

        private async void Awake()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(lifeTimeInSeconds));

            DestroyBullet();
        }

        private void FixedUpdate()
        {
            rigidbody2d.linearVelocity = transform.up * speed * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform == ignoreCollisionTransform)
                return;

            IDamagable damagable = collision.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(damage);

            DestroyBullet();
        }

        public void DestroyBullet()
        {
            if (Server && gameObject)
                GameplayManager.Instance.GameplayNetworkManager.ServerObjectManager.Destroy(gameObject);
        }

    }

}
