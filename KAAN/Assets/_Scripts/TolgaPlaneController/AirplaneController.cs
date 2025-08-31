using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AirplaneController : MonoBehaviour
{
    [SerializeField] List<AeroSurface> controlSurfaces = null;
    [SerializeField] List<WheelCollider> wheels = null;
    [SerializeField] float rollControlSensitivity = 0.2f;
    [SerializeField] float pitchControlSensitivity = 0.2f;
    [SerializeField] float yawControlSensitivity = 0.2f;
    [SerializeField] float YawAssist = 300f;

    [Range(-1, 1)] public float Pitch;
    [Range(-1, 1)] public float Yaw;
    [Range(-1, 1)] public float Roll;
    [Range(0, 1)] public float Flap;
    [SerializeField] Text displayText = null;

    public float thrustPercent;
    float brakesTorque;

    AircraftPhysics aircraftPhysics;
    Rigidbody rb;

    bool IsSpace = true;
    [SerializeField] private Transform IsGround;
    [SerializeField] public bool onGround;

    private float lastVibrateTime;
    private const float VIBRATE_COOLDOWN = 0.1f;

    public float cooldownTime = 2f;
    private float lastBoostTime = -999f;

   
    [SerializeField] private GameObject explosionPrefab;

    private void Start()
    {
        aircraftPhysics = GetComponent<AircraftPhysics>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float yawKey2 = 0f;
        if (Gamepad.current != null) yawKey2 = Gamepad.current.rightStick.x.ReadValue();

        float pankey2 = 0f;
        if (Gamepad.current != null) pankey2 = Gamepad.current.leftStick.y.ReadValue();
        else pankey2 = Input.GetAxis("Vertical");

        //float flap2 = 0f;
        // if (Gamepad.current != null) flap2 = Gamepad.current.rightStick.y.ReadValue();

        //float newflap2 = Mathf.Clamp(flap2 * -1f, 0f, 1f);

        if (transform.position.y < 300)
        {
            if (rb.linearVelocity.magnitude > 55) rb.linearVelocity = rb.linearVelocity.normalized * 55f;
        }
       

        Roll = Input.GetAxis("Horizontal");
        Yaw = -yawKey2;
        Pitch = Input.GetAxis("Vertical");

        float R2 = 0f, L2 = 0f;
        if (Gamepad.current != null)
        {
            R2 = Gamepad.current.rightTrigger.ReadValue();
            L2 = Gamepad.current.leftTrigger.ReadValue();
        }

        float mutlakYaw = Mathf.Abs(Yaw);
        onGround = Physics.Raycast(IsGround.position, Vector3.down, 1f);

        if (mutlakYaw > 0.1f || onGround) rb.AddForce(Vector3.up * YawAssist, ForceMode.Force);

        thrustPercent += R2 * Time.deltaTime;
        thrustPercent -= L2 * Time.deltaTime;
        thrustPercent = Mathf.Clamp(thrustPercent, 0f, 1f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            thrustPercent = IsSpace ? 1 : 0;
            IsSpace = !IsSpace;
        }

        if (Gamepad.current != null)
        {
            bool l3 = Gamepad.current.leftStickButton.isPressed;


            if (l3 && Time.time >= lastBoostTime + cooldownTime && !onGround)
            {
                Boost();
            }

            void Boost()
            {
                //rb.AddForce(transform.forward * 50000f, ForceMode.Impulse);
                rb.linearVelocity = rb.linearVelocity * 1.3f;
                lastBoostTime = Time.time;
                Invoke("BoostEndTime", 5f);
                Debug.Log("boosttt");
            }
        }

        float SpeedDisplay = ((int)rb.linearVelocity.magnitude) * 2f;

        HandleGamepadVibration();
        //Flap = newflap2;

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("BButton"))
            brakesTorque = brakesTorque > 0 ? 0 : 100f;

        if (displayText != null)
        {
            displayText.text = "V: " + ((int)SpeedDisplay).ToString("D3") + " m/s\n";
            displayText.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
            displayText.text += "T: " + (int)(thrustPercent * 100) + "%\n";
            displayText.text += brakesTorque > 0 ? "B: ON" : "B: OFF";
        }
    }

    private void HandleGamepadVibration()
    {
        if (Time.time - lastVibrateTime < VIBRATE_COOLDOWN) return;
        try
        {
            Gamepad currentGamepad = Gamepad.current;
            if (currentGamepad != null && currentGamepad.added)
            {
                float motorSpeed = thrustPercent < 0.5f ? thrustPercent * 0.5f :
                                   Mathf.Lerp(0.25f, 0.8f, (thrustPercent - 0.5f) * 2f);

                motorSpeed = Mathf.Clamp(motorSpeed, 0f, 0.8f);

                currentGamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
                lastVibrateTime = Time.time;
            }
        }
        catch { }
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
                case ControlInputType.Pitch: surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer); break;
                case ControlInputType.Roll: surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer); break;
                case ControlInputType.Yaw: surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer); break;
                case ControlInputType.Flap: surface.SetFlapAngle(Flap * surface.InputMultiplyer); break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
    }

    private void OnDestroy()
    {
        try
        {
            Gamepad currentGamepad = Gamepad.current;
            if (currentGamepad != null && currentGamepad.added)
                currentGamepad.SetMotorSpeeds(0f, 0f);
        }
        catch { }
    }

    public float GetThrustPercent() => thrustPercent;

    public void Crash()
    {
        Debug.Log("Uçak vuruldu ve düşüyor!");
        GetComponent<AirplaneController>().enabled = false;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 300f;
        rb.useGravity = true;
    }

    // 💥 Uçak yere çarpınca tetiklenecek
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundLayer"))
        {
            Debug.Log("Uçak yere çarptı!");
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject); // uçağı yok et
            Time.timeScale = 0f; // oyunu durdur
        }
    }
}
