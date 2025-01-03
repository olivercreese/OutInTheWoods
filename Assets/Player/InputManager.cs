using UnityEngine;

public class InputManager : MonoBehaviour {

    [SerializeField] Movement movement;
    [SerializeField] MouseLook mouseLook;

    PlayerControls playerControls;
    PlayerControls.GroundMovementActions groundMovement;

    Vector2 horizontalInput;
    Vector2 mouseInput;

    private void Awake()
    {
        playerControls = new PlayerControls();
        groundMovement = playerControls.GroundMovement;

        groundMovement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

        groundMovement.Jump.performed += _ => movement.OnJumpPressed();

        groundMovement.Crouch.performed += _ => movement.OnCrouchPressed();
        

        groundMovement.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        groundMovement.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }

    private void Update()
    {
        movement.ReceiveInput(horizontalInput);
        mouseLook.ReceiveInput(mouseInput);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
