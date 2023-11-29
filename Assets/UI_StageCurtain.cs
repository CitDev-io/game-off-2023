using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageCurtain : MonoBehaviour
{
    public Image _curtainSprite;
    Coroutine _runningRoutine;

    void Awake() {
        _curtainSprite.gameObject.SetActive(true);
        _curtainSprite.color = Color.black;
    }

    public void CurtainUp() {
        if (!_curtainSprite.gameObject.activeSelf) return;
        if (_runningRoutine != null) StopCoroutine(_runningRoutine);
        _runningRoutine = StartCoroutine(CurtainUpRoutine());
    }

    public void CurtainDown() {
        if (_curtainSprite.gameObject.activeSelf && (_curtainSprite.color.a == 1f || _runningRoutine != null)) return;
        if (_runningRoutine != null) StopCoroutine(_runningRoutine);
        _runningRoutine = StartCoroutine(CurtainDownRoutine());
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
