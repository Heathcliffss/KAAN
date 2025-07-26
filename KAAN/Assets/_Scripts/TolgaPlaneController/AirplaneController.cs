using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AirplaneController : MonoBehaviour
{
    [SerializeField]
    List<AeroSurface> controlSurfaces = null;
    [SerializeField]
    List<WheelCollider> wheels = null;
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;

    [Range(-1, 1)]
    public float Pitch;
    [Range(-1, 1)]
    public float Yaw;
    [Range(-1, 1)]
    public float Roll;
    [Range(0, 1)]
    public float Flap;
    [SerializeField]
    Text displayText = null;

    public float thrustPercent;
    float brakesTorque;

    AircraftPhysics aircraftPhysics;
    Rigidbody rb;

    bool IsSpace = true;

    

    // Remove the gamepad field declaration and handle it safely in Update
    private float lastVibrateTime;
    private const float VIBRATE_COOLDOWN = 0.1f; // Prevent excessive vibration calls

    private void Start()
    {
        aircraftPhysics = GetComponent<AircraftPhysics>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float yawKey2 = 0f;

        if (Gamepad.current != null)
        {
            yawKey2 = Gamepad.current.rightStick.x.ReadValue();
        }
        else {  };

        float pankey2 = 0f;

        if (Gamepad.current != null)
        {
            pankey2 = Gamepad.current.leftStick.y.ReadValue();
        }
        else { Debug.Log("Gamepad Bagli Degil"); pankey2 = Input.GetAxis("Vertical"); }

        float flap2 = 0f;

        if (Gamepad.current != null)
        {
            flap2 = Gamepad.current.rightStick.y.ReadValue();
        }
        

        float newflap2 = flap2 * -1f;
        newflap2 = Mathf.Clamp(newflap2, 0f, 1f);

        


        //Pitch = Input.GetAxis("Vertical");
        Pitch = pankey2;
        Roll = Input.GetAxis("Horizontal");
        Yaw = -yawKey2;

        float R2 = Input.GetAxis("RightTrigger");
        float L2 = Input.GetAxis("LeftTrigger");

       
        

        thrustPercent += R2 * Time.deltaTime;
        thrustPercent -= L2 * Time.deltaTime;
        thrustPercent = Mathf.Clamp(thrustPercent, 0f, 1f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            if (IsSpace)
            {
                thrustPercent = 1;
            }
            else
            {
                thrustPercent = 0;
            }
            IsSpace = !IsSpace;
        }

        // Fixed gamepad vibration handling
        HandleGamepadVibration();

        /* if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("YButton"))
         {
             Flap = Flap > 0 ? 0 : 0.3f;
         }
     */

        Flap = newflap2;

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("BButton"))
        {
            brakesTorque = brakesTorque > 0 ? 0 : 100f;
        }

        // Update display
        if (displayText != null)
        {
            displayText.text = "V: " + ((int)rb.linearVelocity.magnitude).ToString("D3") + " m/s\n";
            displayText.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
            displayText.text += "T: " + (int)(thrustPercent * 100) + "%\n";
            displayText.text += brakesTorque > 0 ? "B: ON" : "B: OFF";
        }
    }

    private void HandleGamepadVibration()
    {
        // Only update vibration every VIBRATE_COOLDOWN seconds to prevent excessive calls
        if (Time.time - lastVibrateTime < VIBRATE_COOLDOWN) return;

        try
        {
            // Safely check for gamepad
            Gamepad currentGamepad = Gamepad.current;
            if (currentGamepad != null && currentGamepad.added)
            {
                float motorSpeed = 0f;

                if (thrustPercent < 0.5f)
                {
                    motorSpeed = thrustPercent * 0.5f; // Reduced intensity
                }
                else
                {
                    motorSpeed = Mathf.Lerp(0.25f, 0.8f, (thrustPercent - 0.5f) * 2f); // Clamped max intensity
                }

                // Clamp to safe values
                motorSpeed = Mathf.Clamp(motorSpeed, 0f, 0.8f);

                currentGamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
                lastVibrateTime = Time.time;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Gamepad vibration error: {e.Message}");
            // Don't let gamepad errors crash the game
        }
    }

    private void FixedUpdate()
    {
        SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
        aircraftPhysics.SetThrustPercent(thrustPercent);
        foreach (var wheel in wheels)
        {
            wheel.brakeTorque = brakesTorque;
            wheel.motorTorque = 0.01f;
        }
    }

    public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in controlSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(Flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
    }

    // Clean up gamepad vibration when the object is destroyed
    private void OnDestroy()
    {
        try
        {
            Gamepad currentGamepad = Gamepad.current;
            if (currentGamepad != null && currentGamepad.added)
            {
                currentGamepad.SetMotorSpeeds(0f, 0f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error stopping gamepad vibration on destroy: {e.Message}");
        }
    }
    public float GetThrustPercent()
    {
        return thrustPercent;
    }

}