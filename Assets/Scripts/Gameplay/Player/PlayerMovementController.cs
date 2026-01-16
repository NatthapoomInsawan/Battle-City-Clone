using BattleCityClone.Gameplay.Manager;
using Mirage;
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

    public class PlayerMovementController : NetworkBehaviour
    {
        public FacingDirection FacingDirection => facingDirection;

        [Header("Network")]
        [SerializeField] private NetworkIdentity networkIdentity;

        [Header("References")]
        [SerializeField] private Rigidbody2D rigidbody2d;

        [Header("Settings")]
        [SerializeField] private float speed = 500;
        [SerializeField] private FacingDirection facingDirection = FacingDirection.Up;

        private PlayerInputAction playerInputAction;

        private void Awake()
        {
            networkIdentity.OnStartLocalPlayer.AddListener(() =>
            {
                playerInputAction = new PlayerInputAction();
                playerInputAction.Enable();

                playerInputAction.Player.Movement.performed += OnMovementPerform;
            });
        }

        private void FixedUpdate()
        {
            if (playerInputAction != null)
            {
                var directionVector = GetSnapDirectionVector(playerInputAction.Player.Movement.ReadValue<Vector2>());
                rigidbody2d.linearVelocity = new Vector2(directionVector.x, directionVector.y) * speed * Time.fixedDeltaTime;
            }
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
    }

}
