using UnityEngine;

public class NoteTargetLayout : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private NoteTarget[] targets;
    [SerializeField] private float targetY = -3f;

    private void Start()
    {
        PositionTargets();
    }

    public void PositionTargets()
    {
        if (inputManager == null || Camera.main == null)
        {
            Debug.LogError(
                "InputManager or Main Camera is missing.");

            return;
        }

        int railCount = inputManager.railCount;

        for (int rail = 0; rail < targets.Length; rail++)
        {
            if (targets[rail] == null)
            {
                continue;
            }

            bool shouldBeActive = rail < railCount;
            targets[rail].gameObject.SetActive(shouldBeActive);

            if (!shouldBeActive)
            {
                continue;
            }

            float railWidth = Screen.width / (float)railCount;
            float screenX = (rail + 0.5f) * railWidth;
            float cameraDepth = -Camera.main.transform.position.z;

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(
                new Vector3(
                    screenX,
                    Screen.height / 2f,
                    cameraDepth));

            targets[rail].transform.position =
                new Vector3(worldPoint.x, targetY, 0f);

            targets[rail].Initialize(rail);
        }
    }
}