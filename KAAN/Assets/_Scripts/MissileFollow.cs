using UnityEngine;

public class MissileFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 15f;
    public float maxLifetime = 10f;

    private bool isTracking = true;
    private Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, maxLifetime);

        // İlk hareket yönü hedefe doğru
        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        // Sadece izleme aktifken yönü hedefe çevir
        if (isTracking && target != null)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * 2f);
        }

        transform.forward = moveDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HitArea") && isTracking)
        {
            float missChance = 0.3f;
            if (Random.value < missChance)
            {
                Debug.Log("🚫 Missile lost tracking — will miss.");
                isTracking = false;
                // Mevcut yönünde devam et (moveDirection korunuyor)
            }
            else
            {
                Debug.Log("🎯 Missile continues tracking.");
            }
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Missile hit the aircraft!");
            Destroy(gameObject);
        }
    }
}
