using UnityEngine;

public class JetEngineSoundController : MonoBehaviour
{
    public AudioSource RunningSound;
    public float RunningMaxVolume = 1f;
    public float RunningMaxPitch = 1.5f;

    public AudioSource idleSound;
    public float idleMaxVolume = 0.8f;
    public float idleMaxPitch = 1.2f;

    [Header("Wind Sound")]
    public AudioSource windSound;
    public float windMaxVolume = 0.6f;
    public float windMinVolume = 0.1f;
    public float windPitch = 1f;

    private AirplaneController airplaneController;
    private float thrust;

    void Start()
    {
        airplaneController = GetComponent<AirplaneController>();

        if (!RunningSound || !idleSound || !windSound)
        {
            Debug.LogError("JetEngineSoundController: Bir veya daha fazla AudioSource eksik!");
        }
    }

    void Update()
    {
        if (!airplaneController) return;

        thrust = airplaneController.GetThrustPercent(); // 0.0 - 1.0

        // Jet motorları (thrust'a bağlı)
        float idleVolume = Mathf.Lerp(0.1f, idleMaxVolume, 1f - thrust);
        float idlePitch = Mathf.Lerp(0.8f, idleMaxPitch, 1f - thrust);

        float runningVolume = Mathf.Lerp(0.1f, RunningMaxVolume, thrust);
        float runningPitch = Mathf.Lerp(0.7f, RunningMaxPitch, thrust);

        if (idleSound)
        {
            idleSound.volume = idleVolume;
            idleSound.pitch = idlePitch;
        }

        if (RunningSound)
        {
            RunningSound.volume = runningVolume;
            RunningSound.pitch = runningPitch;
        }

        // 🌬️ Rüzgar sesi (Roll değerine bağlı)
        float rollIntensity = Mathf.Abs(airplaneController.Roll); // -1 ile 1 arasında
        float windTargetVolume = Mathf.Lerp(windMinVolume, windMaxVolume, rollIntensity);
        windSound.volume = Mathf.MoveTowards(windSound.volume, windTargetVolume, Time.deltaTime * 3f);
        windSound.pitch = windPitch;
    }
}
