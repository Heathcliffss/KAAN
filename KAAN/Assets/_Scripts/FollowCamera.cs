using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 2f;  // Konum geçiþi
    public float lookSpeed = 2f;    // Yön geçiþi

    void LateUpdate()
    {
        if (target == null) return;

        // Konum geçiþi (smooth follow)
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

        // Yön geçiþi (smooth look)
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
