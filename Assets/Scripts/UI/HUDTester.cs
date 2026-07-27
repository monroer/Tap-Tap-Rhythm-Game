using TMPro;
using UnityEngine;

public class HUDTester : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HotStreakStateMachine hotStreakStateMachine;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text hotStreakStateText;

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += HandleScoreChanged;
            scoreManager.OnStatsChanged += HandleStatsChanged;
        }

        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnStreakChanged += HandleStreakChanged;
            hotStreakStateMachine.OnHotStreakStateChanged += HandleHotStreakStateChanged;
        }
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
            scoreManager.OnStatsChanged -= HandleStatsChanged;
        }

        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnStreakChanged -= HandleStreakChanged;
            hotStreakStateMachine.OnHotStreakStateChanged -= HandleHotStreakStateChanged;
        }
    }

    private void Start()
    {
        scoreText.text = "Score: 0";
        streakText.text = "Streak: 0";
        accuracyText.text = "Accuracy: 100.0%";
        hotStreakStateText.text = "Normal";
    }

    private void HandleScoreChanged(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }

    private void HandleStatsChanged(int hits, int misses, float accuracy)
    {
        accuracyText.text = $"Accuracy: {accuracy:F1}%";
    }

    private void HandleStreakChanged(int streak)
    {
        streakText.text = $"Streak: {streak}";
    }

    private void HandleHotStreakStateChanged(HotStreakState previous, HotStreakState current)
    {
        hotStreakStateText.text = current.ToString();
        Debug.Log($"Hot Streak state: {previous} -> {current}");
    }
}