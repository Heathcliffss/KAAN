using UnityEngine;

public class EnemyChaseAI : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player;
    public Transform[] waypoints;
    private Rigidbody rb;

    [Header("Mesafeler")]
    public float viewDistance = 120f;
    public float loseDistance = 150f;
    public float waypointReach = 10f;

    [Header("Hýzlar")]
    public float patrolSpeed = 40f;
    public float chaseSpeed = 60f;
    public float rotationSpeed = 2f;

    private bool isChasing = false;
    private int wpIndex = 0;

    public GameObject missilePrefab;
    public Transform firePoint;
    public float missileCooldown = 3f;
    private float lastMissileTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isChasing && distToPlayer < viewDistance)
            isChasing = true;

        if (isChasing && distToPlayer > loseDistance)
            isChasing = false;

        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    void ChasePlayer()
    {
        FlyTowards(player.position, chaseSpeed);

        if (Vector3.Distance(transform.position, player.position) < 100f)
        {          
            FireMissile();
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWp = waypoints[wpIndex];
        FlyTowards(targetWp.position, patrolSpeed);

        if (Vector3.Distance(transform.position, targetWp.position) <= waypointReach)
        {
            wpIndex = (wpIndex + 1) % waypoints.Length;
        }
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

    void FireMissile()
    {
        if (Time.time - lastMissileTime < missileCooldown) return;
        lastMissileTime = Time.time;

        // Null kontroller
        if (RocketPool.Instance == null)
        {
            Debug.LogError(">>> RocketPool.Instance yok!");
            return;
        }

        GameObject missile = RocketPool.Instance.GetRocket();
        if (missile == null)
        {
            Debug.LogError(">>> Füze NULL! Pool'dan füze alýnamadý.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError(">>> FirePoint atanmadý!");
            return;
        }

        missile.transform.position = firePoint.position;
        missile.transform.rotation = firePoint.rotation;

        EnemyMissile em = missile.GetComponent<EnemyMissile>();
        if (em == null)
        {
            Debug.LogError(">>> EnemyMissile script füze prefabýnda yok!");
            return;
        }

        em.SetTarget(player);
    }
}
