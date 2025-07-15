using UnityEngine;

public class EnemyChaseAI : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player;            // Oyuncu u�a��
    public Transform[] waypoints;       // Devriye noktalar�

    [Header("Mesafeler")]
    public float viewDistance = 120f; // Oyuncuyu g�rme
    public float loseDistance = 150f; // Bu mesafenin �st�nde kovalama biter
    public float waypointReach = 10f;  // Waypoint�e varm�� say�laca�� yar��ap

    [Header("H�zlar")]
    public float patrolSpeed = 40f;
    public float chaseSpeed = 60f;
    public float rotationSpeed = 2f;

    private bool isChasing = false;
    private int wpIndex = 0;         // Aktif waypoint

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // G�r�� alan�na girerse kovalamaya ba�la
        if (!isChasing && distToPlayer < viewDistance)
            isChasing = true;

        // Uzakla��rsa kovalamay� b�rak
        if (isChasing && distToPlayer > loseDistance)
            isChasing = false;

        // Davran�� se�imi
        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    // ------------ Takip ------------
    void ChasePlayer()
    {
        FlyTowards(player.position, chaseSpeed);
    }

    // ------------ Devriye ------------
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWp = waypoints[wpIndex];
        FlyTowards(targetWp.position, patrolSpeed);

        // Waypoint�e ula�t�ysa s�radaki
        if (Vector3.Distance(transform.position, targetWp.position) <= waypointReach)
        {
            wpIndex = (wpIndex + 1) % waypoints.Length;
        }
    }

    // ------------ Ortak U�u� Fonksiyonu ------------
    void FlyTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        // Yumu�ak d�n��
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);

        // �leri hareket
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
