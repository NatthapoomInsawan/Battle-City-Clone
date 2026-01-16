using BattleCityClone.Gameplay.Manager;
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

        private void OnShootButton(InputAction.CallbackContext callBack) => SpawnShootBulletRpc();

        [ServerRpc]
        private void SpawnShootBulletRpc()
        {
            Bullet bullet = Instantiate(bulletPrefab, spawnTransform.position, transform.rotation);
            GameplayManager.Instance.GameplayNetworkManager.ServerObjectManager.Spawn(bullet.gameObject);
        }

    }
}
