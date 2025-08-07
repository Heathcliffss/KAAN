using UnityEngine;

public class MissileFollowHSS : MonoBehaviour
{
    public Transform target;
    public float speed = 15f;
    public float maxLifetime = 10f;

    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public AudioSource audioSource;

    private bool isTracking = true;
    private Vector3 randomDirection;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    void Update()
    {
        if (isTracking && target != null)
        {
            // Hedefe doğru gider
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
        }
        else
        {
            // Rastgele hareket
            transform.position += randomDirection * speed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, randomDirection, Time.deltaTime * 2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "HitArea")
        {
            Debug.Log("🔄 Missile entered TrackingZone — stop tracking!");
            isTracking = false;
            randomDirection = Random.onUnitSphere;
            randomDirection.y = Mathf.Clamp(randomDirection.y, -0.1f, 0.2f); // Daha kontrollü yükseklik
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Missile hit the aircraft!");

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            if (audioSource != null && explosionSound != null)
                audioSource.PlayOneShot(explosionSound);

            Destroy(gameObject);
        }
    }
}
