using UnityEngine;

public class UI_DialogueBubble : MonoBehaviour
{
    public UI_TextCrawler TextCrawler;
    public GameObject Bubble;

    public void Say(string words) {
        Bubble.SetActive(true);
        TextCrawler._CrawlText.gameObject.SetActive(true);
        TextCrawler.EnqueueMessage(words);
    }

    public void Hide() {
        Bubble.SetActive(false);
        TextCrawler._CrawlText.gameObject.SetActive(false);
    }
}
