using UnityEngine;

public class NameAndScore : MonoBehaviour
{
    public static NameAndScore Instance;
    public int score = 0;

    public MainMenuUI mainMenu;

    public static string playerNameStatic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Sahne deðiþiminde kopya oluþmasýný engelle
        }

        //playerNameStatic = mainMenu.playerName;
    }

  

    
    void Update()
    {
        Debug.Log(playerNameStatic);
        
    }
}
