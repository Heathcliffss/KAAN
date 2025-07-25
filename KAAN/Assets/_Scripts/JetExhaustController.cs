using UnityEngine;

public class JetExhaustController : MonoBehaviour
{
    [SerializeField] private ParticleSystem jetParticle;
    [SerializeField] private float maxRateOverTime = 100f;
    [SerializeField] private float maxStartSpeed = 5f;
    [SerializeField] private float responseSpeed = 2f;

    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;

    private float currentRate = 0f;
    private float currentSpeed = 0f;

    private AirplaneController airplaneController;

    void Start()
    {
        if (jetParticle == null)
        {
            Debug.LogError("🚨 Jet Particle atanmadı!");
            return;
        }

        emission = jetParticle.emission;
        main = jetParticle.main;

        airplaneController = GetComponentInParent<AirplaneController>();
        if (airplaneController == null)
        {
            Debug.LogError("🚨 Ebeveyn objede AirplaneController bulunamadı!");
        }

        emission.rateOverTime = 0;
        main.startSpeed = 0;
    }

    void Update()
    {
        if (airplaneController == null) return;

        float targetRate = airplaneController.thrustPercent * maxRateOverTime;
        float targetSpeed = airplaneController.thrustPercent * maxStartSpeed;

        currentRate = Mathf.Lerp(currentRate, targetRate, Time.deltaTime * responseSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * responseSpeed);

        emission.rateOverTime = currentRate;
        main.startSpeed = currentSpeed;
    }
}
