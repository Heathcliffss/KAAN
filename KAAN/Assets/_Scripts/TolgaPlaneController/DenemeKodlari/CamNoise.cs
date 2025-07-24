using Unity.Cinemachine;
using UnityEngine;

public class CamNoise : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public Transform IsGround;
    public Rigidbody Plane;

    private CinemachineBasicMultiChannelPerlin perlin;
    private float speed;
    private float frequency;

    public bool onGround;

    private void Start()
    {
        perlin = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        frequency = 1.0f;
    }

    private void Update()
    {
        speed = Plane.linearVelocity.magnitude;

         onGround = Physics.Raycast(IsGround.position, Vector3.down, 1f);


        if(!onGround || speed < 0.1f)
        {
            frequency = Mathf.Lerp(1f, 40f, speed / 300f);
        }
        else
        {
            frequency = Mathf.Lerp(1f, 60f, speed / 80f);

        }
        perlin.FrequencyGain = frequency;
    }


}
