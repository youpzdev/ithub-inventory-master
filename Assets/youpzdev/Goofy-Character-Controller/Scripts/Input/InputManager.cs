using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;

    // Абстрактный ввод
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Jump { get; private set; }
    public bool Fire { get; private set; }
    public bool Interact { get; private set; }
    public bool Inventory { get; private set; }
    public bool Debug { get; private set; }
    public bool Run { get; private set; }
    public bool Crouch {get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => Move = Vector2.zero;

        inputActions.Player.Look.performed += ctx => Look = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => Look = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump = true;
        inputActions.Player.Jump.canceled += ctx => Jump = false;

        inputActions.Player.Run.performed += ctx => Run = true;
        inputActions.Player.Run.canceled += ctx => Run = false;

        inputActions.Player.Crouch.performed += ctx => Crouch = true;
        inputActions.Player.Crouch.canceled += ctx => Crouch = false;

        inputActions.Player.Fire.performed += ctx => Fire = true;
        inputActions.Player.Fire.canceled += ctx => Fire = false;

        inputActions.Player.Interact.performed += ctx => Interact = true;
        inputActions.Player.Interact.canceled += ctx => Interact = false;
        
        inputActions.Player.Inventory.performed += ctx => Inventory = true;
        inputActions.Player.Inventory.canceled += ctx => Inventory = false;
        
        inputActions.Player.DebugKey.performed += ctx => Debug = true;
        inputActions.Player.DebugKey.canceled += ctx => Debug = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void ConsumeInputs()
    {
        Jump = false;
        Fire = false; 
        Inventory = false;
        Debug = false;
    }

    public void ConsumeInteract()
    {
        Interact = false;
    }
}
