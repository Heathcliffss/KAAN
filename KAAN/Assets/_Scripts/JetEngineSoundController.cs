using UnityEngine;

[RequireComponent(typeof(AirplaneController))]
public class JetEngineSoundController : MonoBehaviour
{
    [Header("Motor Sesleri")]
    public AudioSource idleSound;
    public AudioSource runningSound;
    public AudioSource windSound;

    [Header("Motor Ses Ayarları")]
    public float idleMaxVolume = 0.6f;
    public float runningMaxVolume = 0.8f;
    public float windMinVolume = 0.0f;
    public float windMaxVolume = 0.7f;

    [Header("Wind Fade Ayarı")]
    public float windFadeDuration = 0.5f;

    [Header("Silah Sesleri")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip hitSound;
    public float gunSoundVolume = 1f;

    [Header("Patlama Sesleri")]
    public AudioClip explosionSound;
    public float explosionVolume = 1f;

    private AirplaneController airplaneController;
    private float thrust;
    private float windFadeTimer = 0f;

    void Start()
    {
        airplaneController = GetComponent<AirplaneController>();

        if (!idleSound || !runningSound || !windSound)
        {
            Debug.LogError("JetEngineSoundController: Ses kaynakları eksik!");
            return;
        }

        idleSound.playOnAwake = runningSound.playOnAwake = windSound.playOnAwake = false;
        idleSound.loop = runningSound.loop = windSound.loop = true;

        idleSound.volume = 0f;
        runningSound.volume = 0f;
        windSound.volume = 0f;

        idleSound.Play();
        runningSound.Play();
        windSound.Play();
    }

    void Update()
    {
        if (!airplaneController) return;

        thrust = Mathf.Clamp01(airplaneController.GetThrustPercent());

        float idleTarget = Mathf.Lerp(0.1f, idleMaxVolume, 1f - thrust);
        idleSound.volume = Mathf.MoveTowards(idleSound.volume, idleTarget, Time.deltaTime * 2f);

        float runningTarget = Mathf.Lerp(0.05f, runningMaxVolume, thrust);
        runningSound.volume = Mathf.MoveTowards(runningSound.volume, runningTarget, Time.deltaTime * 2f);

        windFadeTimer += Time.deltaTime;
        float fadeMultiplier = Mathf.Clamp01(windFadeTimer / windFadeDuration);

        float rollIntensity = Mathf.Abs(airplaneController.Roll);
        float windTargetVolume = Mathf.Lerp(windMinVolume, windMaxVolume, rollIntensity) * fadeMultiplier;
        windSound.volume = Mathf.MoveTowards(windSound.volume, windTargetVolume, Time.deltaTime * 3f);
    }

    // 🎯 Silah sesleri
    public void PlayFireSound(Vector3 pos)
    {
        if (fireSound != null)
            AudioSource.PlayClipAtPoint(fireSound, pos, gunSoundVolume);
    }

    public void PlayReloadSound(Vector3 pos)
    {
        if (reloadSound != null)
            AudioSource.PlayClipAtPoint(reloadSound, pos, gunSoundVolume);
    }

    public void PlayHitSound(Vector3 pos)
    {
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, pos, gunSoundVolume);
    }

    // 🎯 Patlama sesi
    public void PlayExplosionSound(Vector3 pos)
    {
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, pos, explosionVolume);
    }
}
