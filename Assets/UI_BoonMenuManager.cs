using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BoonMenuManager : MonoBehaviour
{
    [Header("Temp Plumbing")]
    public GameObject SwooshModal;
    public GameObject PanelUI;

    public Image Icon;
    public Image Spinner;
    public UI_BoonOptionMgr Option1;
    public UI_BoonOptionMgr Option2;
    public UI_BoonOptionMgr Option3;

    public TextMeshProUGUI Name1;
    public TextMeshProUGUI Name2;
    public TextMeshProUGUI Name3;

    List<BaseBoonResolver> _boons;
    public BaseBoonResolver CurrentSelection;

    public void ToggleLeft() {
        if (_boons.Count == 0) return;

        int currentIndex = _boons.IndexOf(CurrentSelection);
        int newIndex = (currentIndex - 1 + _boons.Count) % _boons.Count;
        ToggleToBoonByIndex(newIndex);
    }

    public void ToggleRight() {
        if (_boons.Count == 0) return;

        int currentIndex = _boons.IndexOf(CurrentSelection);
        int newIndex = (currentIndex + 1) % _boons.Count;
        ToggleToBoonByIndex(newIndex);
    }

    void Awake() {
        SwooshModal.SetActive(false);
        PanelUI.SetActive(false);
        Icon.color = new Color(1f, 1f, 1f, 0f);
        Spinner.color = new Color(1f, 1f, 1f, 0f);
        Icon.gameObject.SetActive(false);
    }

    public void OfferBoons(List<BaseBoonResolver> boons) {
        SetBoonOffers(boons);
        StartCoroutine(DoAppear(0.55f, 179.9f));
        StartCoroutine(IconAppear());
    }

    void SetBoonOffers(List<BaseBoonResolver> boons) {
        if (boons.Count > 0) {
            Option1.SetContent(boons[0]);
            CurrentSelection = boons[0];
        }
        if (boons.Count > 1) {
            Option2.SetContent(boons[1]);
        }
        if (boons.Count > 2) {
            Option3.SetContent(boons[2]);
        }
        Option1.gameObject.SetActive(boons.Count > 0);
        Option2.gameObject.SetActive(boons.Count > 1);
        Option3.gameObject.SetActive(boons.Count > 2);

        Option1.SetSelected(true);
        Option2.SetSelected(false);
        Option3.SetSelected(false);
        _boons = boons;
    }

    public void ToggleToBoonByIndex(int index) {
        CurrentSelection = _boons[index];
        Option1.SetSelected(index == 0);
        Option2.SetSelected(index == 1);
        Option3.SetSelected(index == 2);
    }

    public void Dismiss() {
        CurrentSelection = null;
        DoDisappearPerformance();
    }

    public void UserSelectBoon() {
        GameObject.Find("GameManager").GetComponent<UIManager>().BoonSelected(CurrentSelection);
        CurrentSelection = null;
        DoDisappearPerformance();
    }

    void DoDisappearPerformance() {
        StartCoroutine(DoAppear(0f, 0f));
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

        Debug.Log(Mathf.Abs(SwooshModal.transform.rotation.eulerAngles.z - TargetRotation) );
        while (Mathf.Abs(SwooshModal.transform.rotation.eulerAngles.z - TargetRotation) > 0.5f) {
            SwooshModal.transform.rotation = Quaternion.Lerp(SwooshModal.transform.rotation, Quaternion.Euler(0f, 0f, TargetRotation+0.1f), SPINSPEED);

            PanelUI.SetActive(SwooshModal.transform.rotation.eulerAngles.z < -120f || SwooshModal.transform.rotation.eulerAngles.z > 120);

            yield return new WaitForSeconds(WAITSPEED);
        }
        SwooshModal.transform.rotation = Quaternion.Euler(0f, 0f, TargetRotation);
        if (TargetRotation == 0f) {
            SwooshModal.SetActive(false);
        }
    }
}
