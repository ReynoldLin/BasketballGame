using UnityEngine;

public class Dribbling : MonoBehaviour
{
    [Header("Ball Settings")] 
    public Rigidbody ballRB;
    public Rigidbody playerRB;
    [SerializeField] private float dribbleForce = 6f;
    
    [Header("Dribble Settings")]
    public bool isDribbling = false;
    [SerializeField] private float heightThreshold = 2f;

    private Crossover crossover;
    private bool ballInHand = true;
    private float startingY;
    public Vector3 ballOffset;

    void Start()
    {
        startingY = ballRB.position.y;
        ballOffset = ballRB.position - playerRB.position;
        crossover = GetComponent<Crossover>();
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

    public void ApplyDribbleForce()
    {
        ballRB.linearVelocity = Vector3.zero;
        ballRB.AddForce(Vector3.down * dribbleForce, ForceMode.Impulse);
    }

    public void KeepBallToPlayer()
    {
        if (crossover != null && crossover.isCrossingOver) return;
        
        Vector3 targetPosition = playerRB.position + ballOffset;
        ballRB.MovePosition(new Vector3(targetPosition.x, ballRB.position.y, targetPosition.z));
    }
    
    public void ResumeKeepBallToPlayer()
    {
        ballOffset = ballRB.position - playerRB.position;
        KeepBallToPlayer();
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
