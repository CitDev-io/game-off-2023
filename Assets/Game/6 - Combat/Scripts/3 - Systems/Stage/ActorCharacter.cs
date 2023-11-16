using System.Collections;
using UnityEngine;
using TMPro;

public class ActorCharacter : MonoBehaviour
{
    [Header("Temp Exposed Plumbing")]
    [SerializeField]
    public TMP_Text HealthTicker;
    [SerializeField]
    public TMP_Text NameTicker;
    [SerializeField]
    public TMP_Text StaggerTicker; 
    [SerializeField]
    public GameObject TurnIndicator;
    SpriteRenderer _skin;
    Character _character;
    void Awake() {
        _character = GetComponent<Character>();
        _skin = transform.Find("Skin").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _skin.sprite = _character.Config.Skin;        
    }

    void OnMouseDown()
    {
        if (_character.isDead) return;
        Debug.Log(gameObject.name + " was clicked!");
        GameObject.Find("GameManager").GetComponent<UIManager>().TargetSelected(_character);
    }

    void FixedUpdate()
    {
        if (HealthTicker != null) {
            HealthTicker.GetComponent<TMP_Text>().text = _character.currentHealth.ToString() + "/" + _character.Config.BaseHP.ToString();
        }
        if (NameTicker != null) {
            NameTicker.text = _character.Config.Name;
        }
        if (StaggerTicker != null) {
            StaggerTicker.text = _character.currentStagger.ToString() + "/" + _character.Config.BaseSP.ToString();
        }

        if (TurnIndicator != null) {
            if (_character.IsCurrentCombatant) {
                TurnIndicator.SetActive(true);
            } else {
                TurnIndicator.SetActive(false);
            }
        }
    }

    public void DoDeathPerformance() {
        StartCoroutine(DeathPerformance());
    }

    float DEATH_FADE_SPEED = 0.08f;
    float DEATH_FADE_INCREMENT = 0.1f;
    IEnumerator DeathPerformance() {
        float alpha = 1f;
        while (alpha > 0f) {
            _skin.color = new Color(1f, 1f, 1f, alpha);
            alpha -= DEATH_FADE_INCREMENT;
            yield return new WaitForSeconds(DEATH_FADE_SPEED);
        }
        gameObject.SetActive(false);
        _skin.color = Color.white;
    }

    public void DoCrackedPerformance() {
        StartCoroutine(CrackedPerformance());
    }

    IEnumerator CrackedPerformance() {
        _skin.color = new Color(1f, 0f, 0f, 0.8f);
        yield return new WaitForSeconds(0.1f);
        _skin.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.1f);
        _skin.color = new Color(1f, 0f, 0f, 0.8f);
        yield return new WaitForSeconds(0.1f);
        _skin.color = new Color(1f, 1f, 1f, 1f);
    }
}
