using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";
    private const string LastScoreKey = "LastScore";
    private const string TotalHitsKey = "TotalHits";
    private const string TotalMissesKey = "TotalMisses";
    private const string LongestStreakKey = "LongestStreak";
    private const string AccuracyKey = "Accuracy";

    public int HighScore => PlayerPrefs.GetInt(HighScoreKey, 0);
    public int LastScore => PlayerPrefs.GetInt(LastScoreKey, 0);
    public int TotalHits => PlayerPrefs.GetInt(TotalHitsKey, 0);
    public int TotalMisses => PlayerPrefs.GetInt(TotalMissesKey, 0);
    public int LongestStreak => PlayerPrefs.GetInt(LongestStreakKey, 0);
    public float Accuracy => PlayerPrefs.GetFloat(AccuracyKey, 0f);

    public void SaveSession(
        int score,
        int hits,
        int misses,
        int longestStreak,
        float accuracy)
    {
        PlayerPrefs.SetInt(LastScoreKey, score);
        PlayerPrefs.SetInt(TotalHitsKey, hits);
        PlayerPrefs.SetInt(TotalMissesKey, misses);
        PlayerPrefs.SetInt(LongestStreakKey, longestStreak);
        PlayerPrefs.SetFloat(AccuracyKey, accuracy);

        if (score > HighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
        }

        PlayerPrefs.Save();
    }

    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.DeleteKey(LastScoreKey);
        PlayerPrefs.DeleteKey(TotalHitsKey);
        PlayerPrefs.DeleteKey(TotalMissesKey);
        PlayerPrefs.DeleteKey(LongestStreakKey);
        PlayerPrefs.DeleteKey(AccuracyKey);

        PlayerPrefs.Save();
    }
}