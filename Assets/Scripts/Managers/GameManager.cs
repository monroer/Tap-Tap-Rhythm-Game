using System;
using UnityEngine;

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
    
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // Other scripts can subscribe to this to react to state changes later
    public event Action<GameState> OnStateChanged;

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (audioSyncManager != null && songClip != null)
        {
            audioSyncManager.PlaySong(songClip);
        }
        ChangeState(GameState.Gameplay);
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
        Time.timeScale = 1f;
        audioSyncManager?.StopSong();
        ChangeState(GameState.GameOver);
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

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        // Gameplay systems only run during actual gameplay. this single line
        // covers Pause, GameOver, Results, and MainMenu
        bool gameplayActive = newState == GameState.Gameplay;
        if (noteSpawner != null) noteSpawner.enabled = gameplayActive;
        if (hitDetectionManager != null) hitDetectionManager.enabled = gameplayActive;
        
        Debug.Log("GameManager state: " + newState);
        OnStateChanged?.Invoke(newState);
    }
}
