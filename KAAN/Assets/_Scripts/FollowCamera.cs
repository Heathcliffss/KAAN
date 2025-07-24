using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 2f;  // Konum ge�i�i
    public float lookSpeed = 2f;    // Y�n ge�i�i

    void LateUpdate()
    {
        if (target == null) return;

        // Konum ge�i�i (smooth follow)
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

        // Y�n ge�i�i (smooth look)
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
