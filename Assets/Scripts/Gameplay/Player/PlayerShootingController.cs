using BattleCityClone.Gameplay.Manager;
using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCityClone.Gameplay.Player
{
    public class PlayerShootingController : NetworkBehaviour
    {
        [Header("Network")]
        [SerializeField] private NetworkIdentity networkIdentity;

        [Header("Prefabs")]
        [SerializeField] private Bullet bulletPrefab;

        [Header("References")]
        [SerializeField] private Transform spawnTransform;

        [Header("Settings")]
        [SerializeField] private float shootCooldownInSeconds = 0.5f;

        private bool canShoot = true;

        private PlayerInputAction playerInputAction;

        private void Awake()
        {
            networkIdentity.OnStartLocalPlayer.AddListener(() =>
            {
                playerInputAction = new PlayerInputAction();
                playerInputAction.Player.Enable();

                playerInputAction.Player.Shooting.performed += OnShootButton;
            });
        }

        private async void OnShootButton(InputAction.CallbackContext callBack) 
        {
            if (!canShoot)
                return;

            canShoot = false;

            SpawnShootBulletRpc(NetId);

            await UniTask.Delay(System.TimeSpan.FromSeconds(shootCooldownInSeconds));

            canShoot = true;
        }

        [ServerRpc]
        private void SpawnShootBulletRpc(uint netId)
        {
            Bullet bullet = Instantiate(bulletPrefab, spawnTransform.position, transform.rotation);
            bullet.Init(netId);
            
            GameplayManager.Instance.GameplayNetworkManager.ServerObjectManager.Spawn(bullet.gameObject);
        }

        private void OnDisable()
        {
            if (playerInputAction != null)
            {
                playerInputAction.Player.Shooting.performed -= OnShootButton;
                playerInputAction.Player.Disable();
            }
        }

    }
}
