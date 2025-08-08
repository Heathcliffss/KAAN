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

    private void Update()
    {
        if (isFiring && target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= detectionRange)
            {
                FireMissile();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFiringAt(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopFiring();
        }
    }

    public void StartFiringAt(Transform newTarget)
    {
        target = newTarget;
        isFiring = true;
    }

    public void StopFiring()
    {
        target = null;
        isFiring = false;
    }

    private void FireMissile()
    {
        if (Time.time >= fireCooldown)
        {
            GameObject missileObj = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            MissileFollowHSS missile = missileObj.GetComponent<MissileFollowHSS>();
            if (missile != null)
            {
                missile.SetTarget(target);
            }

            fireCooldown = Time.time + fireRate;
        }
    }
}
