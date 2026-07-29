using UnityEngine;

public class RailTargetLayout : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private InputManager inputManager;

    [Header("Target Markers")]
    [Tooltip("Assign the target markers in rail order from left to right.")]
    [SerializeField] private Transform[] targetMarkers;

    [Header("Positioning")]
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private float targetWorldY = -3f;

    private int lastRailCount = -1;
    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;

    private void Awake()
    {
        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }
    }

    private void Start()
    {
        RefreshLayout();
    }

    private void LateUpdate()
    {
        if (inputManager == null)
        {
            return;
        }

        if (inputManager.railCount != lastRailCount ||
            Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight)
        {
            RefreshLayout();
        }
    }

    public void RefreshLayout()
    {
        if (inputManager == null || gameplayCamera == null || targetMarkers == null)
        {
            return;
        }

        int railCount = Mathf.Max(1, inputManager.railCount);

        for (int i = 0; i < targetMarkers.Length; i++)
        {
            Transform marker = targetMarkers[i];
            if (marker == null)
            {
                continue;
            }

            bool shouldBeActive = i < railCount;
            marker.gameObject.SetActive(shouldBeActive);

            if (!shouldBeActive)
            {
                continue;
            }

            float railWidth = Screen.width / (float)railCount;
            float screenX = (i + 0.5f) * railWidth;

            // Convert the rail center from screen space to the same world-space
            // horizontal position used by the note lanes.
            Vector3 screenPoint = new Vector3(
                screenX,
                gameplayCamera.WorldToScreenPoint(
                    new Vector3(0f, targetWorldY, 0f)).y,
                Mathf.Abs(gameplayCamera.transform.position.z)
            );

            Vector3 worldPoint = gameplayCamera.ScreenToWorldPoint(screenPoint);
            marker.position = new Vector3(worldPoint.x, targetWorldY, marker.position.z);
        }

        lastRailCount = inputManager.railCount;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}