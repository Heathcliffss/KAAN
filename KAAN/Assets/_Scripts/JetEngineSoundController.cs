using UnityEngine;

[RequireComponent(typeof(AirplaneController))]
public class JetEngineSoundController : MonoBehaviour
{
    [Header("Ses Kaynakları")]
    public AudioSource idleSound;
    public AudioSource runningSound;
    public AudioSource windSound;

    [Header("Ses Ayarları")]
    public float idleMaxVolume = 0.6f;
    public float runningMaxVolume = 0.8f;
    public float windMinVolume = 0.0f;
    public float windMaxVolume = 0.7f;

    [Header("Wind Fade Ayarı")]
    public float windFadeDuration = 0.5f; // Rüzgar sesi fade-in süresi

    private AirplaneController airplaneController;
    private float thrust;
    private float windFadeTimer = 0f;

    void Start()
    {
        airplaneController = GetComponent<AirplaneController>();

        if (!runningSound || !idleSound || !windSound)
        {
            Debug.LogError("JetEngineSoundController: Bir veya daha fazla AudioSource eksik!");
            return;
        }

        // Play On Awake yerine manuel başlatma
        idleSound.playOnAwake = false;
        runningSound.playOnAwake = false;
        windSound.playOnAwake = false;

        idleSound.volume = 0f;
        runningSound.volume = 0f;
        windSound.volume = 0f;

        idleSound.loop = true;
        runningSound.loop = true;
        windSound.loop = true;

        idleSound.Play();
        runningSound.Play();
        windSound.Play();
    }

    void Update()
    {
        if (!airplaneController) return;

        thrust = airplaneController.GetThrustPercent();

        // Idle sesi - Thrust düşükken yüksek, yüksekken düşük
        float idleTarget = Mathf.Lerp(0.1f, idleMaxVolume, 1f - thrust);
        idleSound.volume = Mathf.MoveTowards(idleSound.volume, idleTarget, Time.deltaTime * 2f);

        // Running sesi - Thrust arttıkça yükselir
        float runningTarget = Mathf.Lerp(0.1f, runningMaxVolume, thrust);
        runningSound.volume = Mathf.MoveTowards(runningSound.volume, runningTarget, Time.deltaTime * 2f);

        // Wind sesi - Fade-in ile başlar, roll ile artar
        windFadeTimer += Time.deltaTime;
        float fadeMultiplier = Mathf.Clamp01(windFadeTimer / windFadeDuration);

        float rollIntensity = Mathf.Abs(airplaneController.Roll);
        float windTargetVolume = Mathf.Lerp(windMinVolume, windMaxVolume, rollIntensity) * fadeMultiplier;
        windSound.volume = Mathf.MoveTowards(windSound.volume, windTargetVolume, Time.deltaTime * 3f);
    }
}
