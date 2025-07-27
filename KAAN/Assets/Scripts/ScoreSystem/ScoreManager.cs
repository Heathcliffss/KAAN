using System.IO;
using System.Linq;
using UnityEngine;

public static class ScoreManager
{
    public static string FilePath => Application.persistentDataPath + "/scores.json";

    public static void SaveScore(string playerName, int score)
    {
        ScoreData data = LoadScores();

        data.scores.Add(new ScoreEntry
        {
            playerName = playerName,
            score = score
        });

        // Sýralama: Skor büyükten küçüðe
        data.scores.Sort((a, b) => b.score.CompareTo(a.score));

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }


    public static ScoreData LoadScores()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }

        return new ScoreData(); // Ýlk kez çalýþýyorsa boþ liste döner
    }
}
