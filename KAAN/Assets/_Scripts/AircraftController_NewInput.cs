using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class AircraftController_Takeoff : MonoBehaviour
{
    /* -------- Ayarlar -------- */
    [Header("Hız")]
    public float maxSpeed = 220f;
    public float accelRate = 100f;   // Shift
    public float decelRate = 60f;   // Ctrl
    public float idleDecay = 10f;   // Motor açık, gaz yok
    public float takeoffSpeed = 120f;

    [Header("Dönüş")]
    public float pitchRate = 50f;
    public float rollRate = 70f;
    public float yawRate = 40f;

    [Header("Zemin")]
    public float rayOffset = 1.0f;         // gövdeden ne kadar yukarıdan ray atılsın
    public LayerMask groundLayer;                  // runway layer’ı
    public float groundedDrag = 5f;           // yerdeyken fren etkisi

    /* -------- Dahili -------- */
    float curSpeed;
    bool engineOn = false;
    bool isGrounded = true;

    Vector2 moveInp;
    float yawInp;
    float throttleInp;

    Rigidbody rb;

    /* -------- Input Callbacks (PlayerInput) -------- */
    public void OnMove1(InputAction.CallbackContext c) => moveInp = c.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext c) => yawInp = c.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext c) => throttleInp = c.ReadValue<float>();

    /* ---------------- Başlangıç ---------------- */
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1500;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);   // gövdenin altına

        // İniş öncesi dengede tutmak için:
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        curSpeed = 0;
        engineOn = false;
    }

    /* ---------------- Ana Döngü ---------------- */
    void Update()
    {
        ToggleEngine();     // Space
        CheckGround();      // Raycast
        HandleThrottle();   // Shift / Ctrl / Trigger
        HandleRotation();   // W A S D  Q E
    }

    void FixedUpdate()      // Fiziksel ileri itme
    {
        MoveForward();
    }

    /* ------------ Motor ON/OFF (Space) ---------- */
    void ToggleEngine()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            engineOn = !engineOn;
            Debug.Log(engineOn ? "Motor ÇALIŞTI" : "Motor KAPANDI");
        }
    }

    /* ------------ Zemin Algılama (raycast) ------ */
    void CheckGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayOffset;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayOffset + 0.2f, groundLayer);

        // Yerdeyken fren drag’i uygula
        rb.linearDamping = isGrounded ? groundedDrag : 0f;
    }

    /* ------------ Gaz / Hız --------------------- */
    void HandleThrottle()
    {
        if (!engineOn)
        {
            curSpeed = Mathf.MoveTowards(curSpeed, 0f, decelRate * Time.deltaTime);
            return;
        }

        if (Mathf.Abs(throttleInp) > 0.01f)              // Input Actions’tan geliyorsa
            curSpeed += throttleInp * accelRate * Time.deltaTime;

        else                                             // Klavye yedeği
        {
            var kb = Keyboard.current;
            if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed)
                curSpeed += accelRate * Time.deltaTime;
            else if (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed)
                curSpeed -= decelRate * Time.deltaTime;
            else
                curSpeed -= idleDecay * Time.deltaTime;
        }

        curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);
    }

    /* ------------ Dönüş ------------------------- */
    void HandleRotation()
    {
        bool canPitch = !isGrounded || curSpeed >= takeoffSpeed;

        float pitch = canPitch ? -moveInp.y * pitchRate * Time.deltaTime : 0f;
        float roll = -moveInp.x * rollRate * Time.deltaTime;
        float yaw = yawInp * yawRate * Time.deltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    /* ------------ İleri İtme (Physics) ---------- */
    void MoveForward()
    {
        if (curSpeed <= 0.01f) { rb.linearVelocity = Vector3.zero; return; }

        Vector3 vel = transform.forward * curSpeed;
        vel.y = rb.linearVelocity.y;                  // dikey hızı koru (gravity)
        rb.linearVelocity = vel;
    }
}
