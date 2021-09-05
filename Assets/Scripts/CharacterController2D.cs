using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour {
    [Range(0, .3f)] [SerializeField, Tooltip("How much to smooth out the movement")] 
    private float m_MovementSmoothing = .05f; 
    
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
    
    [SerializeField] 
    private LayerMask whatIsGround;	
    
    [SerializeField] 
    private Transform groundCheck;
    
    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;
    
    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
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

        if (!grounded)
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        
        transform.Translate(velocity * Time.deltaTime);
    }
    
    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }
    
    private void IsGrounded()
    {
        if (grounded) {
            velocity.y = 0;

            if (Input.GetButtonDown("Jump")) {
                grounded = false;
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }
    }

}
