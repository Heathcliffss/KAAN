using UnityEngine;

public class AircraftPart : MonoBehaviour
{
    public enum PartType { Wing, Body }
    public PartType partType;

    [Tooltip("Bu parça yok edildiðinde birlikte devre dýþý býrakýlacak obje (Opsiyonel)")]
    public GameObject additionalPartToDisable; // Kapatýlacak ek obje

    private int hitCount = 0;
    private bool detached = false;

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

        // Ana objeyi devre dýþý býrak
        gameObject.SetActive(false);

        // Ek objeyi de devre dýþý býrak (eðer atanmýþsa)
        if (additionalPartToDisable != null)
        {
            additionalPartToDisable.SetActive(false);
        }

        Debug.Log(">> Kanat ve baðlý parça devre dýþý býrakýldý.");
    }
}