using TMPro;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // UI text
    [SerializeField] private float countdownTime = 120f; // 2 dakika = 120 saniye

    private float currentTime;

    void Start()
    {
        currentTime = countdownTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // zamaný azalt
            if (currentTime < 0) currentTime = 0;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
