using UnityEngine;
using TMPro;

public class AltimeterController : MonoBehaviour
{
    public Transform Ucak;
    public RectTransform altitudeScale;
    public float altitudeUnitSize = 20f; // 10 birim irtifa = 20px kayma
    public float scaleStep = 50f; // Gösterilen her text 10 birim aralýkla artýyor

    void Update()
    {
        float altitude = Ucak.position.y;

        // Ölçek çizelgesini yukarý/aþaðý kaydýr
        float offset = altitude / scaleStep * altitudeUnitSize;
        altitudeScale.anchoredPosition = new Vector2(0f, offset);
    }
}
