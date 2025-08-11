using UnityEngine;

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
        if (isReloading)
        {
            // Reload sırasında Mouse 0'a her basıldığında reload sesi
            if (Input.GetMouseButtonDown(0) && reloadSound != null)
                AudioSource.PlayClipAtPoint(reloadSound, firePoint.position, soundVolume);
            return;
        }

        // Normal ateş
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        // Mermi biterse reload başlat
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        if (currentAmmo <= 0) return;

        // Hedef yönü hesapla
        Vector3 targetPoint;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Crosshair ortası
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000); // 1000 metre ileriye varsayalım
        }

        // Mermiyi oluştur
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Yönünü hedefe çevir
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed;

        currentAmmo--;

        // Ateş sesi
        if (fireSound != null)
            AudioSource.PlayClipAtPoint(fireSound, firePoint.position, soundVolume);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;

        // Reload başlarken ses çal
        if (reloadSound != null)
            AudioSource.PlayClipAtPoint(reloadSound, firePoint.position, soundVolume);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
