using System.Collections;
using UnityEngine;

public class BombFollow : MonoBehaviour
{
    public float followDelay = 2f;          // Bombanýn takip etmeye baþlamadan önceki bekleme süresi
    public float followForce = 5000f;       // Takip ederken uygulanan kuvvet
    public GameObject explosionEffect;      // Patlama efekti prefab

    private Transform enemy;
    private Rigidbody rb;
    private bool following;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ýlk bulduðu Enemy'yi hedef alýyor
        GameObject targetObj = GameObject.FindGameObjectWithTag("Enemy");
        if (targetObj != null)
        {
            enemy = targetObj.transform;
        }

        StartCoroutine(StartFollowingAfterDelay(followDelay));
    }

    void Update()
    {
        if (following && enemy != null)
        {
            // Hedefin yönünü hesapla
            Vector3 directionToEnemy = (enemy.position - transform.position).normalized;

            // Füzenin ileri yönü ile hedef arasýndaki açýyý hesapla
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

            // Eðer hedef çok dik açýdaysa takip etmeyi býrak
            if (angleToEnemy < 180f)
            {
                rb.AddForce(directionToEnemy * followForce * Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Düþmaný yok et
            Destroy(other.gameObject);

            // Patlama efekti oluþtur
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            // Bombayý yok et
            Destroy(gameObject);
        }
    }

    IEnumerator StartFollowingAfterDelay(float delay)
    {
        following = false;
        yield return new WaitForSeconds(delay);
        following = true;
    }
}
