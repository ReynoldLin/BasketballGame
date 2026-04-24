using UnityEngine;

public class Dribbling : MonoBehaviour
{
    [Header("Ball Settings")] 
    public Rigidbody ballRB;
    public Rigidbody playerRB;
    public float dribbleForce = 6f;

    private Vector3 ballOffset;
    
    [Header("Dribble Settings")]
    public bool isDribbling = false;
    public float heightThreshold = 2f;
    
    private bool ballInHand = true;
    private float startingY;

    void Start()
    {
        startingY = ballRB.position.y;
        ballOffset = ballRB.position - playerRB.position;
    }
    
    void Update()
    {
        if (ballInHand)
        {
            HoldBall();
        }

        if (isDribbling)
        {
            KeepBallToPlayer();
            if (ballRB.position.y >= startingY - heightThreshold && ballRB.linearVelocity.y > 0)
            {
                ApplyDribbleForce();
            }
        }
    }
    
    public void StartDribble()
    {
        if (!isDribbling)
        {
            isDribbling = true;
            ReleaseBall();
            ApplyDribbleForce();
        }
    }
    
    public void StopDribble()
    {
        isDribbling = false;
        ballInHand = true;
        ballRB.isKinematic = false;
        ballRB.linearVelocity = Vector3.zero;
    }

    private void ApplyDribbleForce()
    {
        ballRB.linearVelocity = Vector3.zero;
        ballRB.AddForce(Vector3.down * dribbleForce, ForceMode.Impulse);
    }

    private void KeepBallToPlayer()
    {
        Vector3 targetPosition = playerRB.position + ballOffset;
        ballRB.MovePosition(new Vector3(targetPosition.x, ballRB.position.y, targetPosition.z));
    }
    
    public void ReleaseBall()
    {
        ballRB.isKinematic = false;
        ballInHand = false;
    }
    
    private void HoldBall()
    {
        ballRB.isKinematic = true;
    }
}
