using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(JetEngineSoundController))]
public class AircraftGun : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 50;
    public float reloadTime = 3f;

    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    bool PressedFireButton = false;

    public GameManager gameManager;
    public Transform L3FirePosition;
    private Transform FirePosition;


    // Sesleri buradan çalacağız
    private JetEngineSoundController soundHub;

    void Awake()
    {
        // Aynı objede yoksa parent’ta arar
        soundHub = GetComponent<JetEngineSoundController>();
        if (soundHub == null) soundHub = GetComponentInParent<JetEngineSoundController>();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading) return;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                PressedFireButton = true;
            }
            else { PressedFireButton = false; }
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime || PressedFireButton && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        if (currentAmmo <= 0 && !isReloading)
            StartCoroutine(Reload());
    }

    void Fire()
    {
        if (currentAmmo <= 0 || firePoint == null || bulletPrefab == null) return;

        // 🎯 hedef noktası (ekran ortası crosshair)
        Vector3 targetPoint;
        var cam = Camera.main;
        if (cam != null)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(1000f);
        }
        else
        {
            targetPoint = firePoint.position + firePoint.forward * 1000f;
        }

        // Mermiyi oluştur
        if (!gameManager.l3)
        { FirePosition = firePoint; }
        else {  FirePosition = L3FirePosition; }
            
            
            GameObject bullet = Instantiate(bulletPrefab, FirePosition.position, Quaternion.identity);
        

        
        var rb = bullet.GetComponent<Rigidbody>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Yönlendirme
        Vector3 dir = (targetPoint - firePoint.position).normalized;
        rb.linearVelocity = dir * bulletSpeed;

        // Collider ayarları
        var col = bullet.GetComponent<Collider>();
        if (col == null) col = bullet.AddComponent<SphereCollider>();
        col.isTrigger = false;

        // Kendi uçak colliderlarını ignore et
        foreach (var oc in GetComponentsInChildren<Collider>())
            if (oc != null) Physics.IgnoreCollision(col, oc, true);

        // Bullet davranışı + ses merkezi referansı
        var bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.soundHub = soundHub;

        currentAmmo--;

        // 🔊 Ateş sesi merkezden
        soundHub?.PlayFireSound(firePoint.position);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // 🔊 Reload sesi merkezden
        soundHub?.PlayReloadSound(firePoint.position);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    // ✅ Mermi davranışı bu scriptin içinde
    public class Bullet : MonoBehaviour
    {
        public JetEngineSoundController soundHub; // vurma sesini buradan çalacağız
        public float lifeTime = 5f;
        public int Damage = 1;
        public EnemyChaseAI enemyChaseAI;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (Camera.main != null)
                    soundHub?.PlayHitSound(Camera.main.transform.position);

                
                var enemy = other.GetComponent<EnemyChaseAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(Damage);
                }

                Destroy(gameObject);
            }
        }
    }
}
