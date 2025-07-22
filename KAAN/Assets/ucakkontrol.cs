using UnityEngine;

public class UcakKontrol : MonoBehaviour
{
    public float motorGucu = 1000f;
    public float donusHizi = 50f;
    public float kalkisHizi = 80f; // Kalk�� i�in gereken minimum h�z
    public Transform kanatlar;

    private Rigidbody rb;
    private bool havada = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float hizIleri = Input.GetAxis("Vertical"); // W ile gaz ver
        float yaw = Input.GetAxis("Horizontal");    // A - D y�n

        // �leri itme g�c�
        rb.AddForce(transform.forward * hizIleri * motorGucu * Time.fixedDeltaTime);

        // D�nme ama sadece havadaysa yumu�ak hareket
        if (havada)
        {
            float pitch = Input.GetAxis("Mouse Y");
            float roll = Input.GetAxis("Mouse X");

            Vector3 torque = new Vector3(-pitch, yaw, -roll) * donusHizi;
            rb.AddTorque(torque);
        }

        // Kalk�� kontrol�
        if (!havada && rb.linearVelocity.magnitude >= kalkisHizi)
        {
            rb.AddForce(Vector3.up * 5000f); // Yukar� do�ru kald�rma kuvveti
            havada = true;
        }
    }
}
