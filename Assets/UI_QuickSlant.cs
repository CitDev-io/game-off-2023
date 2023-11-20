using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuickSlant : MonoBehaviour
{
    public RectTransform Panel1;
    public RectTransform Panel2;
    public RectTransform Panel3;
    public RectTransform Panel4;
    public float SPEED = 0.01f;
    public float AMOUNT = 1.2f;

    public float DELAY1 = 1f;
    public float DELAY2 = 1f;
    public float DELAY3 = 1f;
    public float DELAY4 = 1f;


    void Start() {
        Slant();
    }

    void Slant() {
        StartCoroutine(SlantIn(Panel1, DELAY1));
        StartCoroutine(SlantIn(Panel2, DELAY2));
        StartCoroutine(SlantIn(Panel3, DELAY3));
        StartCoroutine(SlantIn(Panel4, DELAY4));
    }

    IEnumerator SlantIn(RectTransform panel, float delay) {
        yield return new WaitForSeconds(delay);
        while (panel.sizeDelta.y < 700f) {
            yield return new WaitForSeconds(SPEED);
            panel.sizeDelta = new Vector2(200f, panel.sizeDelta.y + AMOUNT);
        
        }
    }

    [ContextMenu("Slant")]
    void SlantAgain() {
        Panel1.sizeDelta = new Vector2(200f, 0f);
        Panel2.sizeDelta = new Vector2(200f, 0f);
        Panel3.sizeDelta = new Vector2(200f, 0f);
        Panel4.sizeDelta = new Vector2(200f, 0f);
        Slant();
    }
}
