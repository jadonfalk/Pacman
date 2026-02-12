using UnityEngine;
using TMPro;

public static class HighScoreManager
{
    
    private const int maxHighScores = 5;

    public static string[] Names = new string[maxHighScores];
    public static int[] Scores = new int[maxHighScores];

    public static void Load()
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            Names[i] = PlayerPrefs.GetString($"HS_Name_{i}", "---");
            Scores[i] = PlayerPrefs.GetInt($"HS_Value_{i}", 0);
        }
    }

    public static void Save()
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            PlayerPrefs.SetString($"HS_Name_{i}", Names[i]);
            PlayerPrefs.SetInt($"HS_Value_{i}", Scores[i]);
        }
        PlayerPrefs.Save();
    }

    public static bool TryInsert(string name, int newScore)
    {
        for (int i = 0; i < maxHighScores; i++)
        {
            if (newScore > Scores[i])
            {
                // Shift down
                for (int j = maxHighScores - 1; j > i; j--)
                {
                    Scores[j] = Scores[j - 1];
                    Names[j] = Names[j - 1];
                }

                Scores[i] = newScore;
                Names[i] = name;
                return true;
            }
        }
        return false;
    }
}
