using System;
using UnityEngine;

/// <summary>
/// Tracks hits, misses, and total score in real time. Subscribes directly to
/// HitDetectionManager.OnHitClassified (same event HotStreakStateMachine
/// subscribes to) and reads the current multiplier from
///  HotStreakStateMachine when awarding points.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private HitDetectionManager hitDetectionManager;
    [SerializeField] private HotStreakStateMachine hotStreakStateMachine;

    [Header("Base points per hit quality (TUNE THESE)")]
    [SerializeField] private int perfectBasePoints = 200;
    [SerializeField] private int greatBasePoints = 100;
    [SerializeField] private int goodBasePoints = 50;

    public int Score { get; private set; }
    public int Hits { get; private set; }
    public int Misses { get; private set; }
    public float Accuracy => (Hits + Misses) > 0 ? (float)Hits / (Hits + Misses) * 100f : 100f;

    /// <summary>Fired whenever Score changes, with the new total.</summary>
    public event Action<int> OnScoreChanged;

    /// <summary>Fired whenever hits/misses change.</summary>
    public event Action<int, int, float> OnStatsChanged;

    private void OnEnable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified += HandleHitClassified;
        }
    }

    private void OnDisable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified -= HandleHitClassified;
        }
    }

    private void HandleHitClassified(int rail, HitResult result)
    {
        if (result == HitResult.Miss)
        {
            Misses++;
            OnStatsChanged?.Invoke(Hits, Misses, Accuracy);
            return;
        }

        Hits++;
        int basePoints = result == HitResult.Perfect ? perfectBasePoints
            : result == HitResult.Great ? greatBasePoints
            : goodBasePoints;
        float multiplier = hotStreakStateMachine != null ? hotStreakStateMachine.CurrentMultiplier : 1f;
        int awarded = Mathf.RoundToInt(basePoints * multiplier);

        Score += awarded;
        OnScoreChanged?.Invoke(Score);
        OnStatsChanged?.Invoke(Hits, Misses, Accuracy);
    }

    /// <summary>Call this on song restart / new game so leftover state doesn't carry over.</summary>
    public void ResetForNewSong()
    {
        Score = 0;
        Hits = 0;
        Misses = 0;
        OnScoreChanged?.Invoke(Score);
        OnStatsChanged?.Invoke(Hits, Misses, Accuracy);
    }
}
