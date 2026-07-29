using System.Collections;
using UnityEngine;

public class BackgroundVisualController : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private HotStreakStateMachine hotStreakStateMachine;

    [Header("Background")]
    [SerializeField] private Camera gameplayCamera;

    [Header("State Colors")]
    [SerializeField] private Color normalColor =
        new Color32(24, 27, 32, 255);

    [SerializeField] private Color buildingColor =
        new Color32(31, 38, 65, 255);

    [SerializeField] private Color hotStreakColor =
        new Color32(70, 28, 75, 255);

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 0.35f;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnHotStreakStateChanged +=
                HandleHotStreakStateChanged;
        }
    }

    private void OnDisable()
    {
        if (hotStreakStateMachine != null)
        {
            hotStreakStateMachine.OnHotStreakStateChanged -=
                HandleHotStreakStateChanged;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    private void Start()
    {
        if (gameplayCamera == null)
        {
            return;
        }

        HotStreakState startingState =
            hotStreakStateMachine != null
                ? hotStreakStateMachine.CurrentState
                : HotStreakState.Normal;

        gameplayCamera.backgroundColor =
            GetColorForState(startingState);
    }

    private void HandleHotStreakStateChanged(
        HotStreakState previous,
        HotStreakState current)
    {
        if (gameplayCamera == null)
        {
            return;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(
            TransitionBackground(GetColorForState(current)));
    }

    private Color GetColorForState(HotStreakState state)
    {
        switch (state)
        {
            case HotStreakState.Building:
                return buildingColor;

            case HotStreakState.HotStreak:
                return hotStreakColor;

            default:
                return normalColor;
        }
    }

    private IEnumerator TransitionBackground(Color targetColor)
    {
        Color startingColor = gameplayCamera.backgroundColor;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / transitionDuration);

            gameplayCamera.backgroundColor =
                Color.Lerp(startingColor, targetColor, progress);

            yield return null;
        }

        gameplayCamera.backgroundColor = targetColor;
        transitionCoroutine = null;
    }
}