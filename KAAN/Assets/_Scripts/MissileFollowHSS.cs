using UnityEngine;

public class MissileFollowHSS : MonoBehaviour
{
    public float speed = 100f;
    public float rotateSpeed = 5f;
    public GameObject explosionEffect;

    private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotateTo = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Füze çarptı: " + other.name); // Hangi objeye çarptığını logla

        var part = other.GetComponent<AircraftPart>();
        if (part != null)
        {
            Debug.Log("AircraftPart bulundu, hasar veriliyor.");
            part.TakeDamage();
           
        }
        else
        {
            Debug.Log("Çarpılan objede AircraftPart yok.");
        }

        Destroy(gameObject); // Füze her durumda yok olsun
    }
}
