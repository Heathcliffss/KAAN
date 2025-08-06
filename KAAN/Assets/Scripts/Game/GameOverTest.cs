using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTest : MonoBehaviour
{
    public int testScore = 51623576;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            string name = PlayerPrefs.GetString("CurrentPlayerName", "Ýsimsiz");
            ScoreManager.SaveScore(name, testScore); // Örnek skor
            SceneManager.LoadScene("MainMenu");
        }
    }
}
