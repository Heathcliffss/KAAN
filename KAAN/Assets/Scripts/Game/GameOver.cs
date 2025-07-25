using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public int playerScore; // Bu skoru oyun bitince at�yorsun

    public void GameOverNow()
    {
        string playerName = PlayerPrefs.GetString("CurrentPlayerName", "�simsiz");
        ScoreManager.SaveScore(playerName, playerScore);

        SceneManager.LoadScene("MainMenu");
    }
}
