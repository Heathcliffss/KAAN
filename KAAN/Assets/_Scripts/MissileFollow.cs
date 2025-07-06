using UnityEngine;

public class MissileFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 15f;
    public float maxLifetime = 10f;

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
            // Artık hedefe gitmiyor, rastgele hareket ediyor
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
            randomDirection.y = Mathf.Clamp(randomDirection.y, -0.1f, 0.2f); // Uçuş yüksekliğini sınırlayarak daha iyi kontrol
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("💥 Missile hit the aircraft!");
            Destroy(gameObject);
        }
    }
}
