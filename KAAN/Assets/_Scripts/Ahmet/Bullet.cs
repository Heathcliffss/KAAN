using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlaneHealth health = collision.gameObject.GetComponent<PlaneHealth>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }

        // Mermiyi yok et
        Destroy(gameObject);
    }
}
