using UnityEngine;

public class AirDefenseSystem : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float detectionRange = 500f;

    [Header("Ses Ayarları")]
    public AudioClip explosionClip;   // Patlama sesi (Inspector’dan atayacaksın)

    private float fireCooldown;
    private Transform target;
    private bool isFiring = false;

    public void StartFiringAt(Transform newTarget)
    {
        if (newTarget == null) return;
        target = newTarget;
        isFiring = true;
        Debug.Log("AirDefenseSystem: StartFiringAt -> " + newTarget.name);
    }

    public void StopFiring()
    {
        isFiring = false;
        target = null;
        Debug.Log("AirDefenseSystem: StopFiring");
    }

    private void Update()
    {
        if (!isFiring || target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > detectionRange)
        {
            StopFiring();
            return;
        }

        if (Time.time >= fireCooldown)
        {
            FireMissile();
            fireCooldown = Time.time + fireRate;
        }
    }

    private void FireMissile()
    {
        if (missilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("AirDefenseSystem: missilePrefab veya firePoint atanmadı.");
            return;
        }

        GameObject missileObj = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        var missile = missileObj.GetComponent<MissileFollowHSS>();
        if (missile != null)
            missile.SetTarget(target);

        Debug.Log("AirDefenseSystem: Missile fired at " + (target != null ? target.name : "null"));
    }

    // --- YENİ EKLENEN KISIM ---
    // --- YENİ EKLENEN KISIM ---
    private bool isDestroyed = false;

    public void DestroySystem()
    {
        if (isDestroyed) return; // tek tetikleme güvenliği
        isDestroyed = true;

        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, transform.position);
            Debug.Log("AirDefenseSystem: Patlama sesi çaldı.");
        }

        // === YENİ: skor bildirimi ===
        GameManager.Instance?.OnHSSEliminated();

        Destroy(gameObject);
    }

}
