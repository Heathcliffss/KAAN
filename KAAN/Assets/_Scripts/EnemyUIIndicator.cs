using UnityEngine;
using UnityEngine.UI;

public class EnemyUIIndicator : MonoBehaviour
{
    public Image indicatorUI;   // UI image (Canvas içinde world space veya uçaðýn üstünde küçük UI)
    public float detectionDistance = 200f;

    private Transform player;
    private Camera mainCam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // oyuncu uçaðýný tagle
        mainCam = Camera.main; // Cinemachine main cam'ini buradan alabiliriz
        indicatorUI.enabled = false; // baþta kapalý
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Mesafe kontrolü
        if (dist <= detectionDistance)
        {
            indicatorUI.enabled = true;

            // UI her zaman kameraya baksýn
            indicatorUI.transform.LookAt(mainCam.transform);
            indicatorUI.transform.Rotate(0, 180, 0); // ters dönmemesi için
        }
        else
        {
            indicatorUI.enabled = false;
        }
    }
}
