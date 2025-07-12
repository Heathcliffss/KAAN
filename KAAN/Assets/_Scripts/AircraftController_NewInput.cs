using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftController_NewInput : MonoBehaviour
{
    /* ───────────── Inspector Ayarları ───────────── */
    [Header("Hız & İvmelenme")]
    public float maxSpeed = 180f;    // en yüksek hız
    public float accelRate = 60f;     // Shift basılıyken saniyede +60
    public float decelRate = 40f;     // Ctrl  basılıyken saniyede –40
    public float idleDecay = 10f;     // motor açıkken gaz yoksa düşüş

    [Header("Kalkış")]
    public float takeoffSpeed = 100f;    // kalkış eşiği
    public float groundY = 0.35f;   // pist yüksekliği (Pivot‑Y)

    [Header("Dönüş Hızları")]
    public float pitchRate = 40f;
    public float rollRate = 60f;
    public float yawRate = 35f;

    /* ───────────── Dahili ───────────── */
    bool engineOn = false;
    float curSpeed = 0f;
    Vector2 moveInp = Vector2.zero;   // Move1
    float yawInp = 0f;
    float throttleInp = 0f;

    /* --------- INPUT CALLBACK’LER --------- */
    public void OnMove1(InputAction.CallbackContext c) => moveInp = c.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext c) => yawInp = c.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext c) => throttleInp = c.ReadValue<float>();

    /* ───────────── Lifecycle ───────────── */
    void Update()
    {
        ToggleEngine();         // Space ON/OFF
        HandleThrottle();       // Shift / Ctrl
        HandleRotation();       // WASD + Q/E
        MoveForward();          // ileri itme (yalnız motor ON iken)
        GroundLock();           // yerdeyken Y sabit
    }

    /* ------------ Motor ON/OFF ------------ */
    void ToggleEngine()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            engineOn = !engineOn;
            if (!engineOn) curSpeed = 0f;          // motor kapandı → hız sıfırla
            Debug.Log(engineOn ? "Motor AÇIK" : "Motor KAPALI");
        }
    }

    /* ------------ Gaz / Hız --------------- */
    void HandleThrottle()
    {
        if (!engineOn) return;                     // motor kapalı → hiç işlem yok

        // Yeni InputSystem Throttle
        if (Mathf.Abs(throttleInp) > 0.01f)
            curSpeed += throttleInp * accelRate * Time.deltaTime;

        // Klavye yedeği
        var kb = Keyboard.current;
        if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed)
            curSpeed += accelRate * Time.deltaTime;
        else if (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed)
            curSpeed -= decelRate * Time.deltaTime;
        else
            curSpeed -= idleDecay * Time.deltaTime;

        curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);
    }

    /* ------------ Dönüş ------------------- */
    void HandleRotation()
    {
        bool canPitch = engineOn && curSpeed >= takeoffSpeed;

        float pitch = canPitch ? -moveInp.y * pitchRate * Time.deltaTime : 0f;
        float roll = -moveInp.x * rollRate * Time.deltaTime;
        float yaw = yawInp * yawRate * Time.deltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    /* ------------ İleri İtme -------------- */
    void MoveForward()
    {
        if (!engineOn || curSpeed <= 0.01f) return;
        transform.position += transform.forward * curSpeed * Time.deltaTime;
    }

    /* ------ Pistte Yüksekliği Sabit Tut ---- */
    void GroundLock()
    {
        if (engineOn && curSpeed >= takeoffSpeed) return;  // havalanınca serbest
        Vector3 p = transform.position;
        p.y = groundY;
        transform.position = p;
    }
}
