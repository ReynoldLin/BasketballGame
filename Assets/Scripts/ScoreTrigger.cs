using UnityEngine;
using TMPro;

public class ScoreTrigger : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
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
