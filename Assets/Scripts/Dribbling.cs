using UnityEngine;

public class Dribbling : MonoBehaviour
{
    [Header("Ball Settings")] 
    public Rigidbody ballRigidbody;
    public Transform handPosition;
    public float dribbleForce = 8f;
    public float holdHeight = 0.8f;
    
    [Header("Dribble Settings")]
    public float dribbleSpeedMultiplier = 1f;
    public bool isDribbling = false;
    
    private bool ballInHand = true;
    
    void Update()
    {
        if (ballInHand)
        {
            HoldBall();
        }
    }
    
    void FixedUpdate()
    {
        if (isDribbling && !ballInHand)
        {
            ApplyDribbleForce();
        }
    }
    
    public void StartDribble()
    {
        if (!isDribbling)
        {
            isDribbling = true;
            ReleaseBall();
            ballRigidbody.linearVelocity = Vector3.zero;
            ballRigidbody.AddForce(Vector3.down * dribbleForce, ForceMode.Impulse);
        }
    }
    
    public void StopDribble()
    {
        isDribbling = false;
        ballInHand = true;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.isKinematic = false;
    }
    
    public void ReleaseBall()
    {
        ballRigidbody.isKinematic = false;
        ballInHand = false;
    }
    
    private void HoldBall()
    {
        ballRigidbody.isKinematic = true;
        ballRigidbody.transform.position = handPosition.position;
    }
    
    private void ApplyDribbleForce()
    {
        float ballHeight = ballRigidbody.transform.position.y;
        
        if (ballHeight >= holdHeight)
        {
            ballRigidbody.isKinematic = false;
            float force = dribbleForce * dribbleSpeedMultiplier;
            ballRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // catch the ball when it bounces back up to hand height
        if (isDribbling && other.attachedRigidbody == ballRigidbody)
        {
            ballRigidbody.linearVelocity = Vector3.zero;
            ballRigidbody.AddForce(Vector3.down * dribbleForce, ForceMode.Impulse);
        }
    }
}
