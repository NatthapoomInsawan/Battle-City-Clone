using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay.Player;
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
        [SerializeField] private float knockbackRate = 300f;
        [SerializeField] private float knockbackDuration = 1f;

        [SerializeField] private float bulletSpeed = 500f;
        [SerializeField] private float lifeTimeInSeconds = 3.5f;

        [SyncVar] private uint ownerId;

        public void Init(uint ownerId)
        {
            this.ownerId = ownerId;
        }

        private async void Awake()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(lifeTimeInSeconds));

            DestroyBullet();
        }

        private void FixedUpdate()
        {
            rigidbody2d.linearVelocity = transform.up * bulletSpeed * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<NetworkIdentity>(out var collideIdentity))
            {
                if (collideIdentity.NetId == ownerId)
                    return;
            }
            
            if (collision.TryGetComponent<PlayerStateController>(out var playerStateController))
            {
                if (playerStateController.CurrentState is PlayerInvincibleState)
                    return;
            }

            IDamagable damagable = collision.GetComponent<IDamagable>();
            IStaggerable staggerable = collision.GetComponent<IStaggerable>();

            if (staggerable != null)
                staggerable.Stagger(transform.up.normalized, knockbackRate, knockbackDuration);

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
