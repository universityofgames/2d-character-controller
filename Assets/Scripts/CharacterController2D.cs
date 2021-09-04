using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour {
    [SerializeField, Tooltip("Player's maximum speed in seconds")]
    float speed = 5;
    
    [SerializeField, Tooltip("The player's maximum jump height, not including gravity")]
    float jumpHeight = 4;
    
    [SerializeField, Tooltip("Acceleration of the player on the ground")]
    float walkAcceleration = 10;

    [SerializeField, Tooltip("Acceleration of the player in the air")]
    float airAcceleration = 15;

    [SerializeField, Tooltip("Slowing the player down when he stops moving on the ground")]
    float groundDeceleration = 25;
    
    private BoxCollider2D boxCollider;
    private Vector2 velocity;
    private bool grounded;

    private void Awake() { 
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()  {
        // Manage player if it touches the ground
        IsGrounded();

        // Set the correct values for the appropriate case
        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        float moveInput = Input.GetAxisRaw("Horizontal");
        velocity.x = (moveInput != 0) 
            ? Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime)
            : Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);

        velocity.y += Physics2D.gravity.y * Time.deltaTime;
        transform.Translate(velocity * Time.deltaTime);

        // Check if player has touched the ground
        IsTouchGround();
    }

    private void IsGrounded()
    {
        if (grounded) {
            velocity.y = 0;

            if (Input.GetButtonDown("Jump")) {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }
    }

    private void IsTouchGround()
    {
        // Retrieve all colliders to check that the player has touched the ground
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        grounded = false;
        
        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Check if we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, it means that we have touched the ground
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                    grounded = true;
            }
        }
    }
}
