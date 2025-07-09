using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform body;     // Y ekseninde döner
    public Transform head;     // X ekseninde döner
    public Transform target;   // Takip edilecek hedef

    [Header("Ayarlar")]
    public float bodySpeed = 5f;
    public float headSpeed = 3f;
    public float headMinAngle = -30f;
    public float headMaxAngle = 45f;

    void Update()
    {
        if (!target) return;

        RotateBody();
        RotateHead();
    }

    void RotateBody()
    {
        Vector3 flatDir = target.position - body.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(flatDir);
        float targetY = targetRot.eulerAngles.y;

        // Þu anki body'sinin Y rotasyonu
        float currentY = body.rotation.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * bodySpeed);

        // Burada 270 X rotasyonu sabit tutuluyor
        body.rotation = Quaternion.Euler(270f, newY, 0f);
    }

    void RotateHead()
    {
        Vector3 dirToTarget = target.position - head.position;
        Quaternion lookRot = Quaternion.LookRotation(dirToTarget, body.up);
        float targetX = NormalizeAngle(lookRot.eulerAngles.x);
        targetX = Mathf.Clamp(targetX, headMinAngle, headMaxAngle);

        Vector3 currentEuler = head.localEulerAngles;
        float currentX = NormalizeAngle(currentEuler.x);
        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * headSpeed);

        head.localRotation = Quaternion.Euler(newX, 0f, 0f);
    }

    float NormalizeAngle(float angle)
    {
        return (angle > 180f) ? angle - 360f : angle;
    }
}
