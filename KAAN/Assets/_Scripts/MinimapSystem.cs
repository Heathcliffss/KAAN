using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapSystem : MonoBehaviour
{
    [Header("Minimap Settings")]
    public Transform player;
    public RectTransform minimapPanel;
    public RectTransform minimapCircle;
    public GameObject enemyBlipPrefab;

    [Header("World Settings")]
    public float radarRange = 500f;

    private List<Transform> enemies = new List<Transform>();

    void Start()
    {
        // Etiketi "Enemy" olan tüm düþmanlarý bul
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyObjs)
        {
            enemies.Add(obj.transform);
        }
    }

    void Update()
    {
        // Önce eski blip'leri temizle
        foreach (Transform child in minimapCircle)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform enemy in enemies)
        {
            Vector3 offset = enemy.position - player.position;

            if (offset.magnitude > radarRange)
                continue;

            // Minimap alanýna ölçekle
            float scaledX = Mathf.Clamp(offset.x / radarRange, -1f, 1f);
            float scaledZ = Mathf.Clamp(offset.z / radarRange, -1f, 1f);

            Vector2 minimapPos = new Vector2(scaledX, scaledZ) * (minimapCircle.rect.width / 2f);

            // Yeni blip oluþtur
            GameObject blip = Instantiate(enemyBlipPrefab, minimapCircle);
            RectTransform blipRect = blip.GetComponent<RectTransform>();
            blipRect.anchoredPosition = minimapPos;
        }
    }
}
