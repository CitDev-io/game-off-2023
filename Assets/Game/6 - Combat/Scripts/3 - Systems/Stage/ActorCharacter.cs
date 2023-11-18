using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;

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
    SkeletonAnimation _spineSkin;
    bool IN_SPINE_MODE = false;

    Character _character;
    void Awake() {
        _character = GetComponent<Character>();
        _skin = transform.Find("Skin").GetComponent<SpriteRenderer>();
        _spineSkin = transform.Find("SpineSkin").GetComponent<SkeletonAnimation>();
    }

    void Start()
    {
        IN_SPINE_MODE = _character.Config.SpineSkeleton != null;
        GetDressed();
        SpineEventSubscribe();
    }

    void SpineEventSubscribe() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.Event += HandleSpineEvent;
        }
    }

    void HandleSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
        if (e.Data.Name == "damage") {
            Debug.Log("OOF!");
            StartCoroutine(SpineRedFlash());
        }
    }

    IEnumerator SpineRedFlash() {
        Color flashColor = Color.red;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
        _spineSkin.skeleton.A = flashColor.a;
        yield return new WaitForSeconds(0.1f);
        flashColor = Color.white;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
        _spineSkin.skeleton.A = flashColor.a;
    }

    void GetDressed() {
        if (IN_SPINE_MODE) {
            _skin.gameObject.SetActive(false);
            _spineSkin.gameObject.SetActive(true);
            _spineSkin.skeletonDataAsset = _character.Config.SpineSkeleton;
        } else {
            _skin.gameObject.SetActive(true);
            _spineSkin.gameObject.SetActive(false);
            _skin.sprite = _character.Config.Skin;
        }
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

    public void DoDamageTakenPerformance() {
        StartCoroutine(TakeDamagePerformance());
    }

    IEnumerator TakeDamagePerformance() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.damage.ToString(), false, 0f);
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0f);
            yield return null;
        } else {
            yield return new WaitForSeconds(0.3f);
            _skin.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _skin.color = Color.white;
        }
    }

    public void DoBasicAttackPerformance() {
        StartCoroutine(BasicAttackPerformance());
    }

    public void DoSpecialAbilityPerformance() {
        StartCoroutine(SpecialAbilityPerformance());
    }

    IEnumerator BasicAttackPerformance() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.attack.ToString(), false);
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0.55f);
        } else {
            Vector3 startingPosition = transform.position;
            Vector3 forwardPosition = transform.position;
            forwardPosition.x += 2f * (_character.Config.TeamType == TeamType.CPU ? 1 : -1);

            // move to forward position and back
            float moveSpeed = 20f;
            while (transform.position != forwardPosition) {
                transform.position = Vector3.MoveTowards(transform.position, forwardPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            while (transform.position != startingPosition) {
                transform.position = Vector3.MoveTowards(transform.position, startingPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator SpecialAbilityPerformance() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.special.ToString(), false);
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0.55f);
        } else {
            Vector3 startingPosition = transform.position;
            Vector3 forwardPosition = transform.position;
            forwardPosition.x += 2f * (_character.Config.TeamType == TeamType.CPU ? 1 : -1);

            // move to forward position and back
            float moveSpeed = 20f;
            while (transform.position != forwardPosition) {
                transform.position = Vector3.MoveTowards(transform.position, forwardPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            while (transform.position != startingPosition) {
                transform.position = Vector3.MoveTowards(transform.position, startingPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void DoDeathPerformance() {
        StartCoroutine(DeathPerformance());
    }

    float DEATH_FADE_SPEED = 0.08f;
    float DEATH_FADE_INCREMENT = 0.1f;
    IEnumerator DeathPerformance() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.death.ToString(), false, 0.35f);
            yield return new WaitForSeconds(2f);
            float alpha = 1f;
            while (alpha > 0f) {
                _spineSkin.skeleton.A = alpha;
                alpha -= DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            gameObject.SetActive(false);
        } else {
            yield return new WaitForSeconds(0.75f);
            float alpha = 1f;
            while (alpha > 0f) {
                _skin.color = new Color(1f, 1f, 1f, alpha);
                alpha -= DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            gameObject.SetActive(false);
            _skin.color = Color.white;
        }
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

    public void DoRevivedPerformance() {
        gameObject.SetActive(true);
        StartCoroutine(RevivedPerformance());
    }

    IEnumerator RevivedPerformance() {
        if (IN_SPINE_MODE) {
            float alpha = 0f;
            while (alpha < 0.95f) {
                _spineSkin.skeleton.A = alpha;
                alpha += DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _spineSkin.skeleton.A = 1f;
            yield return new WaitForSeconds(0.75f);
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.idle.ToString(), true);
        } else {
            float alpha = _skin.color.a;
            while (alpha < 0.95f) {
                _skin.color = new Color(1f, 1f, 1f, alpha);
                alpha += DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _skin.color = Color.white;
        }
    }
}
