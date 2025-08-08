using System.Collections;
using System.Reflection;
using UnityEngine;

public class AirDefenseSystem : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform missileLaunchPoint;
    public float launchInterval = 3f;

    private Transform currentTarget;
    private Coroutine firingCoroutine;

    public void StartFiringAt(Transform target)
    {
        currentTarget = target;

        if (firingCoroutine == null)
            firingCoroutine = StartCoroutine(FireAtTarget());
    }

    public void StopFiring()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }

        currentTarget = null;
    }

    private IEnumerator FireAtTarget()
    {
        while (currentTarget != null)
        {
            GameObject missile = Instantiate(missilePrefab, missileLaunchPoint.position, Quaternion.identity);
            missile.GetComponent<MissileFollowHSS>().SetTarget(currentTarget);

            yield return new WaitForSeconds(launchInterval);
        }
    }

    // AirDefenseSystem.cs
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(">> Oyuncu hava sahasına girdi, füze ateşi başlıyor.");
            StartFiringAt(other.transform); // işte bu zaten Transform!
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopFiring();
        }
    }
}
