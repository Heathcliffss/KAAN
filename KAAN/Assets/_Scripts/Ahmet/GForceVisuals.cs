using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using TMPro;

public class GForceVisuals : MonoBehaviour
{
    public Rigidbody rb;
    public Volume volume;
    public TMP_Text gForceText; // UI metni

    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;

    private Vector3 lastVelocity;

    // Yazı görünme ayarları
    public float displayThreshold = 1.5f; // Kaç G üstünde yazı çıksın?
    public float textFadeSpeed = 5f;     // Yazı kaybolma hızı
    private float textAlpha = 0f;

    void Start()
    {
        lastVelocity = rb.linearVelocity;

        if (!volume.profile.TryGet<Vignette>(out vignette))
            Debug.LogError("Vignette bulunamadı, Volume profiline ekleyin!");
        if (!volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
            Debug.LogError("Color Adjustments bulunamadı, Volume profiline ekleyin!");
        if (!volume.profile.TryGet<Bloom>(out bloom))
            Debug.LogError("Bloom bulunamadı, Volume profiline ekleyin!");
        if (!volume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            Debug.LogError("Chromatic Aberration bulunamadı, Volume profiline ekleyin!");
        if (!volume.profile.TryGet<MotionBlur>(out motionBlur))
            Debug.LogError("Motion Blur bulunamadı, Volume profiline ekleyin!");

        if (gForceText != null)
            gForceText.text = "";
    }

    void FixedUpdate()
    {
        Vector3 acceleration = (rb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
        float gForce = Vector3.Dot(acceleration, -Physics.gravity.normalized) / 9.81f;
        lastVelocity = rb.linearVelocity;

        // Efektleri uygula
        ApplyVisualEffects(gForce);

        // G yazısını göster
        HandleGForceText(gForce);
    }

    void ApplyVisualEffects(float gForce)
    {
        if (gForce > 2f) // 2 G'den sonra başlasın
        {
            // 2 G → 0, 5 G → 1 olacak şekilde normalize et
            float intensity = Mathf.Clamp01((gForce - 2f) / 3f);

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, intensity * 0.9f, Time.fixedDeltaTime * 1f);
            vignette.color.value = Color.black;
            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, -intensity * 1.5f, Time.fixedDeltaTime * 1f);
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, Mathf.Lerp(10f, 5f, intensity), Time.fixedDeltaTime * 1f);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, intensity * 0.4f, Time.fixedDeltaTime * 1f);
            motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, intensity * 0.7f, Time.fixedDeltaTime * 1f);
        }
        else if (gForce < -1f) // Negatif G (aşağıya yapışma hissi)
        {
            // -1 G → 0, -4 G → 1
            float intensity = Mathf.Clamp01((-gForce - 1f) / 3f);

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, intensity * 0.7f, Time.fixedDeltaTime * 1f);
            vignette.color.value = Color.Lerp(vignette.color.value, Color.red * 0.8f, Time.fixedDeltaTime * 1f);
            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, -intensity * 1f, Time.fixedDeltaTime * 1f);
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 8f, Time.fixedDeltaTime * 1f);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0f, Time.fixedDeltaTime * 1f);
            motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, 0f, Time.fixedDeltaTime * 1f);
        }
        else
        {
            // Normal durumda efektler sıfırlansın
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, Time.fixedDeltaTime * 1f);
            vignette.color.value = Color.Lerp(vignette.color.value, Color.black, Time.fixedDeltaTime * 1f);
            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, 0f, Time.fixedDeltaTime * 1f);
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 10f, Time.fixedDeltaTime * 1f);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0f, Time.fixedDeltaTime * 1f);
            motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, 0f, Time.fixedDeltaTime * 1f);
        }
    }

    void HandleGForceText(float gForce)
    {
        if (Mathf.Abs(gForce) >= displayThreshold)
        {
            textAlpha = 1f;
            gForceText.text = $"{(gForce > 0 ? "+" : "-")}{Mathf.Abs(gForce).ToString("0.0")} G";
        }
        else
        {
            textAlpha = Mathf.Lerp(textAlpha, 0f, Time.fixedDeltaTime * textFadeSpeed);
        }

        if (gForceText != null)
        {
            Color c = gForceText.color;
            c.a = textAlpha;
            gForceText.color = c;
        }
    }
}
