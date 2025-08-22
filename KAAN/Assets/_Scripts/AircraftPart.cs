using UnityEngine;

public class AircraftPart : MonoBehaviour
{
    public enum PartType { Wing, Body }
    public PartType partType;

    [Tooltip("Bu parça yok edildiðinde birlikte devre dýþý býrakýlacak objeler (Opsiyonel)")]
    public GameObject[] additionalPartsToDisable; // Birden fazla ek obje

    [Header("Yok olma efekti")]
    public ParticleSystem destroyEffect; // Partikül efekti

    private int hitCount = 0;
    private bool detached = false;

    private void Start()
    {
        // Baþta efekt kapalý olsun
        if (destroyEffect != null)
        {
            destroyEffect.gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        if (partType == PartType.Wing && !detached)
        {
            DetachPart();
        }
        else if (partType == PartType.Body)
        {
            hitCount++;
            if (hitCount >= 2)
            {
                GetComponentInParent<AirplaneController>().Crash();
            }
        }
    }

    private void DetachPart()
    {
        if (detached) return;
        detached = true;

        // Efekti çalýþtýr
        if (destroyEffect != null)
        {
            destroyEffect.gameObject.SetActive(true);
            destroyEffect.Play();
        }

        // Ana objeyi devre dýþý býrak
        gameObject.SetActive(false);

        // Tüm ek objeleri devre dýþý býrak
        if (additionalPartsToDisable != null && additionalPartsToDisable.Length > 0)
        {
            foreach (var part in additionalPartsToDisable)
            {
                if (part != null)
                    part.SetActive(false);
            }
        }

        Debug.Log(">> Kanat ve baðlý parçalar devre dýþý býrakýldý, efekt oynatýldý.");
    }
}
