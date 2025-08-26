using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 1f; // Þimdilik sadece sayým için
    public string targetTag = "Player";   // Senin uçaðýn Player tag'li olmalý

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // AircraftPart var mý diye kontrol et
            AircraftPart part = other.GetComponent<AircraftPart>();
            if (part != null)
            {
                part.TakeEnemyBulletDamage();
            }

            // çarpýnca mermiyi yok et
            Destroy(gameObject);
        }
    }
}
