using System.Collections;
using TMPro;
using UnityEngine;

public class HitFeedbackController : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private HitDetectionManager hitDetectionManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private NoteTarget[] noteTargets;

    [Header("Audio")]
    [SerializeField] private AudioSource feedbackAudioSource;
    [SerializeField] private AudioClip perfectSound;
    [SerializeField] private AudioClip greatSound;
    [SerializeField] private AudioClip goodSound;
    [SerializeField] private AudioClip missSound;

    [Header("Visual Feedback")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private CanvasGroup resultCanvasGroup;

    [SerializeField] private float resultDisplayDuration = 0.5f;
    [SerializeField] private float riseDistance = 40f;
    [SerializeField] private float startScale = 1.25f;

    [Tooltip("Vertical screen position of the feedback text. 0 is bottom and 1 is top.")]
    [Range(0f, 1f)]
    [SerializeField] private float resultScreenHeight = 0.25f;

    [Header("Feedback Colors")]
    [SerializeField] private Color perfectColor =
        new Color32(255, 215, 70, 255);

    [SerializeField] private Color greatColor =
        new Color32(80, 200, 255, 255);

    [SerializeField] private Color goodColor =
        new Color32(90, 200, 120, 255);

    [SerializeField] private Color missColor =
        new Color32(235, 75, 80, 255);

    [Header("Optional Effects")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private Animator hitLineAnimator;

    private Coroutine resultTextCoroutine;
    private Vector2 feedbackStartPosition;
    private Vector3 feedbackNormalScale;

    private void Awake()
    {
        if (resultText != null)
        {
            feedbackNormalScale = resultText.rectTransform.localScale;
        }
    }

    private void OnEnable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified += HandleHitClassified;
        }
    }

    private void OnDisable()
    {
        if (hitDetectionManager != null)
        {
            hitDetectionManager.OnHitClassified -= HandleHitClassified;
        }

        if (resultTextCoroutine != null)
        {
            StopCoroutine(resultTextCoroutine);
            resultTextCoroutine = null;
        }
    }

    private void Start()
    {
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }

        if (resultCanvasGroup != null)
        {
            resultCanvasGroup.alpha = 0f;
        }
    }

    private void HandleHitClassified(int rail, HitResult result)
    {
        MoveResultTextToRail(rail);

        switch (result)
        {
            case HitResult.Perfect:
                PlayFeedback(
                    rail,
                    "PERFECT!",
                    perfectSound,
                    perfectColor,
                    true);
                break;

            case HitResult.Great:
                PlayFeedback(
                    rail,
                    "GREAT!",
                    greatSound,
                    greatColor,
                    true);
                break;

            case HitResult.Good:
                PlayFeedback(
                    rail,
                    "GOOD",
                    goodSound,
                    goodColor,
                    true);
                break;

            case HitResult.Miss:
                PlayFeedback(
                    rail,
                    "MISS",
                    missSound,
                    missColor,
                    false);
                break;
        }
    }

    private void MoveResultTextToRail(int rail)
    {
        if (resultText == null || inputManager == null)
        {
            return;
        }

        int railCount = inputManager.railCount;

        if (railCount <= 0 || rail < 0 || rail >= railCount)
        {
            return;
        }

        RectTransform parentRect =
            resultText.rectTransform.parent as RectTransform;

        if (parentRect == null)
        {
            return;
        }

        float railWidth = Screen.width / (float)railCount;
        float screenX = (rail + 0.5f) * railWidth;
        float screenY = Screen.height * resultScreenHeight;

        Camera canvasCamera = null;
        Canvas canvas = resultText.canvas;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = canvas.worldCamera;
        }

        bool converted =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                new Vector2(screenX, screenY),
                canvasCamera,
                out Vector2 localPoint);

        if (converted)
        {
            resultText.rectTransform.anchoredPosition = localPoint;
            feedbackStartPosition = localPoint;
        }
    }

    private void PlayFeedback(
        int rail,
        string message,
        AudioClip sound,
        Color color,
        bool successfulHit)
    {
        PlaySound(sound);
        ShowResultText(message, color);

        if (successfulHit)
        {
            if (noteTargets != null &&
                rail >= 0 &&
                rail < noteTargets.Length &&
                noteTargets[rail] != null)
            {
                noteTargets[rail].PlayHitAnimation();
            }

            if (hitParticles != null)
            {
                hitParticles.Play();
            }

            if (hitLineAnimator != null)
            {
                hitLineAnimator.SetTrigger("Hit");
            }
        }
        else
        {
            if (noteTargets != null &&
                rail >= 0 &&
                rail < noteTargets.Length &&
                noteTargets[rail] != null)
            {
                noteTargets[rail].PlayMissAnimation();
            }

            if (hitLineAnimator != null)
            {
                hitLineAnimator.SetTrigger("Miss");
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (feedbackAudioSource != null && clip != null)
        {
            feedbackAudioSource.PlayOneShot(clip);
        }
    }

    private void ShowResultText(string message, Color color)
    {
        if (resultText == null)
        {
            return;
        }

        if (resultTextCoroutine != null)
        {
            StopCoroutine(resultTextCoroutine);
        }

        resultTextCoroutine =
            StartCoroutine(DisplayResultText(message, color));
    }

    private IEnumerator DisplayResultText(
        string message,
        Color color)
    {
        RectTransform textRect = resultText.rectTransform;

        resultText.text = message;
        resultText.color = color;
        resultText.gameObject.SetActive(true);

        textRect.anchoredPosition = feedbackStartPosition;
        textRect.localScale = feedbackNormalScale * startScale;

        if (resultCanvasGroup != null)
        {
            resultCanvasGroup.alpha = 1f;
        }

        float elapsed = 0f;

        while (elapsed < resultDisplayDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / resultDisplayDuration);

            textRect.anchoredPosition =
                feedbackStartPosition +
                Vector2.up * riseDistance * progress;

            textRect.localScale = Vector3.Lerp(
                feedbackNormalScale * startScale,
                feedbackNormalScale,
                progress);

            if (resultCanvasGroup != null)
            {
                resultCanvasGroup.alpha = 1f - progress;
            }

            yield return null;
        }

        textRect.localScale = feedbackNormalScale;
        textRect.anchoredPosition = feedbackStartPosition;

        if (resultCanvasGroup != null)
        {
            resultCanvasGroup.alpha = 0f;
        }

        resultText.gameObject.SetActive(false);
        resultTextCoroutine = null;
    }
}