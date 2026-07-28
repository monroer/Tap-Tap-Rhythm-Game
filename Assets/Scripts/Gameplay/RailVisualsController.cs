using UnityEngine;

public class RailVisualsController : MonoBehaviour
{
    [SerializeField] private BeatMapManager beatMapManager;
    [SerializeField] private GameObject target4;

    private void Start()
    {
        if (beatMapManager == null || target4 == null) return;
        target4.SetActive(beatMapManager.RailCount == 4);
    }
}
