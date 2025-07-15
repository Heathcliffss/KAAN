using UnityEngine;

public class EnemyChaseAI : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player;            // Oyuncu uçaðý
    public Transform[] waypoints;       // Devriye noktalarý

    [Header("Mesafeler")]
    public float viewDistance = 120f; // Oyuncuyu görme
    public float loseDistance = 150f; // Bu mesafenin üstünde kovalama biter
    public float waypointReach = 10f;  // Waypoint’e varmýþ sayýlacaðý yarýçap

    [Header("Hýzlar")]
    public float patrolSpeed = 40f;
    public float chaseSpeed = 60f;
    public float rotationSpeed = 2f;

    private bool isChasing = false;
    private int wpIndex = 0;         // Aktif waypoint

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Görüþ alanýna girerse kovalamaya baþla
        if (!isChasing && distToPlayer < viewDistance)
            isChasing = true;

        // Uzaklaþýrsa kovalamayý býrak
        if (isChasing && distToPlayer > loseDistance)
            isChasing = false;

        // Davranýþ seçimi
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

        // Waypoint’e ulaþtýysa sýradaki
        if (Vector3.Distance(transform.position, targetWp.position) <= waypointReach)
        {
            wpIndex = (wpIndex + 1) % waypoints.Length;
        }
    }

    // ------------ Ortak Uçuþ Fonksiyonu ------------
    void FlyTowards(Vector3 targetPos, float speed)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        // Yumuþak dönüþ
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);

        // Ýleri hareket
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
