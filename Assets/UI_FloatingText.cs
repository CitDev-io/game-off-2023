using System.Collections;
using UnityEngine;
using TMPro;

public class UI_FloatingText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    [ContextMenu("Test Me")]
    void TestMe() {
        Float("-25", Color.red);
    }

    public void Float(string text, Color color) {
        StartCoroutine(FloatRoutine(text, color));
    }

    IEnumerator FloatRoutine(string text, Color color) {
        Text.text = text;
        Text.color = color;
        float alpha = 1f;

        while (alpha > 0f) {
            alpha -= 0.015f;
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, alpha);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.015f, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
