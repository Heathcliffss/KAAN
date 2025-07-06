using UnityEngine;

public class AirDefenseSystem : MonoBehaviour
{
    public Transform firePoint;
    public GameObject missilePrefab;
    public float fireInterval = 2f;

    [HideInInspector] public Transform target;
    private float timer = 0f;
    private bool canShoot = false;

    void Update()
    {
        if (!canShoot || target == null) return;

        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            timer = 0f;
            ShootMissile();
        }
    }

    public void StartFiringAt(Transform target)
    {
        this.target = target;
        canShoot = true;
    }

    public void StopFiring()
    {
        canShoot = false;
    }

    void ShootMissile()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
        MissileFollow follow = missile.GetComponent<MissileFollow>();
        if (follow != null)
        {
            follow.target = target;
        }

        Debug.Log("🚀 Missile fired at: " + Time.time);
    }
}
