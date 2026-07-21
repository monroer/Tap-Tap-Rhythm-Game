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

    private IReadOnlyList<NoteData> notes;
    private int nextNoteIndex;

    private float GetRailX(int rail)
    {
        float railWidth = Screen.width / (float)inputManager.railCount;
        float screenX = (rail + 0.5f) * railWidth;
        float camDepth = -Camera.main.transform.position.z;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(
            new Vector3(screenX, Screen.height / 2f, camDepth));
        return worldPoint.x;
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
        
        note.Initialize(startPos, hitPos, fallDuration, data.rail, data.time);

        if (hitDetectionManager != null)
        {
            hitDetectionManager.RegisterNote(note);
        }
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
