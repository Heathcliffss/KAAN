using UnityEngine;

public class WingWindEffectController : MonoBehaviour
{
    public ParticleSystem leftWingEffect;
    public ParticleSystem rightWingEffect;

    public AirplaneController airplaneController;
    public Rigidbody rb;

    [Header("Roll Ayarlarý")]
    public float rollThreshold = 10f;
    public float maxRoll = 45f;

    [Header("Pitch Ayarlarý")]
    public float pitchThreshold = 10f;
    public float maxPitch = 45f;

    [Header("Emission Ayarlarý")]
    public float minEmissionRate = 0f;
    public float maxEmissionRate = 50f;

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        if (!airplaneController || !leftWingEffect || !rightWingEffect) return;

        if (speed > 40f) // Sadece hýz 40’tan büyükse efekt çalýþsýn
        {
            float rollInput = airplaneController.Roll;   // -1 (sola yatýþ) ila 1 (saða yatýþ)
            float pitchInput = airplaneController.Pitch; // -1 (burun aþaðý) ila 1 (burun yukarý)

            float rollAngle = rollInput * maxRoll;
            float pitchAngle = pitchInput * maxPitch;

            // Sol kanat için hem sola roll hem de pitch etkisini hesapla
            float leftRollIntensity = rollAngle < 0 ? Mathf.InverseLerp(rollThreshold, maxRoll, Mathf.Abs(rollAngle)) : 0f;
            float leftPitchIntensity = Mathf.InverseLerp(pitchThreshold, maxPitch, Mathf.Abs(pitchAngle));
            float leftEmission = Mathf.Lerp(minEmissionRate, maxEmissionRate, Mathf.Max(leftRollIntensity, leftPitchIntensity));

            // Sað kanat için hem saða roll hem de pitch etkisini hesapla
            float rightRollIntensity = rollAngle > 0 ? Mathf.InverseLerp(rollThreshold, maxRoll, Mathf.Abs(rollAngle)) : 0f;
            float rightPitchIntensity = Mathf.InverseLerp(pitchThreshold, maxPitch, Mathf.Abs(pitchAngle));
            float rightEmission = Mathf.Lerp(minEmissionRate, maxEmissionRate, Mathf.Max(rightRollIntensity, rightPitchIntensity));

            SetEmissionRate(leftWingEffect, leftEmission);
            SetEmissionRate(rightWingEffect, rightEmission);
        }
        else
        {
            // Hýz 40'tan küçükse emission'u sýfýrla
            SetEmissionRate(leftWingEffect, 0f);
            SetEmissionRate(rightWingEffect, 0f);
        }
    }

    private void SetEmissionRate(ParticleSystem ps, float rate)
    {
        var emission = ps.emission;
        var rateOverTime = emission.rateOverTime;
        rateOverTime.constant = rate;
        emission.rateOverTime = rateOverTime;
    }
}
