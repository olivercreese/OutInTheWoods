using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using OITW.Manager;
using UnityEngine.Rendering;

namespace OITW.Player
{
    public class NewPlayerController : MonoBehaviour
    {
        [SerializeField] private float animBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float LowerLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        private Rigidbody rb;

        private NewInputManager inputManager;

        private Animator animator;

        private bool hasAnimator;

        private int _xVelHash; // Hash for the velocity parameters in the animator
        private int _yVelHash;
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
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            CameraMovement();
        }

        private void Move()
        {
            if(!hasAnimator) return;      
            
            float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;

            if(inputManager.Move == Vector2.zero) targetSpeed = 0.1f;

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

            xRotation -= mouseY * MouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, UpperLimit, LowerLimit);
            Camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.Rotate(Vector3.up, mouseX * MouseSensitivity * Time.deltaTime);
        }
    }

}
