using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text longestStreakText;
    [SerializeField] private TMP_Text hitsText;
    [SerializeField] private TMP_Text missesText;

    public void ShowResults(
        ScoreManager score,
        HotStreakStateMachine streak)
    {
        scoreText.text = $"Score: {score.Score}";
        accuracyText.text = $"Accuracy: {score.Accuracy:F1}%";
        hitsText.text = $"Hits: {score.Hits}";
        missesText.text = $"Misses: {score.Misses}";
        longestStreakText.text =
            $"Longest Streak: {streak.LongestStreak}";
    }
}