using UnityEngine;

public class AirDefenseSystem : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float detectionRange = 500f;

    private float fireCooldown;
    private Transform target;
    private bool isFiring = false;

    // Bu iki metot TriggerTest tarafından çağrılıyor — bunların PUBLIC ve doğru imzada olması şart.
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

    // Bu triggerlar opsiyonel — TriggerTest zaten Start/Stop çağırıyorsa gerek yok.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // otomatik başlatmak istersen burayı aç
            // StartFiringAt(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // StopFiring();
        }
    }
}
