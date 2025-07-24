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

            // Hedefin y�n�n� hesapla
            Vector3 directionToEnemy = (Enemy.position - transform.position).normalized;

            // F�zenin ileri y�n� ile hedef aras�ndaki a��y� hesapla
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

            // A��n�n belirli bir e�ik de�erden k���k olup olmad���n� kontrol et
            if (angleToEnemy < 180f) // �rnek: 45 derece
            {
                // E�er hedef belirli bir g�r�� alan�ndaysa, takip et
                rb.AddForce(directionToEnemy * 5000f);
            }
            else
            {
                // A�� �ok b�y�kse, hedef �ok dik veya geride
                Debug.Log("Hedef g�r�� a��s�n�n d���nda");
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
