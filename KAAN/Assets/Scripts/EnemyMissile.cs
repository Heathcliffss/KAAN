using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float speed = 50f;
    public float rotateSpeed = 5f;
    public float trackingDuration = 3f; // Ka� saniye hedef takip edilsin
    public float selfDestructTime = 10f; // Yine de 10 saniye sonra yok olsun

    private Transform target;
    private bool tracking = true;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        Destroy(gameObject, selfDestructTime); // Son �are g�venlik
        Destroy(gameObject, 25f); // Roketi 20 saniye sonra sahneden sil
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (tracking && target != null && Time.time - startTime < trackingDuration)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            tracking = false; // S�re doldu, d�z gitmeye ba�la
        }

        // Her durumda ileri do�ru hareket
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // hasar vs. eklenecekse buraya
            Destroy(gameObject);
        }
    }
}
