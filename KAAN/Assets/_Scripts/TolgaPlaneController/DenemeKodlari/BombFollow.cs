using System.Collections;
using UnityEngine;

public class BombFollow : MonoBehaviour
{
    public Transform BombLoc1;
    public GameObject Bomblocation1;
    public float FollowTime = 2f;
    private bool following;
    public float followspeed;
    public Transform Enemy;
    private Rigidbody rb;
    void Start()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag("Enemy");
        if (targetObj != null)
        {
            Enemy = targetObj.transform;
        }
        StartCoroutine(Follow(FollowTime));

    }

   
    void Update()
    {

        if (following && Enemy != null)
        {
            rb = GetComponent<Rigidbody>();

            // Hedefin yönünü hesapla
            Vector3 directionToEnemy = (Enemy.position - transform.position).normalized;

            // Füzenin ileri yönü ile hedef arasýndaki açýyý hesapla
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

            // Açýnýn belirli bir eþik deðerden küçük olup olmadýðýný kontrol et
            if (angleToEnemy < 180f) // örnek: 45 derece
            {
                // Eðer hedef belirli bir görüþ alanýndaysa, takip et
                rb.AddForce(directionToEnemy * 5000f);
            }
            else
            {
                // Açý çok büyükse, hedef çok dik veya geride
                Debug.Log("Hedef görüþ açýsýnýn dýþýnda");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    IEnumerator Follow(float FollowTime)
    {
        following = false;
        yield return new WaitForSeconds(FollowTime);
        following = true;
    }

}
