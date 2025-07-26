using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public bool L1 = false;

    public GameObject Cam1;
    public GameObject Cam2;

    public GameObject Cam2UI;

    public GameObject Bomb;
    public Transform BombLocation;
    public Transform BombRotation;
    public GameObject Plane;
    public float atishizi;

    public Transform Enemy;

    public AirplaneController AirplaneController;

    public Vector3 planeSpeed;

    bool RightShoulderPressed = false;










    void Start()
    {
        Cam2UI.SetActive(false);
        
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.JoystickButton4) || Input.GetKeyUp(KeyCode.Alpha1))
        {
            
            L1 = false;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            L1= true;
        }

        if (L1)
        {
            Cam2.SetActive(true);
            Cam1.SetActive(false);
            Cam2UI.SetActive(true);

        }
        else
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
            Cam2UI.SetActive(false);

        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                RightShoulderPressed = true;
            }
            else { RightShoulderPressed = false; };
        }

            Rigidbody rb2 = Plane.GetComponent<Rigidbody>();
        float speed2 = rb2.linearVelocity.magnitude;

        float egim = AirplaneController.Pitch;
        float flap3 = AirplaneController.Flap;

        planeSpeed = Plane.GetComponent<Rigidbody>().linearVelocity;
        
        if (Input.GetKeyDown(KeyCode.V) & egim < 0.2|| RightShoulderPressed & egim < 0.2)
        {
            GameObject instantiatedBomb = Instantiate(Bomb, BombLocation.position, BombRotation.rotation);

            Rigidbody rb = instantiatedBomb.GetComponent<Rigidbody>();
            if (rb != null)
            {

                rb.linearVelocity = Plane.GetComponent<Rigidbody>().linearVelocity;
                
                

            }
        }


        


    }
}
