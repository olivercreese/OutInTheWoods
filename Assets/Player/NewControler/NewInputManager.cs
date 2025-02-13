using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

//https://www.youtube.com/watch?v=xWHsS7ju3m8 // tutorial link for the controller
//This class is responsible for managing the player input 
//code has been taken from the tutorial above and expanded upon for different actions

public class NewInputManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] AudioClip flashOn;
    [SerializeField] AudioClip flashOff;
    [SerializeField] AudioSource flashLightSource;
    [SerializeField] Weapon weapon;

    public Vector2 Move { get; private set; } // movement values from the input system
    public Vector2 Look { get; private set; } 
    public bool Run { get; private set; } 
    public bool Jump { get; private set; }
    public bool Crouch { get; private set; }
    public bool Flashlight { get; private set; }
    public bool Fire { get; private set; }
    public bool Reload { get; private set; }
    public bool Aim { get; private set; }

    private InputActionMap _currentActionMap; // current action map for the player input
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction FlashlightAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private Light light; // flashlight light
    private AudioManager audioManager;

    private bool Flash; // flashlight on or off
    public bool isAiming; // aiming or not

    private void Awake()
    {
        Flash = false;
        HideCursor(); 
        _currentActionMap = playerInput.currentActionMap; 
        moveAction = _currentActionMap.FindAction("Move"); // find the actions in the action map
        lookAction = _currentActionMap.FindAction("Look");
        runAction = _currentActionMap.FindAction("Run");
        jumpAction = _currentActionMap.FindAction("Jump");
        crouchAction = _currentActionMap.FindAction("Crouch");
        FlashlightAction = _currentActionMap.FindAction("Flashlight");
        fireAction = _currentActionMap.FindAction("Fire");
        aimAction = _currentActionMap.FindAction("Aim");

        moveAction.performed += OnMove; // add the performed and canceled events to the actions
        lookAction.performed += OnLook;
        runAction.performed += OnRun;
        jumpAction.performed += OnJump;
        crouchAction.performed += OnCrouch;
        FlashlightAction.performed += OnFlashlight;
        fireAction.performed += OnFire;
        aimAction.performed += OnAim;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        runAction.canceled += OnRun;
        jumpAction.canceled += OnJump;
        crouchAction.canceled += OnCrouch;
        FlashlightAction.canceled += OnFlashlight;
        fireAction.canceled += OnFire;
        aimAction.canceled += OnAim;
        light = GameObject.Find("Flashlight").GetComponent<Light>();
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void OnMove(InputAction.CallbackContext context) 
    {
        Move = context.ReadValue<Vector2>(); // reads the value of the input and assigns it to the move vector
    }
    private void OnAim(InputAction.CallbackContext context)
    {
        //Aim = context.ReadValueAsButton();
        if (Aim == true)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>(); 
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        Fire = context.ReadValueAsButton();
        //weapon.Shoot();
    }

    private void OnFlashlight(InputAction.CallbackContext context)
    {
        Flashlight = context.ReadValueAsButton(); 
        if (Flash == false)
        {
            Flash = true;
            light.enabled = !light.enabled;
            audioManager.PlaySFX(flashOn, flashLightSource);
        }
        else
        {
            Flash = false;
            audioManager.PlaySFX(flashOff, flashLightSource);
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
