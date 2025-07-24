using UnityEngine;
using TMPro;

public class AltimeterController : MonoBehaviour
{
    public Transform Ucak;
    public RectTransform altitudeScale;
    public float altitudeUnitSize = 20f; // 10 birim irtifa = 20px kayma
    public float scaleStep = 50f; // G�sterilen her text 10 birim aral�kla art�yor

    void Update()
    {
        float altitude = Ucak.position.y;

        // �l�ek �izelgesini yukar�/a�a�� kayd�r
        float offset = altitude / scaleStep * altitudeUnitSize;
        altitudeScale.anchoredPosition = new Vector2(0f, offset);
    }
}
