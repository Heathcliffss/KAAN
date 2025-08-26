using UnityEngine;

public class AircraftPart : MonoBehaviour
{
    public enum PartType { Wing, Body }
    public PartType partType;

    [Tooltip("Bu parça yok edildiğinde birlikte devre dışı bırakılacak objeler (Opsiyonel)")]
    public GameObject[] additionalPartsToDisable;

    [Header("Yok olma efektleri")]
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

        // Alev efektlerini çalıştır ve parent’tan ayır
        PlayAndDetachEffect(alev1);
        PlayAndDetachEffect(alev2);

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

        Debug.Log($">> {partType} yok edildi, alev efektleri çalıştı.");
    }

    private void PlayAndDetachEffect(ParticleSystem effect)
    {
        if (effect == null) return;

        // Eğer inspector’da disable haldeyse açıyoruz
        effect.gameObject.SetActive(true);

        // Parent’tan ayır ki kapalı objeyle kapanmasın
        effect.transform.SetParent(null);

        // Çalıştır
        effect.Play();

        // Bitince otomatik sil
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }

    // AircraftPart.cs içine ekle
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet")) // mermi tag'ı bu olmalı
        {
            TakeEnemyBulletDamage();
            Destroy(other.gameObject); // mermiyi yok et
        }
    }

}
