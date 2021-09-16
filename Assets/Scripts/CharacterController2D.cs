using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerMovement))]
public class CharacterController2D : MonoBehaviour {
	[Header("General")]
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .075f;  	// How much to smooth out the movement
	[SerializeField] private float m_JumpForce = 350f;							    // Amount of force added when the player jumps
	[SerializeField] private bool m_AirControl = true;							    // Can player control character in the air?
	
	[Header("Coliders")]
	[SerializeField] private LayerMask m_WhatIsGround;							    // Mask that determines what is grounded for the player
	[SerializeField] private Transform m_GroundCheck;							    // Position from which we verify if the character touches the ground
	
	[Header("Events")]
	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	
	const float k_GroundedRadius = .2f;												// Radius of the overlap circle to determine if grounded
	private bool m_Grounded;														// Whether or not the player is grounded.
	private bool m_FacingRight = true;												// For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private Rigidbody2D m_Rigidbody2D;
	
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// Check if the player touches the ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{ 
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump)
	{
		// Control the player if grounded or air control is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// Control the player facing
			if (move > 0f && !m_FacingRight)
				Flip();
			else if (move < 0f && m_FacingRight)
				Flip();
		}
		
		// If the player can jump...
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}
	
	// Control the player facing
	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		
		Vector3 facingScale = transform.localScale;
		facingScale.x *= -1;
		transform.localScale = facingScale;
	}
}