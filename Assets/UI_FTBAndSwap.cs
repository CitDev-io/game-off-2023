using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FTBAndSwap : MonoBehaviour
{
    public GameObject Button;
    public ChangeScene changer;
    public Image _curtain;
    bool going = false;
    public void Go() {
        if (going) return;
        going = true;
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack() {
        _curtain.gameObject.SetActive(true);
        Button.SetActive(false);
        float alpha = 0f;
        while (alpha < 1f) {
            alpha += 0.01f;
            _curtain.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1.7f);
        GameObject.FindFirstObjectByType<GameController_DDOL>().FadeToStop();
        yield return new WaitForSeconds(1f);
        changer.SwapToScene("CombatScene");
    }
}
