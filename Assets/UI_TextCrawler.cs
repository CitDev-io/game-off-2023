using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TextCrawler : MonoBehaviour
{
    public TextMeshProUGUI _CrawlText;
    Queue<string> MessageQueue = new Queue<string>();
    public float CRAWL_SPEED = 0.05f;
    public float DISPLAY_DURATION = 1.25f;
    public float CHECK_QUEUE_TIMEOUT = 0.25f;
    string SpellItOut = "";
    Coroutine _runningCoroutine;

    void Start() {
        StartCoroutine(Crawl());
    }

    public void ClearQueue() {
        MessageQueue.Clear();
        _CrawlText.text = "";
    }

    IEnumerator Crawl() {
        while (true) {
            if (MessageQueue.Count > 0) {
                string targetMessage = MessageQueue.Dequeue();
                SpellItOut = "";
                if (targetMessage.Length > 0) {
                    int index = 0;
                    foreach(char letter in targetMessage) {
                        SpellItOut += letter;
                        _CrawlText.text = SpellItOut;
                        if (index % 3 == 0) {
                            yield return new WaitForSeconds(CRAWL_SPEED);
                        }
                        index++;
                    }
                }
                _CrawlText.text = targetMessage;
                yield return new WaitForSeconds(DISPLAY_DURATION);
            }
            yield return new WaitForSeconds(CHECK_QUEUE_TIMEOUT);
        }
    }
    public void EnqueueMessage(string message) {
        if (MessageQueue.Contains(message)) {
            return;
        }
        MessageQueue.Enqueue(message);
    }
}
