using UnityEngine;

public class DusmanUcakKanat : MonoBehaviour
{
    public enum WingType { Sag, Sol }
    public WingType wingType;

    // Kanat durumu (1 = sa?lam, 0 = koptu)
    public int sagKanat = 1;
    public int solKanat = 1;

    public GameObject ExplosionRight;
    public GameObject ExplosionLeft;

    public GameManager Gm;

    private bool kopuk = false;
    public GameObject Plane;
    public GameObject PlaneVariant;

    private MinimapTargets mintargets;

    private void Start()
    {
        MinimapTargets mintargets = PlaneVariant.GetComponent<MinimapTargets>();
        if (ExplosionLeft == null)
            ExplosionLeft = transform.Find("ExplosionLeft").gameObject;

        if (ExplosionRight == null)
            ExplosionRight = transform.Find("ExplosionRight").gameObject;

        if (ExplosionLeft != null) ExplosionLeft.SetActive(false);
        if (ExplosionRight != null) ExplosionRight.SetActive(false);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("PlayerBullet")) 
        {
            
            Debug.Log("Kanat vuruldu");

            gameObject.transform.SetParent(null);
            
            if (wingType == WingType.Sag && sagKanat == 1)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                if (mintargets != null)
                {
                    mintargets.enabled = false;
                }
                Destroy(Plane, 5f);
            }
            else if (wingType == WingType.Sol && solKanat == 1)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                if (mintargets != null)
                {
                    mintargets.enabled = false;
                }
                Destroy(Plane, 10f);

            }



            if (wingType == WingType.Sag)
            {
                sagKanat = 0;
                Gm.AddScore(5);
                if (ExplosionRight != null)
                    ExplosionRight.SetActive(true);
            }
            else if (wingType == WingType.Sol)
            {
                solKanat = 0;
                Gm.AddScore(5);
                if (ExplosionLeft != null)
                    ExplosionLeft.SetActive(true);
            }



        }

    }

    
}
