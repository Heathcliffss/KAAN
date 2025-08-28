using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Transform target;         // Bu marker'ın üstünde duracağı düşman uçağı
    public Camera playerCamera;      // Oyuncunun ana kamerası
    public float detectionRange = 500f; // Kaç metreden sonra gözüksün
    public Image markerImage;        // UI image (ikon)

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (markerImage != null)
            markerImage.enabled = false; // başta kapalı
    }

    void Update()
    {
        if (target == null || playerCamera == null) return;

        float distance = Vector3.Distance(playerCamera.transform.position, target.position);

        // Mesafe kontrolü
        if (distance <= detectionRange)
        {
            if (markerImage != null && !markerImage.enabled)
                markerImage.enabled = true;

            // Kamera yönüne bakması için
            transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward,
                             playerCamera.transform.rotation * Vector3.up);
        }
        else
        {
            if (markerImage != null && markerImage.enabled)
                markerImage.enabled = false;
        }
    }
}
