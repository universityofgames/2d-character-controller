using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 40f;

    private CharacterController2D m_Controller;
    private Animator m_Animator;
    private float m_HorizontalMove = 0f;
    private bool m_Jump = false;
    
    private void Awake()
    {
        m_Controller = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
    }
    
    private void Update() 
    {
        m_HorizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        m_Animator.SetFloat("Speed", Mathf.Abs(m_HorizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            m_Jump = true;
            m_Animator.SetBool("IsJumping", true);
        }
    }

    private void FixedUpdate()
    {
        m_Controller.Move(m_HorizontalMove * Time.fixedDeltaTime, m_Jump);
        m_Jump = false;
    }
    
    public void OnLanding ()
    {
        m_Animator.SetBool("IsJumping", false);
    }
}
