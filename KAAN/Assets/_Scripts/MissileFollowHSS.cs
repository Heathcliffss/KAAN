using UnityEngine;

public class MissileFollowHSS : MonoBehaviour
{
    public float baseExtraSpeed = 5f; // Uçağın hızına eklenecek sabit miktar
    public float rotateSpeed = 5f;
    public GameObject explosionEffect;

    private Transform target;
    private Rigidbody targetRb;

    public void SetTarget(Transform target)
    {
        this.target = target;
        targetRb = target.GetComponent<Rigidbody>(); // Uçağın rigidbody'sini al
    }

    private void Update()
    {
        if (target == null) return;

        // Hedefin hızına göre füze hızını ayarla
        float currentTargetSpeed = 0f;
        if (targetRb != null)
            currentTargetSpeed = targetRb.linearVelocity.magnitude; // Uçağın anlık hızı

        float currentMissileSpeed = currentTargetSpeed + baseExtraSpeed;

        // Yönelme
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotateTo = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, rotateSpeed * Time.deltaTime);

        // İlerleme
        transform.position += transform.forward * currentMissileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "DetectionArea") return;

        Debug.Log("Füze çarptı: " + other.name);

        var part = other.GetComponent<AircraftPart>();
        if (part != null)
        {
            Debug.Log("AircraftPart bulundu, hasar veriliyor.");
            part.TakeDamage();
        }
        else
        {
            Debug.Log("Çarpılan objede AircraftPart yok.");
        }

        Destroy(gameObject);
    }
}
