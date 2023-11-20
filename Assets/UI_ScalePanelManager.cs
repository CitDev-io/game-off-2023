using TMPro;
using UnityEngine;

public class UI_ScalePanelManager : MonoBehaviour
{
    public TextMeshProUGUI Light;
    public TextMeshProUGUI Shadow;


    public void SetScale(int lightCount, int shadowCount) {
        Light.text = lightCount.ToString();
        Shadow.text = shadowCount.ToString();
    }
}
