using UnityEngine;

public class EnemyChaseAI : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player;               // Ana oyuncu objesi (pivot)
    public Transform playerTargetPoint;    // Oyuncunun vurulacak merkezi (örneğin kokpit boş obje)
    public Transform[] waypoints;
    private Rigidbody rb;

    [Header("Mesafeler")]
    public float viewDistance = 120f;
    public float loseDistance = 150f;
    public float waypointReach = 10f;

    [Header("Hızlar")]
    public float patrolSpeed = 40f;
    public float chaseSpeed = 60f;
    public float rotationSpeed = 2f;

    private bool isChasing = false;
    private int wpIndex = 0;

    [Header("Silah Sistemi")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 200f;
    public float fireCooldown = 0.2f;
    private float lastFireTime;

    [Header("Hasar Sistemi")]
    public int health = 5;
    public GameObject explosionPrefab;
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    public GameObject DeadBombs;


    private bool isDead = false;

    private Vector3 chaseOffset;

    public DusmanUcakKanat DusmanUcakKanatsol;
    public DusmanUcakKanat DusmanUcakKanatsag;

    public GameObject Marker;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        DeadBombs.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        if (DusmanUcakKanatsol.solKanat == 0)
        {
            rb.AddTorque(Vector3.forward * 2f, ForceMode.Impulse);
        }
        if (DusmanUcakKanatsag.sagKanat == 0)
        {
            rb.AddTorque(-Vector3.forward * 2f, ForceMode.Impulse);
        }

        // hedef noktayı seç → eğer atanmışsa targetPoint, yoksa normal player
        Transform targetTransform = playerTargetPoint != null ? playerTargetPoint : player;

        float distToPlayer = Vector3.Distance(transform.position, targetTransform.position);

        if (!isChasing && distToPlayer < viewDistance)
        {
            isChasing = true;
            chaseOffset = new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-20f, 20f),
                Random.Range(-50f, 50f)
            );
        }

        if (isChasing && distToPlayer > loseDistance)
            isChasing = false;

        if (isChasing)
            ChasePlayer(targetTransform);
        else
            Patrol();
    }

    void ChasePlayer(Transform targetTransform)
    {
        Vector3 chaseTarget = targetTransform.position + chaseOffset;

        FlyTowards(chaseTarget, chaseSpeed);

        Vector3 toPlayer = (targetTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle < 60f && Vector3.Distance(transform.position, targetTransform.position) < 400f)
            FireBullet(targetTransform);

        if (Random.value < 0.01f)
        {
            chaseOffset = new Vector3(
                Random.Range(-60f, 60f),
                Random.Range(-30f, 30f),
                Random.Range(-60f, 60f)
            );
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWp = waypoints[wpIndex];
        FlyTowards(targetWp.position, patrolSpeed);

        if (Vector3.Distance(transform.position, targetWp.position) <= waypointReach)
            wpIndex = (wpIndex + 1) % waypoints.Length;
    }

    void FlyTowards(Vector3 targetPos, float speed)
    {
        Vector3 targetDir = (targetPos - transform.position);
        Vector3 flatTargetDir = new Vector3(targetDir.x, 0f, targetDir.z).normalized;

        Quaternion targetYawRotation = Quaternion.LookRotation(flatTargetDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetYawRotation, rotationSpeed * Time.deltaTime);

        Vector3 localTargetDir = transform.InverseTransformDirection(flatTargetDir);
        float rollInput = Mathf.Clamp(-localTargetDir.x, -1f, 1f);
        float targetRoll = rollInput * 45f;

        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newZ = Mathf.Lerp(currentZ, targetRoll, Time.deltaTime * 3f);

        Quaternion rollRotation = Quaternion.Euler(0f, transform.eulerAngles.y, newZ);
        transform.rotation = rollRotation;

        float targetY = Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * 0.5f);
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

        rb.linearVelocity = transform.forward * speed;
    }

    void FireBullet(Transform targetTransform)
    {
        if (Time.time - lastFireTime < fireCooldown) return;
        lastFireTime = Time.time;

        if (bulletPrefab == null || firePoint == null) return;

        Vector3 dirToPlayer = (targetTransform.position - firePoint.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, lookRot);
        Rigidbody brb = bullet.GetComponent<Rigidbody>();
        if (brb != null)
            brb.linearVelocity = dirToPlayer * bulletSpeed;

        Destroy(bullet, 5f);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (health <= 0)
            Die();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GroundLayer"))
        {
            Fall();
            
        }
    }
    public void Die()
    {
        isDead = true;
        rb.useGravity = true;
        rb.mass += 200f;

        Marker.SetActive(false);

       // rb.AddExplosionForce(200f, transform.position, 1f);

       rb.AddForce(-transform.position * 200f);

       // rb.AddTorque(Vector3.forward * 600f, ForceMode.Impulse);

        // if (explosionPrefab != null)
        //     Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        DeadBombs.SetActive(true);

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // === YENİ: skor bildirimi ===
        GameManager.Instance?.OnEnemyAircraftEliminated();

        Destroy(gameObject, 5f);
    }

    void Fall()
    {
        isDead = true;
        rb.useGravity = true;
        

        

        DeadBombs.SetActive(true);

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // === YENİ: skor bildirimi ===
        

        Destroy(gameObject, 3f);
    }
}
