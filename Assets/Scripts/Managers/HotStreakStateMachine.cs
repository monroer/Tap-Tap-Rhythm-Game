using System;
using UnityEngine;

public enum HotStreakState { Normal, Building, HotStreak }

/// <summary>
/// Owns the streak counter and the Normal -> Building -> HotStreak state.
/// Subscribes directly to HitDetectionManager.OnHitClassified. It does NOT
/// go through ScoreManager, so UI and Note visuals can subscribe
/// to this component's events independently of scoring logic.
///
/// Event contract:
///   OnStreakChanged(int currentStreak)
///   OnHotStreakStateChanged(HotStreakState previous, HotStreakState current)
///   OnLongestStreakChanged(int newLongestStreak)
/// </summary>
public class HotStreakStateMachine : MonoBehaviour
{
    [SerializeField] private HitDetectionManager hitDetectionManager;

    [Header("Streak thresholds (TUNE THESE)")]
    [Tooltip("Streak count needed to leave Normal and enter Building.")]
    [SerializeField] private int streakForBuilding = 10;

    [Tooltip("Streak count needed to enter HotStreak.")]
    [SerializeField] private int streakForHotStreak = 25;

    [Header("Score multipliers per state (TUNE THESE)")]
    [SerializeField] private float normalMultiplier = 1f;
    [SerializeField] private float buildingMultiplier = 2f;
    [SerializeField] private float hotStreakMultiplier = 5f;

    public HotStreakState CurrentState { get; private set; } = HotStreakState.Normal;
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }

    public float CurrentMultiplier
    {
        get
        {
            switch (CurrentState)
            {
                case HotStreakState.Building: return buildingMultiplier;
                case HotStreakState.HotStreak: return hotStreakMultiplier;
                default: return normalMultiplier;
            }
        }
    }

    public event Action<int> OnStreakChanged;
    public event Action<HotStreakState, HotStreakState> OnHotStreakStateChanged;
    public event Action<int> OnLongestStreakChanged;

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
            RegisterMiss();
        }
        else
        {
            RegisterHit();
        }
    }

    private void RegisterHit()
    {
        CurrentStreak++;
        OnStreakChanged?.Invoke(CurrentStreak);

        if (CurrentStreak > LongestStreak)
        {
            LongestStreak = CurrentStreak;
            OnLongestStreakChanged?.Invoke(LongestStreak);
        }

        EvaluateState();
    }

    private void RegisterMiss()
    {
        CurrentStreak = 0;
        OnStreakChanged?.Invoke(CurrentStreak);
        EvaluateState();
    }

    private void EvaluateState()
    {
        HotStreakState newState;
        if (CurrentStreak >= streakForHotStreak) newState = HotStreakState.HotStreak;
        else if (CurrentStreak >= streakForBuilding) newState = HotStreakState.Building;
        else newState = HotStreakState.Normal;

        if (newState != CurrentState)
        {
            HotStreakState previous = CurrentState;
            CurrentState = newState;
            OnHotStreakStateChanged?.Invoke(previous, CurrentState);
        }
    }

    /// <summary>Call this on song restart / new game so leftover state doesn't carry over.</summary>
    public void ResetForNewSong()
    {
        CurrentStreak = 0;
        LongestStreak = 0;
        CurrentState = HotStreakState.Normal;
        OnStreakChanged?.Invoke(CurrentStreak);
    }
}
