using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void OnStartGame()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Ýsimsiz";

        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        SceneManager.LoadScene("Cem"); 
    }
}
