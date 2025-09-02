using UnityEngine;

public class DusmanUcakKanat : MonoBehaviour
{
    public enum WingType { Sag, Sol }
    public WingType wingType;

    // Kanat durumu (1 = saðlam, 0 = koptu)
    public int sagKanat = 1;
    public int solKanat = 1;

    private bool kopuk = false;

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("PlayerBullet")) // Mermi tag'ýný kontrol et
        {
            
            Debug.Log("Kanat vuruldu");

            gameObject.transform.SetParent(null);
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            

            
            if(wingType == WingType.Sag)
            {
                sagKanat = 0; 
            }else if (wingType == WingType.Sol) {solKanat = 0; }
            

            
        }
    }
}
