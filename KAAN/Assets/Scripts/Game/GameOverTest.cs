using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTest : MonoBehaviour
{
    public int testScore = 1234;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            string name = PlayerPrefs.GetString("CurrentPlayerName", "Ýsimsiz");
            ScoreManager.SaveScore(name, 1234); // Örnek skor
            SceneManager.LoadScene("MainMenu");
        }
    }
}
