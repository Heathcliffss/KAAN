using System.Collections;
using Tarodev;
using Unity.Mathematics;
using UnityEngine;

public class AirDefenceBomb : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 5f;
    [SerializeField] private float BombLifeTime = 5;

    private Transform target;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        StartCoroutine(BombLife(BombLifeTime));
    }

    private IEnumerator BombLife(float BombLifeTime)
    {
        yield return new WaitForSeconds(BombLifeTime);
        Destroy(gameObject);
    }

    void Update()
    {
        if (target == null) return;

        // Hedefe doğru dön
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        // İleri hareket et
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Plane scriptinde tanımlı olan PlaneBombed metodunu çalıştır
            PlaneBombed();

            // Kendini yok et
            Destroy(gameObject);
        }
    }

    public void PlaneBombed()
    {

    }
}
