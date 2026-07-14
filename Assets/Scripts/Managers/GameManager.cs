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

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // Other scripts can subscribe to this to react to state changes later
    public event Action<GameState> OnStateChanged;

    private void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void StartGame()
    {
        ChangeState(GameState.Gameplay);
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Gameplay);
    }

    public void EndGame()
    {
        ChangeState(GameState.GameOver);
    }

    public void ShowResults()
    {
        ChangeState(GameState.Results);
    }

    public void ReturnToMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("GameManager state: " + newState);
        OnStateChanged?.Invoke(newState);
    }
}
