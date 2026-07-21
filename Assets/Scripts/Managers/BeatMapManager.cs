using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct NoteData
{
    public float time;
    public int rail;
}

public class BeatMapManager : MonoBehaviour
{
    [SerializeField]
    private List<NoteData> notes = new List<NoteData>
    {
        new NoteData { time = 1.0f, rail = 0},
        new NoteData { time = 1.5f, rail = 2},
        new NoteData { time = 2.0f, rail = 1},
        new NoteData { time = 2.5f, rail = 3},
        new NoteData { time = 3.0f, rail = 0},
    };

    public IReadOnlyList<NoteData> Notes => notes;

}