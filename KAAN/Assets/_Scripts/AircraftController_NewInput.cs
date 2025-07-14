using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftController_NewInput : MonoBehaviour
{
    [Header("Hız Parametreleri")]
    public float maxSpeed = 160f;
    public float accelRate = 40f;
    public float decelRate = 30f;
    public float idleDecay = 8f;

    [Header("Kalkış Eşiği")]
    public float takeoffSpeed = 90f;

    [Header("Dönüş Hızları")]
    public float pitchRate = 35f;
    public float rollRate = 55f;
    public float yawRate = 30f;

    [Header("Mouse Ayarları")]
    [SerializeField] private float mouseSensitivity = 0.005f;

    [Header("Fiziksel Simülasyon")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float groundCheckDistance = 3f;

    [Header("Zemin Kontrol Noktaları")]
    public Transform[] groundCheckPoints;  // ✅ Alt noktaları dışarıdan atayacaksın

    private bool engineOn = false;
    private float curSpeed = 0f;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    private Vector2 moveInp = Vector2.zero;
    private float yawInp = 0f;
    private float throttleInp = 0f;

    // INPUT CALLBACKS
    public void OnMove1(InputAction.CallbackContext c) => moveInp = c.ReadValue<Vector2>();
    public void OnYaw(InputAction.CallbackContext c) => yawInp = c.ReadValue<float>();
    public void OnThrottle(InputAction.CallbackContext c) => throttleInp = c.ReadValue<float>();

    void Start()
    {
        engineOn = false;
        curSpeed = 0f;
        verticalVelocity = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GroundCheck();
        ToggleEngine();
        HandleThrottle();
        HandleRotation();
        SimulateGravity();
        MoveForward();
    }

    void ToggleEngine()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            engineOn = !engineOn;
            if (!engineOn)
                curSpeed = 0f;

            Debug.Log(engineOn ? "<color=green>Motor ON</color>" : "<color=red>Motor OFF</color>");
        }
    }

    void HandleThrottle()
    {
        if (!engineOn) return;

        if (Mathf.Abs(throttleInp) > 0.01f)
            curSpeed += throttleInp * accelRate * Time.deltaTime;

        var kb = Keyboard.current;
        if (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed)
            curSpeed += accelRate * Time.deltaTime;
        else if (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed)
            curSpeed -= decelRate * Time.deltaTime;
        else
            curSpeed -= idleDecay * Time.deltaTime;

        curSpeed = Mathf.Clamp(curSpeed, 0f, maxSpeed);
    }

    void HandleRotation()
    {
        if (!engineOn) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float pitch = -mouseDelta.y * pitchRate * mouseSensitivity;
        float yaw = mouseDelta.x * yawRate * mouseSensitivity;

        float rollInput = -moveInp.x;
        float roll = 0f;

        if (Mathf.Abs(rollInput) > 0.01f)
        {
            roll = rollInput * rollRate * Time.deltaTime;
        }
        else
        {
            float currentZ = transform.localEulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f;

            float correctedZ = Mathf.Lerp(currentZ, 0f, Time.deltaTime * 2f);
            float deltaZ = correctedZ - currentZ;
            roll = deltaZ;
        }

        if (curSpeed < takeoffSpeed && isGrounded)
            pitch = 0f;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }

    void MoveForward()
    {
        if (!engineOn || curSpeed <= 0.01f) return;

        transform.position += transform.forward * curSpeed * Time.deltaTime;
    }

    void SimulateGravity()
    {
        if (isGrounded)
        {
            verticalVelocity = 0f;
            return;
        }

        verticalVelocity -= gravity * Time.deltaTime;
        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
    }

    void GroundCheck()
    {
        isGrounded = false;

        if (groundCheckPoints == null || groundCheckPoints.Length == 0)
        {
            Debug.LogWarning("⚠️ Ground check noktaları atanmadı!");
            return;
        }

        foreach (var point in groundCheckPoints)
        {
            Debug.DrawRay(point.position, Vector3.down * groundCheckDistance, Color.red);
            if (Physics.Raycast(point.position, Vector3.down, groundCheckDistance))
            {
                isGrounded = true;
                break;
            }
        }
    }
}
