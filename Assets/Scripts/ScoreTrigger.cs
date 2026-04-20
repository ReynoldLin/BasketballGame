using UnityEngine;
using TMPro;

public class ScoreTrigger : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    private int score = 0;
    private bool passedTop = false;

    public void BallPassedTop()
    {
        passedTop = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball") && passedTop)
        {
            AddScore();
        }
    }

    private void AddScore()
    {
        score += 2;
        scoreText.text = "Score: " + score;
        Debug.Log("Score: " + score);
    }
}
