using UnityEngine;

public class TopScoreTrigger : MonoBehaviour
{
    public ScoreTrigger scoreTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            scoreTrigger.BallPassedTop();
        }
    }
}
