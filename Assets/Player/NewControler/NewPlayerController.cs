using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.SearchService;
using Unity.VisualScripting;

//https://www.youtube.com/watch?v=xWHsS7ju3m8 // tutorial link for the controller

public class NewPlayerController : Entity
{
    [SerializeField] private float animBlendSpeed = 8.9f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform Camera;
    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float LowerLimit = 70f;
    [SerializeField] private float MouseSensitivity = 21.9f;
    [SerializeField, Range(10, 5000)] private float jumpFactor = 260f;
    [SerializeField] private float Distance2Ground = 0.89f;
    [SerializeField] private LayerMask groundCheck;
    [SerializeField] private float AirResistance = 0.8f;
    [SerializeField] private Transform SpineIK;
    private Rigidbody rb;
    private NewInputManager inputManager;
    private GameObject pistol; 

    private Animator animator;
    private bool grounded;
    private bool hasAnimator;
    public bool isDead;
    private bool hasPistol;

    private int _xVelHash; // Hash for the velocity parameters in the animator
    private int _yVelHash;

    private int jumpHash;
    private int groundHash;
    private int fallingHash;
    private int crouchHash;
    private float xRotation;

    private const float walkSpeed = 2.0f;
    private const float runSpeed = 6.0f;
    private float AirTime;

    private Vector2 currentVelocity;

    private void Start()
    {
        hasAnimator = TryGetComponent<Animator>(out animator);
        rb = GetComponent<Rigidbody>();
        inputManager = GetComponent<NewInputManager>();
        maxHealth = 100;
        currentHealth = maxHealth;
        isDead = false;
        _xVelHash = Animator.StringToHash("Xvelocity");
        _yVelHash = Animator.StringToHash("Yvelocity");
        jumpHash = Animator.StringToHash("Jump");
        groundHash = Animator.StringToHash("Grounded");
        fallingHash = Animator.StringToHash("Falling");
        crouchHash = Animator.StringToHash("Crouch");
        hasPistol = false;
        pistol = GameObject.FindGameObjectWithTag("Pistol");
    }



    private void FixedUpdate()
    {
        if (currentHealth > 0)
        {
            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
        }
    }

    private void LateUpdate()
    {
        if (currentHealth > 0)
            CameraMovement();
        else
            Camera.position = CameraRoot.position;

        HandlePistol();

    }

    public void PistolSwap()
    {
        if (!hasPistol)
        {
            hasPistol=true;
        }
        else
        {
            hasPistol = false;
        }

    }

    private void HandlePistol()
    {
        if (hasPistol)
        {
            animator.SetBool("hasPistol", true);
            pistol.SetActive(true);
        }
        else
        {
            animator.SetBool("hasPistol", false);
            pistol.SetActive(false);
        }

    }
    private void Move()
    {
        if (!hasAnimator) return;

        float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;

        if (inputManager.Crouch) targetSpeed = 1.5f;

        if (inputManager.Move == Vector2.zero) targetSpeed = 0f;

        if (grounded)
        {
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * targetSpeed, animBlendSpeed * Time.deltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * targetSpeed, animBlendSpeed * Time.deltaTime);

            var xVelDiff = currentVelocity.x - rb.linearVelocity.x;
            var zVelDiff = currentVelocity.y - rb.linearVelocity.z;

            rb.AddForce(transform.TransformVector(new Vector3(xVelDiff, 0, zVelDiff)), ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(transform.TransformVector(new Vector3(currentVelocity.x * AirResistance, 0, currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
        }

        animator.SetFloat(_xVelHash, currentVelocity.x);
        animator.SetFloat(_yVelHash, currentVelocity.y);
    }

    private void CameraMovement()
    {
        if (!hasAnimator) return;

        var mouseX = inputManager.Look.x;
        var mouseY = inputManager.Look.y;

        xRotation -= mouseY * MouseSensitivity * Time.smoothDeltaTime;
        xRotation = Mathf.Clamp(xRotation, UpperLimit, LowerLimit);
        Camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        if (hasPistol) SpineIK.localRotation = Quaternion.Euler(xRotation, Camera.localRotation.y, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX * MouseSensitivity * Time.smoothDeltaTime, 0));
        Camera.position = CameraRoot.position;

    }

    private void HandleJump()
    {
        if (!hasAnimator) return;
        if (!inputManager.Jump) return;
        animator.SetTrigger(jumpHash);
    }

    private void HandleCrouch()
    {
        animator.SetBool(crouchHash, inputManager.Crouch);
    }

    public void JumpAddForce()
    {
        rb.AddForce(-rb.linearVelocity.y * Vector3.up, ForceMode.VelocityChange);
        rb.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
        animator.ResetTrigger(jumpHash);
    }

    private void SampleGround()
    {
        if (!hasAnimator) return;
        RaycastHit hitInfo;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Vector3.down, out hitInfo, Distance2Ground + 0.1f, groundCheck))
        {
            //grounded
            grounded = true;
            SetAnimationGrounding();
            return;
        }
        //falling
        grounded = false;
        AirTime += Time.deltaTime;
        rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration); // increase gravity
        if (AirTime > 1) SetAnimationGrounding();
        return;

    }

    private void SetAnimationGrounding()
    {
        animator.SetBool(fallingHash, !grounded);
        animator.SetBool(groundHash, grounded);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && currentHealth > 0 )
        {
            TakeDamage(25);
            if (currentHealth <= 0)
            {
                animator.SetTrigger("Death");
                isDead = true;
            }
        }
    }
    
}

 


