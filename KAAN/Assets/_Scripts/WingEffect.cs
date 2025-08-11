using UnityEngine;

public class WingEffect : MonoBehaviour
{
    public ParticleSystem leftWingEffect;
    public ParticleSystem rightWingEffect;

    public AirplaneController airplaneController;

    [Header("Roll Ayarları")]
    public float rollThreshold = 10f;
    public float maxRoll = 45f;

    [Header("Pitch Ayarları")]
    public float pitchThreshold = 10f;
    public float maxPitch = 45f;

    [Header("Emission Ayarları")]
    public float minEmissionRate = 0f;
    public float maxEmissionRate = 50f;

    void Update()
    {
        if (!airplaneController || !leftWingEffect || !rightWingEffect) return;

        float rollInput = airplaneController.Roll;   // -1 (sola yatış) ila 1 (sağa yatış)
        float pitchInput = airplaneController.Pitch; // -1 (burun aşağı) ila 1 (burun yukarı)

        float rollAngle = rollInput * maxRoll;
        float pitchAngle = pitchInput * maxPitch;

        // Sol kanat için hem sola roll hem de pitch etkisini hesapla
        float leftRollIntensity = rollAngle < 0 ? Mathf.InverseLerp(rollThreshold, maxRoll, Mathf.Abs(rollAngle)) : 0f;
        float leftPitchIntensity = Mathf.InverseLerp(pitchThreshold, maxPitch, Mathf.Abs(pitchAngle));
        float leftEmission = Mathf.Lerp(minEmissionRate, maxEmissionRate, Mathf.Max(leftRollIntensity, leftPitchIntensity));

        // Sağ kanat için hem sağa roll hem de pitch etkisini hesapla
        float rightRollIntensity = rollAngle > 0 ? Mathf.InverseLerp(rollThreshold, maxRoll, Mathf.Abs(rollAngle)) : 0f;
        float rightPitchIntensity = Mathf.InverseLerp(pitchThreshold, maxPitch, Mathf.Abs(pitchAngle));
        float rightEmission = Mathf.Lerp(minEmissionRate, maxEmissionRate, Mathf.Max(rightRollIntensity, rightPitchIntensity));

        SetEmissionRate(leftWingEffect, leftEmission);
        SetEmissionRate(rightWingEffect, rightEmission);
    }

    private void SetEmissionRate(ParticleSystem ps, float rate)
    {
        var emission = ps.emission;
        var rateOverTime = emission.rateOverTime;
        rateOverTime.constant = rate;
        emission.rateOverTime = rateOverTime;
    }

}
