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
        if (scoreManager == null)
        {
            Debug.LogError("HUDTester: ScoreManager is not assigned.");
            return;
        }

        if (hotStreakStateMachine == null)
        {
            Debug.LogError("HUDTester: HotStreakStateMachine is not assigned.");
            return;
        }

        if (scoreText == null || streakText == null || accuracyText == null)
        {
            Debug.LogError("HUDTester: One or more HUD text fields are not assigned.");
            return;
        }

        scoreText.text = $"Score: {scoreManager.Score}";
        streakText.text = $"Streak: {hotStreakStateMachine.CurrentStreak}";
        accuracyText.text = $"Accuracy: {scoreManager.Accuracy:F1}%";
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

    private void HandleHotStreakStateChanged(
        HotStreakState previous,
        HotStreakState current)
    {
        if (hotStreakStateText != null)
        {
            hotStreakStateText.text = current.ToString();
        }
    }
}