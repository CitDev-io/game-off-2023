using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class UI_BossTaunt : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI Name;
    public UI_TextCrawler TauntText;

    void Awake() {
        Stow();
    }

    public void Stow() {
        Portrait.gameObject.SetActive(false);
        Name.gameObject.SetActive(false);
        TauntText.ClearQueue();
        TauntText._CrawlText.gameObject.SetActive(false);
    }

    public void Taunt(Sprite bossPortrait, string bossName, string taunt) {
        StartCoroutine(DoTaunt(bossPortrait, bossName, taunt));
    }

    IEnumerator DoTaunt(Sprite bossPortrait, string bossName, string taunt) {
        Portrait.sprite = bossPortrait;
        Name.text = bossName;
        yield return new WaitForSeconds(0.5f);
        Portrait.gameObject.SetActive(true);
        Name.gameObject.SetActive(true);
        TauntText._CrawlText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        TauntText.EnqueueMessage(taunt);
    }
}
