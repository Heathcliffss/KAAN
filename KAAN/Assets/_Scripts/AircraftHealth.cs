using UnityEngine;

public class AircraftHealth : MonoBehaviour
{
    public float maxHealth = 300f;
    private float currentHealth;

    public GameObject firePrefab;
    public GameObject smokePrefab;

    private bool smokeStarted = false;
    private bool fireStarted = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeBodyDamage(float amount, Transform hitPoint)
    {
        currentHealth -= amount;

        if (!smokeStarted && currentHealth <= maxHealth * 0.6f)
        {
            Instantiate(smokePrefab, hitPoint.position, Quaternion.identity, hitPoint);
            smokeStarted = true;
        }

        if (!fireStarted && currentHealth <= maxHealth * 0.3f)
        {
            Instantiate(firePrefab, hitPoint.position, Quaternion.identity, hitPoint);
            fireStarted = true;
        }

        if (currentHealth <= 0f)
        {
            Crash();
        }
    }

    void Crash()
    {
        // Uçaðýn kontrolünü kaybetmesi gibi efektleri buraya yaz
        Debug.Log("Uçak yok oldu!");
    }
}
