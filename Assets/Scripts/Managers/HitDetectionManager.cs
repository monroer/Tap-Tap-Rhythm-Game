using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum HitResult { Perfect, Great, Good, Miss }

/// <summary>
/// Subscribes to InputManager.OnRailTapped, compares tap timing against each
/// note's target hit time, and classifies the result. Notes register
/// themselves (via NoteSpawner) when spawned and are automatically
/// deregistered when resolved (hit, missed-by-timing, or missed-by-timeout).
/// </summary>
public class HitDetectionManager : MonoBehaviour
{
    [SerializeField] private AudioSyncManager audioSyncManager;
    [SerializeField] private InputManager inputManager;

    [Header("Timing Windows (ms)")]
    [Tooltip("Max |tap time - target time|, in ms, still counted as Perfect. " +
             "Requirements target ~24ms at the tight end.")]
    [SerializeField] private float perfectWindowMs = 50f;

    [Tooltip("Max |tap time - target time|, in ms, still counted as a hit at all " +
             "(Great, almost Perfect). Sits between Good and Perfect. Requirements " +
             "target ~40ms between tight and loose end.")]
    [SerializeField] private float greatWindowMs = 90f;

    [Tooltip("Max |tap time - target time|, in ms, still counted as a hit at all " +
             "(Good, not Perfect). Anything beyond this is a Miss. Requirements " +
             "target ~80ms at the loose end.")]
    [SerializeField] private float goodWindowMs = 150f;

    [SerializeField] private float noteConsiderationWindowMs = 180f;

    /// <summary>Fired after each tap is classified - hook ScoreManager/UI here.</summary>
    public event Action<int, HitResult> OnHitClassified;

    // Active, unresolved notes per rail. NoteSpawner spawns notes in time
    // order, so each list stays sorted by ascending TargetHitTime as notes
    // are appended - no re-sort needed.
    private readonly Dictionary<int, List<Note>> activeNotesByRail = new Dictionary<int, List<Note>>();

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnRailTapped += HandleRailTapped;
        }
    }

    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnRailTapped -= HandleRailTapped;
        }
    }

    /// <summary>Call from NoteSpawner immediately after a note is instantiated and initialized.</summary>
    public void RegisterNote(Note note)
    {
        if (!activeNotesByRail.TryGetValue(note.Rail, out var list))
        {
            list = new List<Note>();
            activeNotesByRail[note.Rail] = list;
        }

        list.Add(note);
        note.OnResolved += HandleNoteResolved;
    }

    public void ClearAllNotes()
    {
        foreach (var list in activeNotesByRail.Values)
        {
            foreach (var note in list)
            {
                if (note != null)
                {
                    note.OnResolved -= HandleNoteResolved;
                }
            }
        }
        activeNotesByRail.Clear();
    }

    private void HandleNoteResolved(Note note)
    {
        note.OnResolved -= HandleNoteResolved;
        if (activeNotesByRail.TryGetValue(note.Rail, out var list))
        {
            list.Remove(note);
        }
    }

    // Matches InputManager.OnRailTapped's Action<int, double> signature exactly,
    // so it can be subscribed directly with no wrapper needed.
    private void HandleRailTapped(int rail, double tapDspTime)
    {
        // InputManager timestamps the tap with AudioSettings.dspTime the instant
        // it happens, but this handler runs later that frame (or later still,
        // if events queue up). At a 24-80ms budget that gap matters, so we
        // correct for it: figure out how much dsp time has passed since the
        // real tap, and roll SongTime back by the same amount.
        double latency = AudioSettings.dspTime - tapDspTime;
        float songTimeAtTap = audioSyncManager.SongTime - (float)latency;

        HitResult result = Classify(rail, songTimeAtTap);
        OnHitClassified?.Invoke(rail, result);
    }

    private HitResult Classify(int rail, float songTimeAtTap)
    {
        if (!activeNotesByRail.TryGetValue(rail, out var list) || list.Count == 0)
        {
            // Tapped a rail with no note waiting in it at all - a whiff.
            return HitResult.Miss;
        }

        Note target = FindClosestNote(list, songTimeAtTap);
        float offsetMs = Mathf.Abs(songTimeAtTap - target.TargetHitTime) * 1000f;

        HitResult result;
        if (offsetMs <= perfectWindowMs) result = HitResult.Perfect;
        else if (offsetMs <= greatWindowMs) result = HitResult.Great;
        else if (offsetMs <= goodWindowMs) result = HitResult.Good;
        else result = HitResult.Miss;

        if (result == HitResult.Miss)
        {
            if (offsetMs <= noteConsiderationWindowMs)
            {
                target.RegisterMiss();
            }
        }
        else
        {
            target.RegisterHit();
        }

      
        return result;
    }

    private Note FindClosestNote(List<Note> notes, float songTime)
    {
        Note closest = notes[0];
        float bestDelta = Mathf.Abs(songTime - closest.TargetHitTime);

        for (int i = 1; i < notes.Count; i++)
        {
            float delta = Mathf.Abs(songTime - notes[i].TargetHitTime);
            if (delta < bestDelta)
            {
                bestDelta = delta;
                closest = notes[i];
            }
        }

        return closest;
    }
}
