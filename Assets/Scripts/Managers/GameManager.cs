using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// State machine skeleton. Just handles transitions between states for now,
// no actual gameplay logic wired in yet.
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver,
        Results
    }

    [SerializeField] private NoteSpawner noteSpawner;
    [SerializeField] private HitDetectionManager hitDetectionManager;
    [SerializeField] private AudioSyncManager audioSyncManager;
    [SerializeField] private AudioClip songClip; // the track for whatever level is being played
    [SerializeField] private AudioClip songClipB;
    [SerializeField] private BeatMapManager beatMapManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LocalDataManager localDataManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private HotStreakStateMachine hotStreakStateMachine;
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private ResultsUI resultsUI;
    [SerializeField] private RailVisualsController railVisualsController;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // Other scripts can subscribe to this to react to state changes later
    public event Action<GameState> OnStateChanged;

    private void OnEnable()
    {
        if (audioSyncManager != null)
        {
            audioSyncManager.OnSongFinished += EndGame;
        }
    }

    private void OnDisable()
    {
        if (audioSyncManager != null)
        {
            audioSyncManager.OnSongFinished -= EndGame;
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void StartGame(RailMode railMode, SongMode songMode)
    {
        if (beatMapManager != null)
        {
            beatMapManager.SetRailMode(railMode);
            beatMapManager.SetSongMode(songMode);
        }

        if (railVisualsController != null)
        {
            railVisualsController.ApplyRailMode();
        }

        if (noteSpawner != null)
        {
            noteSpawner.ResetSpawner();
        }

        if (scoreManager != null)
        {
            scoreManager.ResetForNewSong();
        }

        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.ResetForNewSong();
        }

        StartGame();
    }

    public void RestartGame()
    {
        if (noteSpawner != null)
        {
            noteSpawner.ResetSpawner();
        }

        if (scoreManager != null)
        {
            scoreManager.ResetForNewSong();
        }

        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.ResetForNewSong();
        }

        StartGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        audioSyncManager?.Pause();
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        audioSyncManager?.Resume();
        ChangeState(GameState.Gameplay);
    }

    public void EndGame()
    {
        if (CurrentState != GameState.Gameplay)
        {
            return;
        }

        Time.timeScale = 1f;

        if (localDataManager != null &&
            scoreManager != null &&
            hotStreakStateMachine != null)
        {
            localDataManager.SaveSession(
                scoreManager.Score,
                scoreManager.Hits,
                scoreManager.Misses,
                hotStreakStateMachine.LongestStreak,
                scoreManager.Accuracy
            );

            Debug.Log("Session data saved.");
        }

        if (resultsUI != null &&
            scoreManager != null &&
            hotStreakStateMachine != null)
        {
            resultsUI.ShowResults(scoreManager, hotStreakStateMachine);
        }
        else
        {
            Debug.LogError("GameManager: ResultsUI or manager references are not assigned.");
        }

        audioSyncManager?.StopSong();
        ChangeState(GameState.Results);
    }

    public void ShowResults()
    {
        ChangeState(GameState.Results);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        audioSyncManager?.StopSong();
        ChangeState(GameState.MainMenu);
    }

    private void StartGame()
    {
        Time.timeScale = 1f;

        if (beatMapManager != null && inputManager != null)
     {
           inputManager.railCount = beatMapManager.RailCount;
        }

        AudioClip clipToPlay = songClip;
        if (beatMapManager != null && beatMapManager.CurrentSongMode == SongMode.SongB)
        {
            clipToPlay = songClipB;
        }

        if (audioSyncManager != null && clipToPlay != null)
        {
            audioSyncManager.PlaySong(clipToPlay);
        }

        ChangeState(GameState.Gameplay);
    }   

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        // Gameplay systems only run during actual gameplay. this single line
        // covers Pause, GameOver, Results, and MainMenu
        bool gameplayActive = newState == GameState.Gameplay;
        if (noteSpawner != null)
            noteSpawner.enabled = gameplayActive;

        if (hitDetectionManager != null)
            hitDetectionManager.enabled = gameplayActive;

        if (resultsPanel != null)
            resultsPanel.SetActive(newState == GameState.Results);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(newState == GameState.MainMenu);

        if (pausePanel != null)
            pausePanel.SetActive(newState == GameState.Paused);

        if (hudPanel != null)
            hudPanel.SetActive(newState == GameState.Gameplay || newState == GameState.Paused);

        Debug.Log("GameManager state: " + newState);
        OnStateChanged?.Invoke(newState);
    }
}
