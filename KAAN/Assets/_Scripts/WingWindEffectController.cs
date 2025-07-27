using UnityEngine;

public class WingWindEffectController : MonoBehaviour
{
    public ParticleSystem leftWingEffect;
    public ParticleSystem rightWingEffect;

    public AirplaneController airplaneController;

    [Header("Roll Ayarlarý")]
    public float rollThreshold = 10f; // Etkinleþme eþiði (derece)
    public float maxRoll = 45f;       // Maksimum etki eþiði

    [Header("Emission Ayarlarý")]
    public float minEmissionRate = 0f;
    public float maxEmissionRate = 50f;

    void Update()
    {
        if (!airplaneController || !leftWingEffect || !rightWingEffect) return;

        // Roll deðerini al (-1 ile 1 arasý)
        float rollInput = airplaneController.Roll;

        // Ýzin verilen aralýkla çarp, dereceye çevir
        float rollAngle = rollInput * maxRoll;

        // Emission oranýný hesapla (normalize edip mutlak deðer kullanýyoruz)
        float intensity = Mathf.InverseLerp(rollThreshold, maxRoll, Mathf.Abs(rollAngle));
        float emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, intensity);

        // Sol ve sað efekti güncelle
        SetEmissionRate(leftWingEffect, rollAngle < 0 ? emissionRate : 0f);
        SetEmissionRate(rightWingEffect, rollAngle > 0 ? emissionRate : 0f);
    }

    private void SetEmissionRate(ParticleSystem ps, float rate)
    {
        var emission = ps.emission;
        var rateOverTime = emission.rateOverTime;
        rateOverTime.constant = rate;
        emission.rateOverTime = rateOverTime;
    }
}
