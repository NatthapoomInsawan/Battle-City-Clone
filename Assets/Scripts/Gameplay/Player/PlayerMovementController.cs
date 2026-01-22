using BattleCityClone.Gameplay.Manager;
using Cysharp.Threading.Tasks;
using Mirage;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCityClone.Gameplay.Player
{
    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class PlayerMovementController : NetworkBehaviour, IStaggerable
    {
        public FacingDirection FacingDirection => facingDirection;

        [Header("Network")]
        [SerializeField] private NetworkIdentity networkIdentity;
        [SerializeField] private PlayerStateController playerStateController;

        [Header("References")]
        [SerializeField] private Rigidbody2D rigidbody2d;
        
        [Header("Settings")]
        [SerializeField] private float speed = 500;
        [SerializeField] private FacingDirection facingDirection = FacingDirection.Up;

        private bool isKnockingBack;

        private Vector2 directionVector;

        private PlayerInputAction playerInputAction;

        private void Awake()
        {
            playerInputAction = new PlayerInputAction();

            networkIdentity.OnStartLocalPlayer.AddListener(() =>
            {
                playerInputAction.Player.Movement.performed += OnMovementPerform;
                playerStateController.OnPlayerStateChanged += SetInputActiveByState;
            });

            GameplayManager.Instance.GameplayStateManager.OnStateChanged += (gameState) =>
            {
                if (gameState is GameplayStartedState)
                    SetInputActiveByState(playerStateController.CurrentState);
                else
                    playerInputAction.Disable();
            };

        }

        private void SetInputActiveByState(PlayerState playerState)
        {
            if (playerState is PlayerDeadState)
                playerInputAction.Disable();
            else
                playerInputAction.Enable();
        }

        private void FixedUpdate()
        {
            if (HasAuthority && playerInputAction != null && !isKnockingBack)
            {
                directionVector = GetSnapDirectionVector(playerInputAction.Player.Movement.ReadValue<Vector2>());
                rigidbody2d.linearVelocity = new Vector2(directionVector.x, directionVector.y) * speed;
            }

            rigidbody2d.linearVelocity = rigidbody2d.linearVelocity * Time.fixedDeltaTime;
        }

        private  Vector2 GetSnapDirectionVector(Vector2 directionVector)
        {
            switch (directionVector)
            {
                case Vector2 v when v == Vector2.up:
                    return Vector2.up;
                case Vector2 v when v == Vector2.down:
                    return Vector2.down;
                case Vector2 v when v == Vector2.left:
                    return Vector2.left;
                case Vector2 v when v == Vector2.right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }

        private void OnMovementPerform(InputAction.CallbackContext callBack)
        {
            switch (playerInputAction.Player.Movement.ReadValue<Vector2>())
            {
                case Vector2 v when v == Vector2.up:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    facingDirection = FacingDirection.Up;
                    break;
                case Vector2 v when v == Vector2.down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    facingDirection = FacingDirection.Down;
                    break;
                case Vector2 v when v == Vector2.left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    facingDirection = FacingDirection.Left;
                    break;
                case Vector2 v when v == Vector2.right:
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    facingDirection = FacingDirection.Right;
                    break;
            }
        }

        public void Stagger(Vector2 direction, float knockbackRate, float knockbackDuration)
        {
            if (isKnockingBack)
                return;

            if (IsServer)
                SendKnockBackRpc(direction, knockbackRate, knockbackDuration);

            rigidbody2d.linearVelocity = direction * knockbackRate;
            KnockBackTimeTask(knockbackDuration).Forget();
        }

        private async UniTaskVoid KnockBackTimeTask(float knockbackDuration)
        {
            isKnockingBack = true;
            await UniTask.Delay(TimeSpan.FromSeconds(knockbackDuration));
            isKnockingBack = false;
        }

        [ClientRpc(excludeHost = true)]
        private void SendKnockBackRpc(Vector2 direction, float knockbackRate, float knockbackDuration)
        {
            Stagger(direction, knockbackRate, knockbackDuration);
        }

        private void OnDisable()
        {
            if (playerInputAction != null)
                playerInputAction.Player.Disable();
        }

    }

}
