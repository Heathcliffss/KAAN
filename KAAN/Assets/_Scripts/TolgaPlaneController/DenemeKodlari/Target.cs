using UnityEngine;

namespace Tarodev
{
    public class Target : MonoBehaviour, IExplode
    {
        [Header("Explosion Settings")]
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private AudioClip explosionSound;   // ✅ Patlama sesi

        public void Explode()
        {
            Debug.Log($"{gameObject.name} vuruldu ve yok oldu!");

            // ✅ Patlama efekti
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // ✅ Patlama sesi
            if (explosionSound != null)
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);

            // ✅ Ana objeyi tamamen sahneden kaldır
            Destroy(transform.root.gameObject);
            // Eğer sadece bu objeyi silmek istiyorsan -> Destroy(gameObject);
        }
    }
}
