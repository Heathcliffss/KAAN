using UnityEngine;
using UnityEngine.UI;

public class ScoreListUI : MonoBehaviour
{
    public Transform listParent;           // ScrollView içindeki Content objesi
    public GameObject scoreEntryPrefab;    // Tek satýrlýk skor prefabý

    void Start()
    {
        var data = ScoreManager.LoadScores();

        foreach (var entry in data.scores)
        {
            GameObject item = Instantiate(scoreEntryPrefab, listParent);
            item.GetComponent<Text>().text = $"{entry.playerName} - {entry.score}";
        }
    }
}
