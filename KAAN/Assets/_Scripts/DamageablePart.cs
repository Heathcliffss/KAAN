using UnityEngine;

public enum PartType { LeftWing, RightWing, Tail, Body }

public class DamageablePart : MonoBehaviour
{
    public PartType partType;
    public GameObject detachedPartPrefab; // Kopacak prefab (ayn� model, Rigidbody�li)
    public Transform fireEffectPoint;     // Yanma efekti nereye eklenecek
    public float health = 100f;
    public bool isDetached = false;

    private AircraftHealth aircraftHealth;

    void Start()
    {
        aircraftHealth = GetComponentInParent<AircraftHealth>();
    }

    public void ApplyDamage(float amount, string damageSource)
    {
        if (isDetached) return;

        health -= amount;

        if (health <= 0f && (partType == PartType.LeftWing || partType == PartType.RightWing || partType == PartType.Tail))
        {
            DetachPart();
        }
        else if (partType == PartType.Body)
        {
            aircraftHealth.TakeBodyDamage(amount, fireEffectPoint);
        }
    }

    void DetachPart()
    {
        isDetached = true;

        // Yeni par�a olu�tur
        GameObject detached = Instantiate(detachedPartPrefab, transform.position, transform.rotation);
        detached.GetComponent<Rigidbody>().linearVelocity = GetComponentInParent<Rigidbody>().linearVelocity;

        // Yang�n Efekti
        Instantiate(aircraftHealth.firePrefab, fireEffectPoint.position, Quaternion.identity, detached.transform);

        // Kendi render'�n� kapat
        gameObject.SetActive(false);

        // 2 saniye sonra yok olsun
        Destroy(detached, 2f);
    }
}
