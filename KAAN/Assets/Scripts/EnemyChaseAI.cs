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

    [Header("H�zlar")]
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
        // 1. Hedef y�n (dikey fark� yok say�larak)
        Vector3 targetDir = (targetPos - transform.position);
        Vector3 flatTargetDir = new Vector3(targetDir.x, 0f, targetDir.z).normalized;

        // 2. Hedef rotasyon (yaw i�in)
        Quaternion targetYawRotation = Quaternion.LookRotation(flatTargetDir, Vector3.up);

        // 3. Yava��a hedefe d�n (Y ekseninde)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetYawRotation, rotationSpeed * Time.deltaTime);

        // 4. Yat�� (roll) efekti � hedef y�n lokal uzayda hangi y�ne? Sa� m� sol mu?
        Vector3 localTargetDir = transform.InverseTransformDirection(flatTargetDir);
        float rollInput = Mathf.Clamp(-localTargetDir.x, -1f, 1f); // sola negatif, sa�a pozitif
        float targetRoll = rollInput * 45f;

        // 5. �u anki Z a��s� (roll) hesapla
        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newZ = Mathf.Lerp(currentZ, targetRoll, Time.deltaTime * 3f);

        // 6. Rotation�a roll�u uygula (Y zaten d�n�yor, Z�yi ekliyoruz)
        Quaternion rollRotation = Quaternion.Euler(0f, transform.eulerAngles.y, newZ);
        transform.rotation = rollRotation;

        // 7. Hedefin y�ksekli�ine do�ru yumu�ak ge�i�
        float targetY = Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * 0.5f);
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

        // 8. Rigidbody ile ileri do�ru sabit h�zda hareket
        rb.linearVelocity = transform.forward * speed;
    }

    void FireMissile()
    {
        if (Time.time - lastMissileTime < missileCooldown) return;
        lastMissileTime = Time.time;

        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        missile.GetComponent<EnemyMissile>().SetTarget(player);
    }

}
