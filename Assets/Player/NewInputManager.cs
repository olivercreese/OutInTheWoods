using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace OITW.Manager
{
    public class NewInputManager : MonoBehaviour
    {

        [SerializeField] PlayerInput playerInput;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }

        private InputActionMap _currentActionMap;

        private InputAction moveAction;

        private InputAction lookAction;

        private InputAction runAction;

        private void Awake()
        {
            _currentActionMap = playerInput.currentActionMap;
            moveAction = _currentActionMap.FindAction("Move");
            lookAction = _currentActionMap.FindAction("Look");
            runAction = _currentActionMap.FindAction("Run");

            moveAction.performed += OnMove;
            lookAction.performed += OnLook;
            runAction.performed += OnRun;

            moveAction.canceled += OnMove;
            lookAction.canceled += OnLook;
            runAction.canceled += OnRun;

        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
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
}