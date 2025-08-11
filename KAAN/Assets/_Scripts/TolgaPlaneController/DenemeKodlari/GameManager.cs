using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool L1 = false;
    public bool l3 = false;

    public GameObject Cam1;
    public GameObject Cam2;
    public GameObject Cam3;

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
        
        QualitySettings.vSyncCount = 1;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton4) || Input.GetKeyUp(KeyCode.Alpha1))
        {
            
            L1 = false;
            l3 = false;
        }
       

        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            L1= true;
            l3 = false;
        }
        

        if (Mouse.current != null && Mouse.current.middleButton.isPressed || Gamepad.current != null && Gamepad.current.rightStickButton.isPressed)
        {
            L1 = false;
            l3 = true;
        }
        else if(!L1)
        {
            l3 = false;
            L1 = false;
        }

        if (L1)
        {
            Cam2.SetActive(true);
            Cam1.SetActive(false);
            Cam3.SetActive(false);
            Cam2UI.SetActive(true);

        }

        if(!L1)
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
            Cam3.SetActive(false);
            Cam2UI.SetActive(false);

        }
        if(l3)
        {
            Cam3.SetActive(true);
            Cam2.SetActive(false);
            Cam1.SetActive(false);
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

        bool IsGround = AirplaneController.onGround;

        planeSpeed = Plane.GetComponent<Rigidbody>().linearVelocity;
        
        if (Input.GetKeyDown(KeyCode.V) & egim < 0.2 & !IsGround || RightShoulderPressed & egim < 0.2 & !IsGround)
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
