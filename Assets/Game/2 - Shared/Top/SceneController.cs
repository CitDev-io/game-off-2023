using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject DDOLobj;

    void Awake()
    {
        if (GameObject.FindObjectOfType<GameController_DDOL>() == null)
        {
            Instantiate(DDOLobj);
        }
    }
}
