using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListUI : MonoBehaviour
{
    public Transform listParent;
    public GameObject scoreEntryPrefab;

    void Start()
    {
        ScoreData scoreData = ScoreManager.LoadScores();
        List<ScoreEntry> scores = scoreData.scores;

        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject); // eski satýrlarý sil
        }

        foreach (var entry in scores)
        {
            GameObject go = Instantiate(scoreEntryPrefab, listParent);
            go.GetComponent<TMP_Text>().text = $"{entry.playerName} - {entry.score}";
        }
    }
}
