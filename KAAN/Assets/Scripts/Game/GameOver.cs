using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public int playerScore; // Bu skoru oyun bitince atýyorsun

    public void GameOverNow()
    {
        string playerName = PlayerPrefs.GetString("CurrentPlayerName", "Ýsimsiz");
        ScoreManager.SaveScore(playerName, playerScore);

        SceneManager.LoadScene("MainMenu");
    }
}
