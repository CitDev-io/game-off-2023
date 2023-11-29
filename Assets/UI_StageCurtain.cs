using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageCurtain : MonoBehaviour
{
    Image _curtainSprite;

    void Awake() {
        _curtainSprite = GetComponent<Image>();
        _curtainSprite.color = Color.black;
    }

    public void CurtainUp() {
        StartCoroutine(CurtainUpRoutine());
    }

    public void CurtainDown() {
        StartCoroutine(CurtainDownRoutine());
    }

    IEnumerator CurtainUpRoutine() {
        _curtainSprite.color = Color.black;
        _curtainSprite.gameObject.SetActive(true);
        float alpha = 1f;

        while (alpha > 0f) {
            alpha -= 0.01f;
            _curtainSprite.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        _curtainSprite.gameObject.SetActive(false);
    }

    IEnumerator CurtainDownRoutine() {
        _curtainSprite.color = Color.clear;
        _curtainSprite.gameObject.SetActive(true);
        float alpha = 0f;

        while (alpha < 1f) {
            alpha += 0.01f;
            _curtainSprite.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
