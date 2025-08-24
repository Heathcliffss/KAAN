using UnityEngine;
using System.Collections;

public class AircraftGun : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 50;
    public float reloadTime = 3f;

    [Header("Ses Ayarları")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip hitSound;
    public float soundVolume = 1f;

    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        if (currentAmmo <= 0 && !isReloading)
            StartCoroutine(Reload());
    }

    void Fire()
    {
        if (currentAmmo <= 0) return;

        // 🎯 Önce mouse'un baktığı noktayı bul
        Vector3 targetPoint;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ekran ortası
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000f);

        // Mermiyi oluştur
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Rigidbody ve collider kontrolü
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // 🎯 yönlendirme: hedef noktasına doğru
        Vector3 dir = (targetPoint - firePoint.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;

        var col = bullet.GetComponent<Collider>();
        if (col == null) col = bullet.AddComponent<SphereCollider>();
        col.isTrigger = false;

        // Kendi uçağına çarpmasın
        var ownerCols = GetComponentsInChildren<Collider>();
        foreach (var oc in ownerCols)
            if (oc != null && col != null)
                Physics.IgnoreCollision(col, oc, true);

        // Mermi davranışı
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.hitSound = hitSound;
        bulletScript.soundVolume = soundVolume;

        currentAmmo--;

        if (fireSound != null)
            AudioSource.PlayClipAtPoint(fireSound, firePoint.position, soundVolume);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (reloadSound != null)
            AudioSource.PlayClipAtPoint(reloadSound, firePoint.position, soundVolume);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    // ✅ Mermi davranışı bu scriptin içinde
    public class Bullet : MonoBehaviour
    {
        public AudioClip hitSound;
        public float soundVolume = 1f;
        public float lifeTime = 5f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hitSound != null && Camera.main != null)
                    AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position, soundVolume);

                var enemy = collision.gameObject.GetComponent<EnemyChaseAI>();
                if (enemy != null)
                    enemy.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }
}
