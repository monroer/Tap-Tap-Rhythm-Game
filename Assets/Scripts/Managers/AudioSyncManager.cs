using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSyncManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    private void Awake()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    [SerializeField] private AudioClip testClip;

    private void Start()
    {
        if (testClip != null) PlaySong(testClip);
    }

    [SerializeField] private double scheduleLeadTime = 0.2;

    private double dspSongStartTime;
    private bool isPlaying;

    public float SongTime => isPlaying 
    ? (float)(AudioSettings.dspTime - dspSongStartTime)
    : 0f;

    public void PlaySong(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AudioSyncManager error - no clip found!");
            return;
        }

        musicSource.clip = clip;
        dspSongStartTime = AudioSettings.dspTime + scheduleLeadTime;
        musicSource.PlayScheduled(dspSongStartTime);
        isPlaying = true;
    }

    public void StopSong()
    {
        musicSource.Stop();
        isPlaying = false;
    }

    private float lastLogTime;

    private void Update()
    {
        if (!isPlaying) return;

        if (SongTime - lastLogTime >= 1f)
        {
            lastLogTime = SongTime;
            Debug.Log($"SongTime: {SongTime:F3}s | musicSource.time: {musicSource.time:F3}");
        }

        if (!musicSource.isPlaying && musicSource.time == 0f)
        {
            isPlaying = false;
        }
    }
}
