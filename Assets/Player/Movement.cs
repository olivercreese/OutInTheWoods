using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour {

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 11f;
    Vector2 horizontalinput;


    [SerializeField] float jumpHeight = 3.5f;
    bool jump;
    bool crouch;
    [SerializeField] Transform camera;

    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = Vector3.zero;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    private void Update()
    {
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - controller.height / 2 , transform.position.z), 0.3f, groundMask);

        if (isGrounded)
        {
            verticalVelocity.y = 0f;
        }
        Vector3 horizontalVelocity = (transform.right * horizontalinput.x + transform.forward * horizontalinput.y) * speed;
        controller.Move(horizontalVelocity * Time.deltaTime);

        if (jump)
        {
            if (isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
                Debug.Log("jumping");
            }
            jump = false;
        }

    
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - controller.height / 2, transform.position.z), 0.3f);
    }


    public void ReceiveInput(Vector2 Hinput)
    {
        horizontalinput = Hinput;
    }

    public void OnJumpPressed()
    {
        jump = true;
    }

    public void OnCrouchPressed()
    {
        crouch = true;
    }
}
