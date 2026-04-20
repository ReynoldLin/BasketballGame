using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    public Rigidbody ball;
    public float launchForce = 10f;
    public float upwardForce = 5f;
    public Dribbling dribbling;
    public Transform rim;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Shoot.performed += OnShoot;
        inputActions.Player.Reset.performed += OnReset;
    }

    void OnDisable()
    {
        inputActions.Player.Shoot.performed -= OnShoot;
        inputActions.Player.Reset.performed -= OnReset;
        inputActions.Player.Disable();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        LaunchBall();
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        SceneReloader.Instance.ReloadCurrentScene();
    }

    void LaunchBall()
    {
        if (dribbling != null && dribbling.isDribbling)
        {
            dribbling.StopDribble();
        }
        
        dribbling.ReleaseBall();
        
        Vector3 directionToRim = (rim.position - ball.position).normalized;
        Vector3 force = directionToRim * launchForce + Vector3.up * upwardForce;
        ball.AddForce(force, ForceMode.Impulse);
    }
}
