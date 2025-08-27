using UnityEngine;

public class MissileFollowHSS : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float baseExtraSpeed = 5f; // Uçağın hızına eklenecek sabit miktar
    public float rotateSpeed = 5f;

    [Header("Efektler")]
    public GameObject explosionEffect;
    public AudioSource missileIdleSound;   // Uçuş sesi
    public AudioSource explosionSound;     // Patlama sesi

    [Header("Yaşam Süresi")]
    public float lifetime = 4f; // Füze ömrü (saniye)

    private Transform target;
    private Rigidbody targetRb;

    void Start()
    {
        // Idle sesi başlat
        if (missileIdleSound != null)
        {
            missileIdleSound.loop = true;
            missileIdleSound.Play();
        }

        // 4 saniye sonra otomatik yok et
        Destroy(gameObject, lifetime);
    }

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

        // Idle sesi kapat
        if (missileIdleSound != null)
            missileIdleSound.Stop();

        // Patlama efekti
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Patlama sesi
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound.clip, transform.position);

        Destroy(gameObject);
    }
}
