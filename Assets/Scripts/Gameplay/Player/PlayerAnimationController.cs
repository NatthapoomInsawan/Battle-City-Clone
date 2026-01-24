using Mirage;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCityClone
{
    public class PlayerAnimationController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        private PlayerInputAction playerInputAction;

        private bool isMoving;

        private void Awake()
        {
            playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            
            playerInputAction.Player.Movement.canceled += OnStopMoving;
            playerInputAction.Player.Movement.performed += OnStartMoving;
        }

        private void OnStopMoving(InputAction.CallbackContext callbackContext)
        {
            isMoving = false;
            UpdateIsMoving();
        }

        private void OnStartMoving(InputAction.CallbackContext callbackContext)
        {
            isMoving = true;
            UpdateIsMoving();
        }

        public void SetInvincible(bool value)
        {
            animator.SetBool("isInvincible", value);
        }


        private void UpdateIsMoving()
        {
            if (Identity.HasAuthority)
                animator.SetBool("isMoving", isMoving);
        }

    }
}
