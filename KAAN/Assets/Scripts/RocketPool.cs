using System.Collections.Generic;
using UnityEngine;

public class RocketPool : MonoBehaviour
{
    public static RocketPool Instance;

    public GameObject rocketPrefab;
    private List<GameObject> rockets = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetRocket()
    {
        // Geçersiz (silinmiþ) objeleri temizle
        rockets.RemoveAll(rocket => rocket == null);

        foreach (var rocket in rockets)
        {
            if (rocket != null && !rocket.activeInHierarchy)
            {
                rocket.SetActive(true);
                return rocket;
            }
        }

        GameObject newRocket = Instantiate(rocketPrefab);
        rockets.Add(newRocket);
        return newRocket;
    }

    public void ReturnRocket(GameObject rocket)
    {
        if (rocket != null)
            rocket.SetActive(false);
    }
}