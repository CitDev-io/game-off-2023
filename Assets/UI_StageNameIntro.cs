using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_StageNameIntro : MonoBehaviour
{
    public Image BarImage;
    public TextMeshProUGUI StageNameTitle;

    public TextMeshProUGUI BossNameText;
    public Image BossImg;

    void Awake() {
        BarImage = GetComponent<Image>();
    }

    [ContextMenu("Introduce Stage")]
    public void IntroduceStage() {
        StartCoroutine(IntroduceStageRoutine());
    }

    IEnumerator IntroduceStageRoutine() {
        BossNameText.color = Color.white;
        BarImage.color = new Color(0f, 0f, 0f, 0.45f);
        BarImage.enabled = true;
        StageNameTitle.color = new Color(1f, 1f, 1f, 0f);
        StageNameTitle.gameObject.SetActive(true);
        
        float alpha = 0f;

        while (alpha < 1f) {
            alpha += 0.01f;
            StageNameTitle.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        BossImg.color = Color.white;
        BossNameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        alpha = 1f;

        while (alpha > 0f) {
            alpha -= 0.01f;
            if (alpha <= BarImage.color.a) {
                BarImage.color = new Color(0f, 0f, 0f, alpha);
            }
            StageNameTitle.color = new Color(1f, 1f, 1f, alpha);
            BossNameText.color = new Color(1f, 1f, 1f, alpha);
            BossImg.color = new Color(1f, 1f, 1f, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        BarImage.enabled = false;
        BossImg.color = Color.clear;
        BossNameText.gameObject.SetActive(false);
        StageNameTitle.gameObject.SetActive(false);
    }

    public void SetStageInfo(StageConfig stageConfig) {
        StageNameTitle.text = stageConfig.StageName;
        BossNameText.text = stageConfig.BossName;
        BossImg.sprite = stageConfig.BossImage;
    }
}
