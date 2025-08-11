using UnityEngine;

public class WingRotationContoller2 : MonoBehaviour
{
    public Transform leftWing;   // Sol kanat ucu objesi
    public Transform rightWing;  // Sağ kanat ucu objesi
    public AirplaneController airplaneController;  // Roll değerini buradan alıyoruz
    
    [SerializeField]
    float neutralX = -90f;  // Nötr X dönüşü
    [SerializeField]
    float range = 20f;      // Maksimum döneceği açı

    void Update()
    {
        float roll =airplaneController.Roll;  // -1 ile 1 arasında sınırla

        // Sağ kanat: -90'tan -150'ye (yani -90 - range * roll)
        float rightTargetX = neutralX - range * roll;

        // Sol kanat: -90'tan -30'a (yani -90 + range * roll)
        float leftTargetX = neutralX + range * roll;

       
        rightWing.localRotation = Quaternion.Euler(rightTargetX, 0f, 0f);
        leftWing.localRotation = Quaternion.Euler(leftTargetX, 0f, 0f);
    }
}

