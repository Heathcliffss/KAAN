using UnityEngine;

public class AircraftPart : MonoBehaviour
{
    public enum PartType { Wing, Body }
    public PartType partType;

    [Tooltip("Bu parça yok edildiğinde birlikte devre dışı bırakılacak objeler (Opsiyonel)")]
    public GameObject[] additionalPartsToDisable;

    [Header("Yok olma efektleri (sahnede hazır olan particle objelerini at)")]
    public ParticleSystem alev1;
    public ParticleSystem alev2;

    private int missileHitCount = 0;     // Füze sayacı
    private int enemyBulletHitCount = 0; // Mermi sayacı
    private bool detached = false;

    /// Füze hasarı
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

    /// Düşman mermisi hasarı
    public void TakeEnemyBulletDamage()
    {
        if (detached) return;

        if (partType == PartType.Wing)
        {
            DetachPart();
        }
        else if (partType == PartType.Body)
        {
            enemyBulletHitCount++;
            if (enemyBulletHitCount >= 5) // 5 mermi → gövde yok olur
            {
                DetachPart();
            }
        }
    }

    private void DetachPart()
    {
        if (detached) return;
        detached = true;

        // Sahnede hazır olan particle objelerini sadece aktif et ve çalıştır
        ActivateEffect(alev1);
        ActivateEffect(alev2);

        // Ana objeyi kapat
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

        Debug.Log($">> {partType} yok edildi, sahnedeki efektler çalıştı.");
    }

    private void ActivateEffect(ParticleSystem effect)
    {
        if (effect == null) return;

        // Objeyi aktif et (sahnede disable ise açılır)
        effect.gameObject.SetActive(true);

        // Particle sistemini oynat
        effect.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet")) // mermi tag'ı bu olmalı
        {
            TakeEnemyBulletDamage();
            Destroy(other.gameObject); // mermiyi yok et
        }
    }
}
