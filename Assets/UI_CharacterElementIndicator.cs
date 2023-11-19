using UnityEngine;

public class UI_CharacterElementIndicator : MonoBehaviour
{
    public Sprite Sun;
    public Sprite Moon;
    
    public void SetPowerType(PowerType type) {
    if (type == PowerType.LIGHT) {
        GetComponent<SpriteRenderer>().sprite = Sun;
    } else {
        GetComponent<SpriteRenderer>().sprite = Moon;
    }
    }
}
