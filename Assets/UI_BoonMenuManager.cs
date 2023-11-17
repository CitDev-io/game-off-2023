using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoonMenuManager : MonoBehaviour
{
    [Header("Temp Plumbing")]
    public GameObject SwooshModal;
    public GameObject PanelUI;

    public Image Icon;
    public Image Spinner;

    void Awake() {
        SwooshModal.SetActive(false);
        PanelUI.SetActive(false);
        Icon.color = new Color(1f, 1f, 1f, 0f);
        Spinner.color = new Color(1f, 1f, 1f, 0f);
        Icon.gameObject.SetActive(false);
    }

    [ContextMenu("DoAppearPerformance")]
    public void DoAppearPerformance() {
        StartCoroutine(DoAppear(0.55f, 0f));
        StartCoroutine(IconAppear());
    }

    [ContextMenu("DoDisappearPerformance")]
    public void DoDisappearPerformance() {
        StartCoroutine(DoAppear(0f, 179.9f));
        StartCoroutine(IconDisappear());
    }

    IEnumerator IconDisappear() {
        // fade out icon
        while (Icon.color.a > 0f) {
            Icon.color = new Color(1f, 1f, 1f, Icon.color.a - 0.15f);
            yield return new WaitForSeconds(0.05f);
            Icon.transform.localScale = new Vector3(Icon.color.a, Icon.color.a, Icon.color.a);
        }
        // hide Spinner
        Spinner.color = new Color(1f, 1f, 1f, 0f);
        Icon.transform.localScale = Vector3.zero;
        Icon.gameObject.SetActive(false);
    }

    IEnumerator IconAppear() {
        // set scale to 0
        Icon.gameObject.SetActive(true);
        Icon.transform.localScale = new Vector3(0f, 0f, 0f);
        // fade in and size up Icon
        while (Icon.color.a < 1f) {
            Icon.color = new Color(1f, 1f, 1f, Icon.color.a + 0.15f);
            Icon.transform.localScale = new Vector3(Icon.color.a, Icon.color.a, Icon.color.a);
            yield return new WaitForSeconds(0.05f);
        }
        // set to full scale
        Icon.transform.localScale = new Vector3(1f, 1f, 1f);
        Spinner.color = new Color(1f, 1f, 1f, 1f);
    }

    public float SPINSPEED = 0.145f;
    public float WAITSPEED = 0.05f;
    IEnumerator DoAppear(float initialDelay, float TargetRotation) {
        SwooshModal.SetActive(true);
        yield return new WaitForSeconds(initialDelay);

        while (Mathf.Abs(SwooshModal.transform.rotation.eulerAngles.z - TargetRotation) > 0.5f) {
            SwooshModal.transform.rotation = Quaternion.RotateTowards(SwooshModal.transform.rotation, Quaternion.Euler(0f, 0f, TargetRotation), SPINSPEED);

            PanelUI.SetActive(SwooshModal.transform.rotation.eulerAngles.z < 40f);
            yield return new WaitForSeconds(WAITSPEED);
        }
        SwooshModal.transform.rotation = Quaternion.Euler(0f, 0f, TargetRotation);
        if (TargetRotation == 179.9f) {
            SwooshModal.SetActive(false);
        }
    }
}
