using UnityEngine;

public class AircraftPart : MonoBehaviour
{
    public enum PartType { Wing, Body }
    public PartType partType;

    [Tooltip("Bu parça yok edildiğinde birlikte devre dışı bırakılacak objeler (Opsiyonel)")]
    public GameObject[] additionalPartsToDisable; // Birden fazla ek obje

    [Header("Yok olma efekti")]
    public ParticleSystem destroyEffect; // Partikül efekti

    private int missileHitCount = 0;       // Füze sayacı (Body için)
    private int enemyBulletHitCount = 0;   // Mermi sayacı (Body için)
    private bool detached = false;

    private void Start()
    {
        if (destroyEffect != null)
        {
            destroyEffect.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Füze hasarı
    /// </summary>
    public void TakeDamage()
    {
        if (partType == PartType.Wing && !detached)
        {
            DetachPart();
        }
        else if (partType == PartType.Body)
        {
            missileHitCount++;
            if (missileHitCount >= 2) // Gövde 2 füze yerse düş
            {
                GetComponentInParent<AirplaneController>().Crash();
            }
        }
    }

    /// <summary>
    /// Düşman mermisi hasarı
    /// </summary>
    public void TakeEnemyBulletDamage()
    {
        if (partType == PartType.Wing && !detached)
        {
            // Kanat tek mermiyle bile kopabilir
            DetachPart();
        }
        else if (partType == PartType.Body)
        {
            enemyBulletHitCount++;
            if (enemyBulletHitCount >= 5) // Gövde 5 mermi yerse düş
            {
                DetachPart(); // Füze mantığıyla aynı işlemi uygula
            }
        }
    }

    private void DetachPart()
    {
        if (detached) return;
        detached = true;

        // Efekti çalıştır
        if (destroyEffect != null)
        {
            destroyEffect.gameObject.SetActive(true);
            destroyEffect.Play();
        }

        // Ana objeyi devre dışı bırak
        gameObject.SetActive(false);

        // Ek parçaları kapat
        if (additionalPartsToDisable != null && additionalPartsToDisable.Length > 0)
        {
            foreach (var part in additionalPartsToDisable)
            {
                if (part != null)
                    part.SetActive(false);
            }
        }

        Debug.Log(">> Parça yok edildi, efekt oynatıldı.");
    }
}
