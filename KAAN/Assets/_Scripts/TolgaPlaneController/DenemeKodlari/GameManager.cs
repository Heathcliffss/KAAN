using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public bool L1 = false;

    public GameObject Cam1;
    public GameObject Cam2;

    
    void Start()
    {
        
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

        }
        else
        {
            Cam1.SetActive(true);
            Cam2.SetActive(false);
            
        }
    }
}
