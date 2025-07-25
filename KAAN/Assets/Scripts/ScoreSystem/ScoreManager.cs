using System.IO;
using System.Linq;
using UnityEngine;

public static class ScoreManager
{
    public static string FilePath => Application.persistentDataPath + "/scores.json";

    public static void SaveScore(string name, int score)
    {
        ScoreData data = LoadScores();

        data.scores.Add(new ScoreEntry { playerName = name, score = score });
        data.scores = data.scores.OrderByDescending(s => s.score).ToList(); // Skora g�re s�rala

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);
    }

    public static ScoreData LoadScores()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }

        return new ScoreData(); // �lk kez �al���yorsa bo� liste d�ner
    }
}
