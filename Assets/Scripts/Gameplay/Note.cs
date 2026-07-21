using Unity.VisualScripting;
using UnityEngine;
public enum NoteState { Waiting, Hit, Missed }
public class Note : MonoBehaviour
{
    [SerializeField] private float fallDuration = 2f;
    [SerializeField] private float missWindow = 0.2f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 hitZonePosition;

    public void Initialize(Vector3 start, Vector3 hitZone, float duration)
    {
        startPosition = start;
        hitZonePosition = hitZone;
        fallDuration = duration;
        elapsed = 0f;
        state = NoteState.Waiting;
    }
    private float elapsed;
    private NoteState state = NoteState.Waiting;

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / fallDuration);
        transform.position = Vector3.Lerp(startPosition, hitZonePosition, t);

        if (state == NoteState.Waiting && elapsed >= fallDuration + missWindow)
        {
            state = NoteState.Missed;
            Debug.Log("Note missed!");
            Destroy(gameObject);
        }
    }

    public void RegisterHit()
    {
        if (state != NoteState.Waiting) return;
        state = NoteState.Hit;
        Debug.Log("Note hit!");
        Destroy(gameObject);
    }
}
