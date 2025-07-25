using UnityEngine;
using UnityEngine.UI;

public class ScoreListUI : MonoBehaviour
{
    public Transform listParent;           // ScrollView i�indeki Content objesi
    public GameObject scoreEntryPrefab;    // Tek sat�rl�k skor prefab�

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
