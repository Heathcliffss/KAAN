using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public AirDefenseSystem airDefenseSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            airDefenseSystem.StartFiringAt(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            airDefenseSystem.StopFiring();
        }
    }
}
