using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    public Rigidbody ball;
    public float launchForce = 10f;
    public float upwardForce = 5f;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Jump.performed += OnReset;
    }

    void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Jump.performed -= OnReset;
        inputActions.Player.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        LaunchBall();
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        SceneReloader.Instance.ReloadCurrentScene();
    }

    void LaunchBall()
    {
        Vector3 force = transform.forward * launchForce + Vector3.up * upwardForce;
        ball.AddForce(force, ForceMode.Impulse);
    }
}
