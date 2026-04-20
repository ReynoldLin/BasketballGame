using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Dribbling dribbling;

    private Rigidbody rb;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
    }
    
    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCancelled;
        
        inputActions.Player.Dribble.performed += OnDribbleStart;
        // inputActions.Player.Dribble.canceled += OnDribbleStop;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCancelled;
        
        inputActions.Player.Dribble.performed -= OnDribbleStart;
        // inputActions.Player.Dribble.canceled -= OnDribbleStop;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
    
    private void OnDribbleStart(InputAction.CallbackContext context)
    {
        dribbling.StartDribble();
    }

    // private void OnDribbleStop(InputAction.CallbackContext context)
    // {
    //     Debug.Log("Stop dribbling");
    //     dribbling.StopDribble();
    // }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 velocity = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }
}
