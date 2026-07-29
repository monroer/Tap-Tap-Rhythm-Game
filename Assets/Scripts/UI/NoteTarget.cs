using System.Collections;
using UnityEngine;

public class NoteTarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private int rail;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float missScale = 0.85f;

    [Header("Colors")]
    [SerializeField] private Color[] railColors =
    {
        new Color32(232, 84, 89, 255),   // Red
        new Color32(233, 200, 63, 255),  // Yellow
        new Color32(77, 186, 115, 255),  // Green
        new Color32(77, 125, 219, 255)   // Blue
    };
    
    [SerializeField] private float hitFlashScale = 1.35f;

    [Header("Tap Animation")]
    [SerializeField] private float pressedScale = 1.2f;
    [SerializeField] private float animationDuration = 0.1f;

    private Vector3 normalScale;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        normalScale = transform.localScale;
    }

    private void Start()
    {
        SetTargetColor();
    }

    public void Initialize(int railIndex)
    {
        rail = railIndex;
        SetTargetColor();
    }
    public void PlayMissAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateMiss());
    }
    private IEnumerator AnimateMiss()
    {
        Color originalColor = targetRenderer.color;

        targetRenderer.color = Color.red;
        transform.localScale = normalScale * missScale;

        yield return new WaitForSeconds(0.1f);

        targetRenderer.color = originalColor;
        transform.localScale = normalScale;
        animationCoroutine = null;
    }

    private void SetTargetColor()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning(
                $"No SpriteRenderer found on target {gameObject.name}.");

            return;
        }

        if (railColors == null ||
            rail < 0 ||
            rail >= railColors.Length)
        {
            targetRenderer.color = Color.white;
            return;
        }

        targetRenderer.color = railColors[rail];
    }

    public void PlayTapAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateTap());
    }

    private IEnumerator AnimateTap()
    {
        transform.localScale = normalScale * pressedScale;

        yield return new WaitForSeconds(animationDuration);

        transform.localScale = normalScale;
        animationCoroutine = null;
    }
    
    public void PlayHitAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateHit());
    }

    private IEnumerator AnimateHit()
    {
        Color originalColor = targetRenderer.color;

        targetRenderer.color = Color.white;
        transform.localScale = normalScale * hitFlashScale;

        yield return new WaitForSeconds(0.08f);

        targetRenderer.color = originalColor;
        transform.localScale = normalScale;
        animationCoroutine = null;
    }
}