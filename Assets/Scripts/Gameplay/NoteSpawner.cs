using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private BeatMapManager beatMapManager;
    [SerializeField] private AudioSyncManager audioSyncManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private HitDetectionManager hitDetectionManager;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private float fallDuration = 2f;
    [SerializeField] private float spawnY = 5f;
    [SerializeField] private float hitZoneY = -3f;
    [SerializeField] private HotStreakStateMachine hotStreakStateMachine;
    
    [Header("Note Colors")]
    [SerializeField] private Color[] railColors =
    {
        new Color32(232, 84, 89, 255),   // Red
        new Color32(233, 200, 63, 255),  // Yellow
        new Color32(77, 186, 115, 255),  // Green
        new Color32(77, 125, 219, 255)   // Blue
    };

    private IReadOnlyList<NoteData> notes;
    private int nextNoteIndex;
    private readonly List<Note> activeNotes = new List<Note>();

    private float GetRailX(int rail)
    {
        float railWidth = Screen.width / (float)inputManager.railCount;
        float screenX = (rail + 0.5f) * railWidth;
        float camDepth = -Camera.main.transform.position.z;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(
            new Vector3(screenX, Screen.height / 2f, camDepth));
        return worldPoint.x;
    }
    
    private void OnEnable()
    {
        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnHotStreakStateChanged += HandleHotStreakStateChanged;
        }
    }

    private void OnDisable()
    {
        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnHotStreakStateChanged -= HandleHotStreakStateChanged;
        }
    }

    private void Start()
    {
        notes = beatMapManager.Notes;
    }

    private void SpawnNote(NoteData data)
    {
        float x = GetRailX(data.rail);
        Vector3 startPos = new Vector3(x, spawnY, 0f);
        Vector3 hitPos = new Vector3(x, hitZoneY, 0f);

        GameObject noteObj = Instantiate(notePrefab, startPos, Quaternion.identity);
        Note note = noteObj.GetComponent<Note>();
        
        Color railColor = GetRailColor(data.rail);

        note.Initialize(
            startPos,
            hitPos,
            fallDuration,
            data.rail,
            data.time,
            railColor);
        
        if (hotStreakStateMachine != null)
        {
            note.ApplyHotStreakVisual(hotStreakStateMachine.CurrentState);
        }

        activeNotes.Add(note);
        note.OnResolved += HandleNoteResolved;

       

        if (hitDetectionManager != null)
        {
            hitDetectionManager.RegisterNote(note);
        }
    }
    
    private void HandleHotStreakStateChanged(HotStreakState previous, HotStreakState current)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            Note note = activeNotes[i];

            if (note == null)
            {
                activeNotes.RemoveAt(i);
                continue;
            }

            note.ApplyHotStreakVisual(current);
        }
    }
    

    private void HandleNoteResolved(Note note)
    {
        if (note == null)
        {
            return;
        }

        note.OnResolved -= HandleNoteResolved;
        activeNotes.Remove(note);
    }
    private Color GetRailColor(int rail)
    {
        if (railColors == null || rail < 0 || rail >= railColors.Length)
        {
            Debug.LogWarning($"No color configured for rail {rail}. Using white.");
            return Color.white;
        }

        return railColors[rail];
    }
    private void Update()
    {
        if (notes == null || nextNoteIndex >= notes.Count) return;

        NoteData nextNote = notes[nextNoteIndex];
        float spawnTime = nextNote.time - fallDuration;

        if (audioSyncManager.SongTime >= spawnTime)
        {
            SpawnNote(nextNote);
            nextNoteIndex++;
        }
    }

    /// <summary>Stops spawning new notes. Notes already in flight are unaffected. Intended for GameManager to call on pause/game over.</summary>
    public void StopSpawning() => enabled = false;
 
    /// <summary>Resumes spawning. Intended for GameManager to call on resume/gameplay start.</summary>
    public void ResumeSpawning() => enabled = true;
}
