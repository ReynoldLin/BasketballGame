using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Dribbling dribbling;

    private Rigidbody _rb;
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputActions = new InputSystem_Actions();
    }
    
    void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMoveCancelled;
        
        _inputActions.Player.Dribble.performed += OnDribbleStart;
        // _inputActions.Player.Dribble.canceled += OnDribbleStop;
    }

    void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMoveCancelled;
        
        _inputActions.Player.Dribble.performed -= OnDribbleStart;
        // _inputActions.Player.Dribble.canceled -= OnDribbleStop;
        _inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
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
        Vector3 moveDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        Vector3 velocity = moveDirection * moveSpeed;
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
    }
}
