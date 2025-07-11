using UnityEngine;
using UnityEngine.InputSystem;   // ↩︎ yeni sistem API’si

public class AircraftController_NewInput : MonoBehaviour
{
    [Header("Tunables")]
    public float maxSpeed = 200f;
    public float acceleration = 100f;
    public float pitchRate = 60f;
    public float rollRate = 80f;
    public float yawRate = 50f;

    private float currentSpeed;

    /* — Input verilerini depolayan değişkenler — */
    private Vector2 moveInput;     // Move1  →  X=Roll, Y=Pitch
    private Vector2 lookInput;     // Look1  →  Mouse Delta (kamera amaçlı)
    private float yawInput;      // Yaw    →  Q / E
    private float throttleInput; // Throttle → Shift / Ctrl

    /* ---------------------- Unity‑Event callback’leri --------------------- */
    /*  PlayerInput bileşeninin Events sekmesinden bağlanacak */
    public void OnMove1(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    public void OnLook1(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext ctx) => yawInput = ctx.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext ctx) => throttleInput = ctx.ReadValue<float>();
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) Debug.Log("🔫 Ateş!");
    }

    /* --------------------------- Uçuş fiziği ------------------------------ */
    void Update()
    {
        HandleThrottle();
        HandleRotation();
        HandleTranslation();
    }

    void HandleThrottle()
    {
        currentSpeed += throttleInput * acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }

    void HandleRotation()
    {
        float pitch = -moveInput.y * pitchRate * Time.deltaTime;  // W/S
        float roll = -moveInput.x * rollRate * Time.deltaTime;  // A/D
        float yaw = yawInput * yawRate * Time.deltaTime;   // Q/E

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    void HandleTranslation()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }
}
