using UnityEngine;
using UnityEngine.UI;

public class UI_ClickOnSpace : MonoBehaviour
{
    public Button button;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            button.onClick.Invoke();
        }
    }
}
