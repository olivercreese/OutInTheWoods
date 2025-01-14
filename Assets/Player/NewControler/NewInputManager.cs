using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class NewInputManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }
    public bool Flashlight { get; private set; }

    private InputActionMap _currentActionMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction FlashlightAction;

    private bool Flash;

    private void Awake()
    {
        Flash = false;
        HideCursor();
        _currentActionMap = playerInput.currentActionMap;
        moveAction = _currentActionMap.FindAction("Move");
        lookAction = _currentActionMap.FindAction("Look");
        runAction = _currentActionMap.FindAction("Run");
        jumpAction = _currentActionMap.FindAction("Jump");
        crouchAction = _currentActionMap.FindAction("Crouch");
        FlashlightAction = _currentActionMap.FindAction("Flashlight");

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;
        runAction.performed += OnRun;
        jumpAction.performed += OnJump;
        crouchAction.performed += OnCrouch;
        FlashlightAction.performed += OnFlashlight;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        runAction.canceled += OnRun;
        jumpAction.canceled += OnJump;
        crouchAction.canceled += OnCrouch;
        FlashlightAction.canceled += OnFlashlight;

    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    private void OnFlashlight(InputAction.CallbackContext context)
    {
        Flashlight = context.ReadValueAsButton();
        Light light = GameObject.Find("Flashlight").GetComponent<Light>();
        if (Flash == false)
        {
            Flash = true;
            light.enabled = !light.enabled;
        }
        else
        {
            Flash = false;
        }
     
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }
    private void OnCrouch(InputAction.CallbackContext context)
    {
        Crouch = context.ReadValueAsButton();
    }
    private void OnEnable()
    {
        _currentActionMap.Enable();
    }

    private void OnDisable()
    {
        _currentActionMap.Disable();
    }



}
