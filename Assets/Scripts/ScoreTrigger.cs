using UnityEngine;
using TMPro;

public class ScoreTrigger : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    private int score = 0;
    private bool passedTop = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreSound;

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
        PlayScoreSound();
        score += 2;
        scoreText.text = "Score: " + score;
    }

    private void PlayScoreSound()
    {
        if (audioSource != null && scoreSound != null)
        {
            audioSource.PlayOneShot(scoreSound);
        }
    }
}
