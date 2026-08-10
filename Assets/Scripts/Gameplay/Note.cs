using System;
using Unity.VisualScripting;
using UnityEngine;

public enum NoteState { Waiting, Hit, Missed }

public class Note : MonoBehaviour
{
    [SerializeField] private float fallDuration = 2f;
    [SerializeField] private float missWindow = 0.2f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 hitZonePosition;
    [SerializeField] private SpriteRenderer noteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    
    /// <summary>Which rail this note belongs to.</summary>
    public int Rail { get; private set; }
 
    /// <summary>
    /// Absolute song time in seconds, matching AudioSyncManager.SongTime at
    /// which this note reaches the hit zone. This is the value HitDetectionManager
    /// compares tap time against.
    /// </summary>
    public float TargetHitTime { get; private set; }
 
    public NoteState State { get; private set; } = NoteState.Waiting;
 
    /// <summary>
    /// Fired exactly once, right before this note is destroyed, whether it was
    /// hit or missed. HitDetectionManager subscribes to this to know when to
    /// stop tracking the note.
    /// </summary>
    public event Action<Note> OnResolved;
 
    private float elapsed;
    
    
    private void Awake()
    {
        if (noteRenderer == null)
        {
            noteRenderer = GetComponent<SpriteRenderer>();
        }
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
    }
    
    public void Initialize(
        Vector3 start,
        Vector3 hitZone,
        float duration,
        int rail,
        float targetHitTime,
        Color railColor)
    {
        startPosition = start;
        hitZonePosition = hitZone;
        fallDuration = duration;
        Rail = rail;
        TargetHitTime = targetHitTime;
        elapsed = 0f;
        State = NoteState.Waiting;

        SetColor(railColor);
    }
    
    public void SetColor(Color color)
    {
        if (noteRenderer != null)
        {
            noteRenderer.color = color;
        }
    }

    public void ApplyHotStreakVisual(HotStreakState state)
    {
        if (trailRenderer == null) return;

        Color baseColor = noteRenderer != null ? noteRenderer.color : Color.white;

        switch (state)
        {
            case HotStreakState.HotStreak:
                trailRenderer.enabled = true;
                trailRenderer.time = 0.35f;
                trailRenderer.widthMultiplier = 0.5f;
                trailRenderer.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.9f);
                trailRenderer.endColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
                break;

            case HotStreakState.Building:
                trailRenderer.enabled = true;
                trailRenderer.time = 0.15f;
                trailRenderer.widthMultiplier = 0.25f;
                trailRenderer.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
                trailRenderer.endColor = new Color (baseColor.r, baseColor.g, baseColor.b, 0f);
                break;

            default:
            trailRenderer.enabled = false;
            break;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / fallDuration);
        transform.position = Vector3.Lerp(startPosition, hitZonePosition, t);

        if (State == NoteState.Waiting && elapsed >= fallDuration + missWindow)
        {
            Resolve(NoteState.Missed);
        }
    }

    /// <summary>
    /// Called by HitDetectionManager when a tap targeted this note but fell
    /// outside the acceptable timing window.
    /// </summary>
    public void RegisterMiss()
    {
        if (State != NoteState.Waiting) return;
        Resolve(NoteState.Missed);
    }
    
    public void RegisterHit()
    {
        if (State != NoteState.Waiting) return;
        Resolve(NoteState.Hit);
    }

    private void Resolve(NoteState result)
    {
        State = result;
        Debug.Log(result == NoteState.Hit ? "Note hit!" : "Note missed!");
        OnResolved?.Invoke(this);
        Destroy(gameObject);
    }
}