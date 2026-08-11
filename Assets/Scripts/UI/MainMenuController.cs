using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Toggle fourRailToggle;
    [SerializeField] private TMP_Dropdown songDropdown;

    public void OnPlayPressed()
    {
        if(gameManager == null) return;

        RailMode selectedRailMode = (fourRailToggle != null && fourRailToggle.isOn)
            ? RailMode.FourRail
            : RailMode.ThreeRail;

        SongMode selectedSongMode = (songDropdown != null && songDropdown.value == 1)
            ? SongMode.SongB
            : SongMode.SongA;

        gameManager.StartGame(selectedRailMode, selectedSongMode);
    }
}
