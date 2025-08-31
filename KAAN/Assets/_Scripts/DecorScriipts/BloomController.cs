using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class BloomController : MonoBehaviour
{
    public Volume globalVolume;   // Inspector'dan baðla
    public float thrust = 0f;     // Oyun mekaniklerinden set edilecek

    private Bloom bloom;
    private bool isBloomActive = false;
    private float previousThrust = 0f;

    [Header("Fade Ayarlarý")]
    public float fadeInDuration = 0.5f;   // Bloom yavaþça artýþ süresi
    public float fadeOutDuration = 1f;    // Bloom yavaþça azalacak süre
    public float bloomDuration = 4f;      // Bloom sabit kalma süresi

    public AirplaneController airplaneController;

    void Start()
    {
        if (globalVolume.profile.TryGet(out bloom))
        {
            bloom.intensity.value = 0f; // Baþlangýçta 0
        }
    }

    void Update()
    {
        thrust = airplaneController.thrustPercent;
        if (thrust > previousThrust && !isBloomActive)
        {
            StartCoroutine(BloomEffectRoutine());
        }

        previousThrust = thrust;
    }

    private System.Collections.IEnumerator BloomEffectRoutine()
    {
        isBloomActive = true;

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            bloom.intensity.value = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        bloom.intensity.value = 1f; // Kesin 1

        // Bloom sabit kalma süresi
        yield return new WaitForSeconds(bloomDuration);

        // Fade out
        elapsed = 0f;
        float startIntensity = bloom.intensity.value;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            bloom.intensity.value = Mathf.Lerp(startIntensity, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        bloom.intensity.value = 0f; // Kesin 0
        isBloomActive = false;
    }
}
