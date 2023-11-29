using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FloatingBuffUp : MonoBehaviour
{
    public Image BuffImage;
    public void ShowBuff(Buff buff) {
        BuffImage.sprite = Resources.Load<Sprite>(buff.PortraitArt);
        BuffImage.color = buff.isDebuff ? Color.red : Color.white;
        StartCoroutine(ShowBuffRoutine());
    }

    IEnumerator ShowBuffRoutine() {
        BuffImage.gameObject.SetActive(true);
        float alpha = 1f;

        while (alpha > 0f) {
            alpha -= 0.01f;
            BuffImage.color = new Color(BuffImage.color.r, BuffImage.color.g, BuffImage.color.b, alpha);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.015f, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
