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
        // Etiketi "Enemy" olan t?m d??manlar? bul
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyObjs)
        {
            enemies.Add(obj.transform);
        }
    }

    void Update()
    {
        // ?nce eski blip'leri temizle
        foreach (Transform child in minimapCircle)
        {
            Destroy(child.gameObject);
        }



        foreach (Transform enemy in new List<Transform>(enemies))
        {
            if (enemy == null) // yok edilmi? mi?
            {
                enemies.Remove(enemy);
                continue;
            }

            Vector3 offset = enemy.position - player.position;

            if (offset.magnitude > radarRange)
                continue;

            float scaledX = Mathf.Clamp(offset.x / radarRange, -1f, 1f);
            float scaledZ = Mathf.Clamp(offset.z / radarRange, -1f, 1f);

            Vector2 minimapPos = new Vector2(scaledX, scaledZ) * (minimapCircle.rect.width / 2f);

            GameObject blip = Instantiate(enemyBlipPrefab, minimapCircle);
            RectTransform blipRect = blip.GetComponent<RectTransform>();
            blipRect.anchoredPosition = minimapPos;
        }

    }
}
