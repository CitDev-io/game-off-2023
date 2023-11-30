
using UnityEngine;

public class UI_ChangeOnSpace : MonoBehaviour
{
    public ChangeScene changer;
    public string sceneName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            changer.SwapToScene(sceneName);
        }
    }
}
