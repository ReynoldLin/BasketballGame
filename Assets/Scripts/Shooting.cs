using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    public Rigidbody ball;
    public Dribbling dribbling;
    public Transform rim;
    public Transform shootingPocket;
    public ShotMeter shotMeter;

    [Header("Shooting Settings")] 
    [SerializeField] private float shootingAngle = 55f;
    [SerializeField] private float pocketLerpSpeed = 18f;
    [SerializeField] private float pocketArrivalThreshold = 0.04f;
    
    [Header("Timing Window")]
    [SerializeField] private float timingWindowDuration = 1f;
    [SerializeField] private float perfectWindowSize = 0.2f;
    [SerializeField] private float missDistance = 0.5f;
    [SerializeField] private float timingWindowStart = -1f;
    private bool windowOpen = false;

    private InputSystem_Actions _inputActions;

    void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Shoot.started += OnShootStarted;
        _inputActions.Player.Shoot.canceled += OnShootReleased;
        _inputActions.Player.Reset.performed += OnReset;
    }

    void OnDisable()
    {
        _inputActions.Player.Shoot.started -= OnShootStarted;
        _inputActions.Player.Shoot.canceled -= OnShootReleased;
        _inputActions.Player.Reset.performed -= OnReset;
        _inputActions.Player.Disable();
    }

    void OpenTimingWindow()
    {
        timingWindowStart = Time.time;
        windowOpen = true;
        shotMeter.StartBar();
    }

    void Update()
    {
        if (windowOpen)
        {
            ball.MovePosition(new Vector3(shootingPocket.position.x, shootingPocket.position.y, shootingPocket.position.z));
        }
        
        if (windowOpen && Time.time - timingWindowStart > timingWindowDuration)
        {
            windowOpen = false;
        }
    }

    void Shoot()
    {
        shotMeter.StopBar();
        ball.isKinematic = false;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        
        Vector3 targetPos = CalculateTargetFromTiming();
        
        Debug.Log("Rim Position: " + rim.position);
        Debug.Log("Target Position: " + targetPos);
        
        Vector3 launchVelocity = CalculateShootingVelocity(ball.position, targetPos, shootingAngle);
        ball.AddForce(launchVelocity, ForceMode.VelocityChange);
    }
    
    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (!windowOpen)
        {
            StartCoroutine(LerpToPocket());
        }
    }

    private void OnShootReleased(InputAction.CallbackContext context)
    {
        if (windowOpen)
        {
            Shoot();
            windowOpen = false;
            ball.isKinematic = false;
        }
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        SceneReloader.Instance.ReloadCurrentScene();
    }

    private IEnumerator LerpToPocket()
    {
        ball.isKinematic = true;
        
        if (dribbling != null && dribbling.isDribbling)
            dribbling.StopDribble();
        dribbling.ReleaseBall();
        
        while (Mathf.Abs(ball.position.y - shootingPocket.position.y) > pocketArrivalThreshold)
        {
            float targetY = Mathf.Lerp(ball.position.y, shootingPocket.position.y, pocketLerpSpeed * Time.deltaTime);
            ball.MovePosition(new Vector3(shootingPocket.position.x, targetY, shootingPocket.position.z));
            yield return null;
        }
        
        ball.position = shootingPocket.position;
        OpenTimingWindow();
    }

    Vector3 CalculateShootingVelocity(Vector3 origin, Vector3 target, float angleDeg)
    {
        float g = Mathf.Abs(Physics.gravity.y);
        float angleRad = angleDeg * Mathf.Deg2Rad;
        
        // Horizontal direction and distance
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float horizontalDist = toTargetXZ.magnitude;
        float verticalDist = toTarget.y;
        
        // Calculate required launch speed using projectile motion formula
        float speedSq = (g * horizontalDist * horizontalDist) /
                        (2 * Mathf.Cos(angleRad) * Mathf.Cos(angleRad) * 
                         (horizontalDist * Mathf.Tan(angleRad) - verticalDist));
        
        if (speedSq <= 0)
        {
            Debug.LogWarning("Invalid launch angle for this distance/height. Increase launchAngle.");
            return Vector3.zero;
        }
    
        float speed = Mathf.Sqrt(speedSq);
    
        // Compose the velocity vector
        Vector3 horizontalDir = toTargetXZ.normalized;
        Vector3 velocity = horizontalDir * speed * Mathf.Cos(angleRad)
                           + Vector3.up * speed * Mathf.Sin(angleRad);
    
        return velocity;
    }
    
    Vector3 CalculateTargetFromTiming()
    {
        float elapsed = Time.time - timingWindowStart;
        float t = elapsed / timingWindowDuration;

        float perfectStart = 0.5f - perfectWindowSize / 2f;
        float perfectEnd   = 0.5f + perfectWindowSize / 2f;
        
        Vector3 missDir = (rim.position - ball.position).normalized;

        if (t >= perfectStart && t <= perfectEnd)
        {
            return rim.position;
        }
        if (t < perfectStart)
        {
            // Too early — overshoot
            float howFarOff = (perfectStart - t) / perfectStart;
            return rim.position + missDir * missDistance * howFarOff;
        }
        else
        {
            // Too late — undershoot
            float howFarOff = (t - perfectEnd) / (1f - perfectEnd);
            return rim.position - missDir * missDistance * howFarOff;
        }
    }
}