using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.Rendering;

//https://www.youtube.com/watch?v=xWHsS7ju3m8 // tutorial link for the controller

public class NewPlayerController : Entity
{
    [SerializeField] private float animBlendSpeed = 8.9f; // animation blend speed
    [SerializeField] private Transform CameraRoot; // camera root for the camera position
    [SerializeField] private Transform Camera;
    [SerializeField] private float UpperLimit = -40f; // upper and lower limits for the camera rotation
    [SerializeField] private float LowerLimit = 70f; 
    [SerializeField] private float MouseSensitivity = 21.9f; 
    [SerializeField, Range(10, 5000)] private float jumpFactor = 260f; // jump height
    [SerializeField] private float Distance2Ground = 0.89f; // distance to the ground for ground check
    [SerializeField] private LayerMask groundCheck; // ground check layer mask
    [SerializeField] private float AirResistance = 0.8f; 
    [SerializeField] private Transform SpineIK;
    private Rigidbody rb;
    private NewInputManager inputManager;
    private GameObject pistol; 

    private Animator animator;
    private bool grounded;
    private bool hasAnimator;
    public bool isDead;

    private int _xVelHash; // Hash for the velocity parameters in the animator
    private int _yVelHash; 

    private int jumpHash; // hash for animation triggers
    private int groundHash; 
    private int fallingHash;
    private int crouchHash;
    private int aimHash;
    private float xRotation; // x rotation for the camera

    private const float walkSpeed = 2.0f;
    private const float runSpeed = 6.0f;
    private float healTimer;
    private float AirTime;

    private Vector2 currentVelocity;

    private void Start()
    {
        hasAnimator = TryGetComponent<Animator>(out animator); // check if the animator is present
        rb = GetComponent<Rigidbody>(); 
        inputManager = GetComponent<NewInputManager>();
        maxHealth = 100;
        currentHealth = maxHealth;
        isDead = false;
        _xVelHash = Animator.StringToHash("Xvelocity"); // get the hash for the blend tree parameters
        _yVelHash = Animator.StringToHash("Yvelocity");
        jumpHash = Animator.StringToHash("Jump"); // get teh has for the animation triggers
        groundHash = Animator.StringToHash("Grounded");
        fallingHash = Animator.StringToHash("Falling");
        crouchHash = Animator.StringToHash("Crouch");
        aimHash = Animator.StringToHash("Aim");
        pistol = GameObject.FindGameObjectWithTag("Pistol");
    }



    private void FixedUpdate()
    {
        if (currentHealth > 0) // if the player is alive movement updates are made
        {
            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
            HandlePistol();
        }
    }

    private void LateUpdate()
    {
        if (currentHealth > 0)
            CameraMovement(); // camera movement is updated in the late update to avoid jitter
        else
            Camera.position = CameraRoot.position;

        AutoHeal();

    }

    

    private void AutoHeal() // automatically heals the player after 10 seconds of not taking damage
    {
        if (currentHealth >= maxHealth) return;
        healTimer += Time.deltaTime;
        if (healTimer >= 10)
        {
            Heal(0.5f);
        }
    }

    private void HandlePistol() //future implementation for pistol aniamtion layer
    {
        animator.SetBool(aimHash, inputManager.Aim); // set the crouch parameter in the animator

    }
    private void Move()
    {
        if (!hasAnimator) return;

        float targetSpeed = inputManager.Run ? runSpeed : walkSpeed; // check if the player is running or walking and set the target speed accordingly

        if (inputManager.Crouch) targetSpeed = 1.5f; 

        if (inputManager.Move == Vector2.zero) targetSpeed = 0f; // if the player is not moving set the target speed to 0

        if (grounded)
        {
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * targetSpeed, animBlendSpeed * Time.deltaTime); // lerp the velocity to the target speed for smooth movement
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * targetSpeed, animBlendSpeed * Time.deltaTime);

            var xVelDiff = currentVelocity.x - rb.linearVelocity.x; // calculate the difference between the current velocity and the rigidbody velocity
            var zVelDiff = currentVelocity.y - rb.linearVelocity.z;

            rb.AddForce(transform.TransformVector(new Vector3(xVelDiff, 0, zVelDiff)), ForceMode.VelocityChange); // add force to the rigidbody to move the player in the direction of the velocity
        }
        else
        {
            rb.AddForce(transform.TransformVector(new Vector3(currentVelocity.x * AirResistance, 0, currentVelocity.y * AirResistance)), ForceMode.VelocityChange); // add air resistance to the player when in the air
        }

        animator.SetFloat(_xVelHash, currentVelocity.x); // set the velocity parameters in the animator to the current velocity
        animator.SetFloat(_yVelHash, currentVelocity.y);
    }

    private void CameraMovement()
    {
        if (!hasAnimator) return;

        var mouseX = inputManager.Look.x; // get the mouse input
        var mouseY = inputManager.Look.y;

        xRotation -= mouseY * MouseSensitivity * Time.smoothDeltaTime; // rotate the camera based on the mouse vertical input 
        xRotation = Mathf.Clamp(xRotation, UpperLimit, LowerLimit); // clamp the rotation to the upper and lower limits 
        Camera.localRotation = Quaternion.Euler(xRotation, 0, 0); // set the camera rotation to the x rotation
        /*
        if (inputManager.isAiming)
        {
            Camera.rotation = Quaternion.Euler(xRotation, CameraRoot.rotation.eulerAngles.y, CameraRoot.rotation.eulerAngles.z); // lock the camera rotation to the y axis when aiming
            SpineIK.localRotation = Quaternion.Euler(xRotation, 0, 0);
        } //future implementation for pistol aiming
        */
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX * MouseSensitivity * Time.smoothDeltaTime, 0)); // rotate the players y axis based on the mouse horizontal input
        Camera.position = CameraRoot.position;

    }

    private void HandleJump()
    {
        if (!hasAnimator) return;
        if (!inputManager.Jump) return;
        animator.SetTrigger(jumpHash); // set the jump trigger in the animator
    }

    private void HandleCrouch()
    {
        animator.SetBool(crouchHash, inputManager.Crouch); // set the crouch parameter in the animator
    }

    public void JumpAddForce() // animation event for the jump
    {
        rb.AddForce(-rb.linearVelocity.y * Vector3.up, ForceMode.VelocityChange); // cancel out the current y velocity 
        rb.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse); // add the jump force
        animator.ResetTrigger(jumpHash); // reset the jump trigger after the jump
    }

    private void SampleGround() // ground check
    {
        if (!hasAnimator) return;
        RaycastHit hitInfo; 

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Vector3.down, out hitInfo, Distance2Ground, groundCheck))
        {
            //grounded
            grounded = true;
            SetAnimationGrounding();
            return;
        }
        //falling
        grounded = false;
        AirTime += Time.deltaTime; // increase the air time to check if the player is in the air for more than 1 second
        rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration); // increase gravity
        if (AirTime > 1) SetAnimationGrounding();
        return;

    }

    private void SetAnimationGrounding() // set the animation parameters for the ground and falling
    {
        animator.SetBool(fallingHash, !grounded);
        animator.SetBool(groundHash, grounded);
    }

    private void OnTriggerEnter(Collider other) // take damage when the player collides with an enemy
    {
        if (other.tag == "Enemy" && currentHealth > 0 )
        {
            TakeDamage(25); 
            healTimer = 0;
            if (currentHealth <= 0)
            {
                animator.SetTrigger("Death"); 
                isDead = true;
            }
        }
    }
    
}

 


