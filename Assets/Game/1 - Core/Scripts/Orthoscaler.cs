using UnityEngine;

[ExecuteInEditMode]
public class Orthoscaler : MonoBehaviour
{
    public float targetAspect = 9f / 16f; // Target aspect ratio (16:9 in this case)
    public float actualAspect = 0f;
    public float originalOrthographicSize = 5.6f; // Original orthographic size
    public float scaleToModel = 1f;
    public Camera cam;
    public float scaleMod = 3.25f;

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
    }

    void Update()
    {
        actualAspect = (float)Screen.height / Screen.width;
        scaleToModel = targetAspect / actualAspect;

        if (scaleToModel > .995) {
            cam.orthographicSize = originalOrthographicSize;
        }

        if (scaleToModel < .995) {
            var difference = 1 - scaleToModel;
            cam.orthographicSize = originalOrthographicSize + (difference * scaleMod);
        }
    }
}