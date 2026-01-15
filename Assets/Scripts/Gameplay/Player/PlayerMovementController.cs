using Mirage;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    public Vector2 Direction => direction;

    [Header("Network")]
    [SerializeField] private NetworkIdentity networkIdentity;

    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody2d;

    [Header("Settings")]
    [SerializeField] private float speed = 500;

    private PlayerInputAction playerInputAction;

    private Vector2 direction;

    private void Awake()
    {
        networkIdentity.OnStartLocalPlayer.AddListener(() => 
        { 
            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
        });
    }

    private void FixedUpdate()
    {
        if (playerInputAction != null)
        {
            direction = playerInputAction.Player.Movement.ReadValue<Vector2>();
            rigidbody2d.linearVelocity = new Vector2( direction.x, direction.y) * speed * Time.fixedDeltaTime;
        }
    }
}
