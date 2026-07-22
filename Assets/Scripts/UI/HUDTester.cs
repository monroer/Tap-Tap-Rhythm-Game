using TMPro;
using UnityEngine;

public class HUDTester : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private HitDetectionManager hitDetectionManager;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text accuracyText;

    private int score;
    private int streak;
    private int hits;
    private int misses;

    private void OnEnable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified += HandleHit;
        }
    }

    private void OnDisable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified -= HandleHit;
        }
    }

    private void Start()
    {
        ResetHUD();
    }

    private void HandleHit(int rail, HitResult result)
    {
        switch (result)
        {
            case HitResult.Perfect:
                hits++;
                streak++;
                score += 100;
                Debug.Log($"Rail {rail}: PERFECT");
                break;

            case HitResult.Good:
                hits++;
                streak++;
                score += 50;
                Debug.Log($"Rail {rail}: GOOD");
                break;

            case HitResult.Miss:
                misses++;
                streak = 0;
                Debug.Log($"Rail {rail}: MISS");
                break;
        }

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        int totalNotes = hits + misses;

        float accuracy = totalNotes > 0
            ? (float)hits / totalNotes * 100f
            : 100f;

        scoreText.text = $"Score: {score}";
        streakText.text = $"Streak: {streak}";
        accuracyText.text = $"Accuracy: {accuracy:F1}%";
    }

    private void ResetHUD()
    {
        score = 0;
        streak = 0;
        hits = 0;
        misses = 0;

        UpdateHUD();
    }
}