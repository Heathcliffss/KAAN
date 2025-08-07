using UnityEngine;

public class PlaneHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public GameObject smokeEffect;      // Hafif duman efekti
    public GameObject explosionEffect;  // Patlama efekti
    public AudioClip explosionSound;    // Patlama sesi
    public AudioClip alarmSound;        // Hasar sesi

    private AudioSource audioSource;
    private bool isDestroyed = false;
    private GameObject currentSmoke;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        // Hasar sesi çal (alarm)
        if (alarmSound != null)
        {
            audioSource.PlayOneShot(alarmSound);
        }

        // Ýlk hasarda duman efekti baþlasýn
        if (currentHealth == 2 && smokeEffect != null && currentSmoke == null)
        {
            currentSmoke = Instantiate(smokeEffect, transform.position, transform.rotation, transform);
        }

        // Patlama ve düþüþ
        if (currentHealth <= 0)
        {
            DestroyPlane();
        }
    }

    void DestroyPlane()
    {
        isDestroyed = true;

        // Patlama efekti
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // Patlama sesi
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }


        // Rigidbody ekle ve düþmeye baþlasýn
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 10;

        // Duman varsa yok et
        if (currentSmoke != null)
        {
            Destroy(currentSmoke);
        }

        // Uçak 5 saniye sonra yok olsun
        Destroy(gameObject, 5f);
    }
}
