using Mirage;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkIdentity networkIdentity;

    [Header("Settings")]
    public float speed = 500;
    public Rigidbody2D rigidbody2d;

    private bool controlable;

    private void Awake()
    {
        networkIdentity.OnStartLocalPlayer.AddListener(() => { controlable = true; });
    }

    private void FixedUpdate()
    {
        if (controlable)
            rigidbody2d.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
    }
}
