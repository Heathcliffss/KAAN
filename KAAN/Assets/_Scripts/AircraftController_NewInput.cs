using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AircraftController_NewInput : MonoBehaviour
{
    /* ───────── Uçuş Parametreleri ───────── */
    [Header("Hız")]
    public float maxSpeed = 160f;
    public float accelRate = 40f;
    public float decelRate = 30f;
    public float idleDecay = 8f;
    public float takeoffSpeed = 90f;

    [Header("Dönüş")]
    public float pitchRate = 35f;
    public float rollRate = 55f;
    public float yawRate = 30f;
    [SerializeField] private float mouseSensitivity = 0.005f;

    [Header("Zemin Kontrolü")]
    public Transform[] groundCheckPoints;
    [SerializeField] private float groundCheckDistance = 3f;
    [SerializeField] private float gravity = 9.81f;

    /* ───────── Ateşleme Parametreleri ───────── */
    [Header("Silah Sistemi")]
    public GameObject bulletPrefab;         // 🔫 Prefab’in kendisi
    public Transform firePoint;             // 🔫 Uçağın burnundaki çıkış
    public float bulletSpeed = 300f;        // mermi hızı
    public float fireRate = 0.2f;           // dakikada kaç saniye? (0.2 = 5/sn)

    [Header("UI")]
    public TMP_Text speedText;

    /* ───────── Dahili Değişkenler ───────── */
    bool engineOn = false;
    bool isGrounded = false;
    float curSpeed = 0f;
    float verticalVelocity = 0f;
    float fireTimer = 0f;

    Vector2 moveInp = Vector2.zero;
    float yawInp = 0f;
    float throttleInp = 0f;

    /* ───────── INPUT CALLBACK’LER ───────── */
    public void OnMove1(InputAction.CallbackContext c) => moveInp = c.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext c) => yawInp = c.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext c) => throttleInp = c.ReadValue<float>();

    /* ───────── START ───────── */
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /* ───────── UPDATE ───────── */
    void Update()
    {
        GroundCheck();
        ToggleEngine();
        HandleThrottle();
        HandleRotation();
        SimulateGravity();
        MoveForward();
        UpdateSpeedUI();
        HandleShooting();        // 🔫 Ateş
    }

    /* ───────── Motor ON/OFF ───────── */
    void ToggleEngine()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            engineOn = !engineOn;
            if (!engineOn) curSpeed = 0f;
            Debug.Log(engineOn ? "🟢 Motor ON" : "🔴 Motor OFF");
        }
    }

    /* ───────── Gaz / Hız ───────── */
    void HandleThrottle()
    {
        if (!engineOn) return;

        if (Mathf.Abs(throttleInp) > 0.01f)
            curSpeed += throttleInp * accelRate * Time.deltaTime;

        var kb = Keyboard.current;
        if (kb.leftShiftKey.isPressed) curSpeed += accelRate * Time.deltaTime;
        else if (kb.leftCtrlKey.isPressed) curSpeed -= decelRate * Time.deltaTime;
        else curSpeed -= idleDecay * Time.deltaTime;

        curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);
    }

    /* ───────── Dönüş ───────── */
    void HandleRotation()
    {
        if (!engineOn) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float pitch = -mouseDelta.y * pitchRate * mouseSensitivity;
        float yaw = mouseDelta.x * yawRate * mouseSensitivity;

        float rollInput = -moveInp.x;
        float roll = Mathf.Abs(rollInput) > 0.01f ? rollInput * rollRate * Time.deltaTime : AutoLevelRoll();

        if (curSpeed < takeoffSpeed && isGrounded) pitch = 0f;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    float AutoLevelRoll()
    {
        float z = transform.localEulerAngles.z;
        if (z > 180f) z -= 360f;
        return Mathf.Lerp(z, 0f, Time.deltaTime * 2f) - z;
    }

    /* ───────── İleri Hareket ───────── */
    void MoveForward()
    {
        if (!engineOn || curSpeed <= 0.01f) return;
        transform.position += transform.forward * curSpeed * Time.deltaTime;
    }

    /* ───────── Yerçekimi ───────── */
    void SimulateGravity()
    {
        if (isGrounded) { verticalVelocity = 0f; return; }
        verticalVelocity -= gravity * Time.deltaTime;
        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
    }

    /* ───────── Zemin Algılama ───────── */
    void GroundCheck()
    {
        isGrounded = false;
        foreach (var p in groundCheckPoints)
        {
            Debug.DrawRay(p.position, Vector3.down * groundCheckDistance, Color.red);
            if (Physics.Raycast(p.position, Vector3.down, groundCheckDistance))
            {
                isGrounded = true; break;
            }
        }
    }

    /* ───────── UI Hız Güncelle ───────── */
    void UpdateSpeedUI()
    {
        if (speedText) speedText.text = $"HIZ: {Mathf.RoundToInt(curSpeed)} km/h";
    }

    /* ─────────🔫 Ateşleme Sistemi───────── */
    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && engineOn && fireTimer >= fireRate)
        {
            fireTimer = 0f;
            if (bulletPrefab && firePoint)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                // Mermiye Rigidbody varsa hız ver
                Rigidbody brb = bullet.GetComponent<Rigidbody>();
                if (brb)
                    brb.linearVelocity = firePoint.forward * bulletSpeed;
                else
                    bullet.transform.forward = firePoint.forward;  // fallback

                Destroy(bullet, 5f); // 5 sn sonra temizle
            }
        }
    }
}
