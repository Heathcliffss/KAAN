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

    private List<Transform> targets = new List<Transform>();

    void Start()
    {
        // MinimapTarget component'i olan tüm objeleri bul
        MinimapTargets[] targetObjs = FindObjectsOfType<MinimapTargets>();
        foreach (MinimapTargets t in targetObjs)
        {
            targets.Add(t.transform);
        }
    }

    void Update()
    {
        // Eski blipleri temizle
        foreach (Transform child in minimapCircle)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform target in new List<Transform>(targets))
        {
            // 1️⃣ Obje sahnede yok mu?
            if (target == null)
            {
                targets.Remove(target);
                continue;
            }

            // 2️⃣ MinimapTarget component'i çıkarılmış mı?
            if (target.GetComponent<MinimapTargets>() == null)
            {
                targets.Remove(target);
                continue;
            }

            // Oyuncuya olan fark
            Vector3 offset = target.position - player.position;

            if (offset.magnitude > radarRange)
                continue;

            // Oyuncunun yönüne göre döndür
            Vector3 rotatedOffset = Quaternion.Euler(0, -player.eulerAngles.y, 0) * offset;

            float scaledX = Mathf.Clamp(rotatedOffset.x / radarRange, -1f, 1f);
            float scaledZ = Mathf.Clamp(rotatedOffset.z / radarRange, -1f, 1f);

            Vector2 minimapPos = new Vector2(scaledX, scaledZ) * (minimapCircle.rect.width / 2f);

            GameObject blip = Instantiate(enemyBlipPrefab, minimapCircle);
            RectTransform blipRect = blip.GetComponent<RectTransform>();
            blipRect.anchoredPosition = minimapPos;
        }
    }
}
