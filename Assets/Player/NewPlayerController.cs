using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using OITW.Manager;
using UnityEngine.Rendering;
using UnityEditor.SearchService;
using Unity.VisualScripting;

namespace OITW.Player //https://www.youtube.com/watch?v=xWHsS7ju3m8
{
    public class NewPlayerController : MonoBehaviour
    {
        [SerializeField] private float animBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float LowerLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField,Range(10,5000)] private float jumpFactor = 260f;
        [SerializeField] private float Distance2Ground = 0.89f;
        [SerializeField] private LayerMask groundCheck;
        private Rigidbody rb;
        private NewInputManager inputManager;

        public CapsuleCollider capsuleCollider;
        private Animator animator;
        private bool grounded;
        private bool hasAnimator;

        private int _xVelHash; // Hash for the velocity parameters in the animator
        private int _yVelHash;

        private int jumpHash;
        private int groundHash;
        private int fallingHash;

        private float xRotation;

        private const float walkSpeed = 2.0f;
        private const float runSpeed = 6.0f;

        private Vector2 currentVelocity;

        private void Start()
        {
            hasAnimator = TryGetComponent<Animator>(out animator);
            rb = GetComponent<Rigidbody>();
            inputManager = GetComponent<NewInputManager>();

            _xVelHash = Animator.StringToHash("Xvelocity");
            _yVelHash = Animator.StringToHash("Yvelocity");
            jumpHash = Animator.StringToHash("Jump");
            groundHash = Animator.StringToHash("Grounded");
            fallingHash = Animator.StringToHash("Falling");
            
        }

        private void Update()
        {
            SampleGround();
        }

        private void FixedUpdate()
        {
            Move();
            HandleJump();
        }

        private void LateUpdate()
        {
            CameraMovement();
        }
        private void Move()
        {
            if(!hasAnimator) return;      
            
            float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;

            if(inputManager.Move == Vector2.zero) targetSpeed = 0f;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x,inputManager.Move.x * targetSpeed,animBlendSpeed * Time.deltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y,inputManager.Move.y * targetSpeed,animBlendSpeed * Time.deltaTime);

            var xVelDiff = currentVelocity.x - rb.linearVelocity.x;
            var zVelDiff = currentVelocity.y - rb.linearVelocity.z;

            rb.AddForce(transform.TransformVector(new Vector3(xVelDiff,0,zVelDiff)),ForceMode.VelocityChange);

            animator.SetFloat(_xVelHash, currentVelocity.x);
            animator.SetFloat(_yVelHash, currentVelocity.y);
        }

        private void CameraMovement()
        {
            if (!hasAnimator) return;
            
            var mouseX = inputManager.Look.x;
            var mouseY = inputManager.Look.y;
            Camera.position = CameraRoot.position;

            xRotation -= mouseY * MouseSensitivity * Time.smoothDeltaTime;
            xRotation = Mathf.Clamp(xRotation, UpperLimit, LowerLimit);
            Camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleJump()
        {
            if (!hasAnimator) return;
            if(!inputManager.Jump) return;
            animator.SetTrigger(jumpHash);

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

            if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 0.3f,transform.position.z), Vector3.down, out hitInfo, Distance2Ground + 0.1f, groundCheck))
            {
                //grounded
                grounded = true;
                SetAnimationGrounding();
                return;
            }
            //falling
            Debug.Log(grounded);
            grounded = false;
            SetAnimationGrounding();
            return;

        }

        private void SetAnimationGrounding()
        {
            animator.SetBool( fallingHash,!grounded);
            animator.SetBool( groundHash, grounded);
        }







    }

 

}
