using UnityEngine;

public class UI_GameOverPanel : MonoBehaviour
{
    public GameObject Heart1;
    public GameObject Heart2;
    public GameObject Heart3;

    public GameObject RetryPanel;
    public GameObject TFPPanel;

    public void SetRetries(int retries) {
        if (retries == 0) {
            RetryPanel.SetActive(false);
            TFPPanel.SetActive(true);
        } else {
            Heart1.SetActive(false);
            Heart2.SetActive(false);
            Heart3.SetActive(false);
            RetryPanel.SetActive(true);
            TFPPanel.SetActive(false);
            if (retries > 0) {
                Heart1.SetActive(true);
            }
            if (retries > 1) {
                Heart2.SetActive(true);
            }
            if (retries > 2) {
                Heart3.SetActive(true);
            }
        }
    }
}
