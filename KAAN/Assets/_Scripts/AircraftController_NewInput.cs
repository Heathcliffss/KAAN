using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftController_NewInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 50f;
    public float acceleration = 30f;
    public float deceleration = 20f;
    public float maxSpeed = 150f;
    public float minSpeed = 30f;

    [Header("Rotation Settings")]
    public float pitchSpeed = 60f;   // Move1.Y
    public float yawSpeed = 40f;   // Yaw axis
    public float rollSpeed = 80f;   // Move1.X

    /* ---------------- Dahili ---------------- */
    float currentSpeed;
    Vector2 moveInput;      // Move1 : X=Roll, Y=Pitch
    float yawInput;       // Yaw axis  (Q/E)
    float throttleInput;  // Throttle (Shift/Ctrl veya gamepad trigger)

    /* -------- PlayerInput Callback’leri ------ */
    public void OnMove1(InputAction.CallbackContext c) => moveInput = c.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext c) => yawInput = c.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext c) => throttleInput = c.ReadValue<float>();

    /* --------------- Yaşam Döngüsü --------------- */
    void Start() => currentSpeed = minSpeed;

    void Update()
    {
        HandleThrottle();
        HandleRotation();
        MoveForward();
    }

    /* ------------- Hız (Shift / Ctrl veya Axis) ------------- */
    void HandleThrottle()
    {
        /* 1)  Input Actions’tan gelen Throttle değeri  */
        if (Mathf.Abs(throttleInput) > 0.01f)
            currentSpeed += throttleInput * acceleration * Time.deltaTime;

        /* 2)  Klavye yedeği (Shift/Ctrl)  */
        else
        {
            var kb = Keyboard.current;
            if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed)
                currentSpeed += acceleration * Time.deltaTime;
            else if (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed)
                currentSpeed -= deceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    /* ---------------- Dönüş ------------------- */
    void HandleRotation()
    {
        float pitch = -moveInput.y * pitchSpeed * Time.deltaTime;   // W/S
        float roll = -moveInput.x * rollSpeed * Time.deltaTime;   // A/D
        float yaw = yawInput * yawSpeed * Time.deltaTime;    // Q/E

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    /* --------------- İleri İtme --------------- */
    void MoveForward()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}
