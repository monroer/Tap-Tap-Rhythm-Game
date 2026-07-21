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
    private double pauseStartDspTime;
    private bool isPlaying;
    private bool isPaused;

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
        isPaused = false;
    }

    /// <summary>Pauses playback in a way SongTime can recover from cleanly (see Resume).</summary>
    public void Pause()
    {
        if (!isPlaying || isPaused) return;
        musicSource.Pause();
        pauseStartDspTime = AudioSettings.dspTime;
        isPaused = true;
    }
 
    /// <summary>
    /// Resumes after Pause(). AudioSettings.dspTime keeps advancing the whole
    /// time the game is paused, so without correction SongTime would jump
    /// ahead by the pause duration the instant we resume, desyncing every
    /// note still in flight. Shifting dspSongStartTime forward by exactly how
    /// long we were paused keeps SongTime continuous with actual playback.
    /// </summary>
    public void Resume()
    {
        if (!isPlaying || !isPaused) return;
        double pausedDuration = AudioSettings.dspTime - pauseStartDspTime;
        dspSongStartTime += pausedDuration;
        musicSource.UnPause();
        isPaused = false;
    }
    
    public void StopSong()
    {
        musicSource.Stop();
        isPlaying = false;
        isPaused = false;
    }

    private float lastLogTime;

    private void Update()
    {
        if (!isPlaying || isPaused) return;

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
