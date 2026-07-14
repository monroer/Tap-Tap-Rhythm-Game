using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    private void Start()
    {
        //UpdateScore(42);
        //UpdateStreak(7);
    }
    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateStreak(int streak)
    {
        streakText.text = $"Streak: {streak}";
    }
}

