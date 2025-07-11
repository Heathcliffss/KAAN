using UnityEngine;

public class MouseAimingController : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float rotationSpeed = 5f;

    [Header("Aim Settings")]
    public Camera mainCamera;
    public float aimDistance = 1000f;
    public Transform aimTarget; // boþ bir hedef objesi

    private Vector3 aimWorldPosition;

    void Update()
    {
        UpdateAimTarget();
        MoveForward();
        RotateTowardsAim();
    }

    void UpdateAimTarget()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        aimWorldPosition = ray.GetPoint(aimDistance);
        if (aimTarget != null)
            aimTarget.position = aimWorldPosition;
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void RotateTowardsAim()
    {
        Vector3 direction = (aimWorldPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
