using UnityEngine;

public class WingRotationControl : MonoBehaviour
{
    public AirplaneController airplaneController;
    public float neutralX = -89.98f;

    // pitch = 1 veya -1 olduğunda ne kadar bükülecek
    public float pitchRange = 60f;

    void Update()
    {
        float pitch = -airplaneController.Pitch;

        float xRotation = neutralX + pitch * pitchRange;

        // YALNIZCA X ekseninde döndür, Y ve Z sabit tut
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

}

