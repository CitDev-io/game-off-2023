using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Collections.Generic;

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
    public UI_CharacterElementIndicator ElementIndicator;
    GameObject _displayLayer;
    SpriteRenderer _skin;
    SkeletonAnimation _spineSkin;
    bool IN_SPINE_MODE = false;
    public bool IsActing = false;
    Queue<CharacterActorPerformance> _performanceQueue = new Queue<CharacterActorPerformance>();

    Character _character;
    void Awake() {
        _character = GetComponent<Character>();
        _displayLayer = transform.Find("DisplayLayer").gameObject;
        _skin = _displayLayer.transform.Find("Skin").GetComponent<SpriteRenderer>();
        _spineSkin = _displayLayer.transform.Find("SpineSkin").GetComponent<SkeletonAnimation>();
    }

    void Start()
    {
        IN_SPINE_MODE = _character.Config.SpineSkeleton != null;
        ElementIndicator.SetPowerType(_character.Config.PowerType);
        GetDressed();
        SpineEventSubscribe();
        StartCoroutine(BrilliantActing());
    }

    IEnumerator BrilliantActing() {
        while (true) {
            yield return new WaitForSeconds(0.05f);

            if (!IsActing && _performanceQueue.Count > 0) {
                CharacterActorPerformance performance = _performanceQueue.Dequeue();
                Debug.Log("DEQUEUE " + performance.ToString() + " for " + gameObject.name);
                switch(performance) {
                    case CharacterActorPerformance.BASICATTACK:
                        StartCoroutine(BasicAttackPerformance());
                        break;
                    case CharacterActorPerformance.SPECIALATTACK:
                        StartCoroutine(SpecialAbilityPerformance());
                        break;
                    case CharacterActorPerformance.TAKEDAMAGE:
                        StartCoroutine(TakeDamagePerformance());
                        break;
                    case CharacterActorPerformance.DIE:
                        StartCoroutine(DeathPerformance());
                        break;
                    case CharacterActorPerformance.CRACKED:
                        StartCoroutine(CrackedPerformance());
                        break;
                    case CharacterActorPerformance.REVIVE:
                        StartCoroutine(RevivedPerformance());
                        break;
                }
            }
        }
    }



    public void EnqueuePerformance(CharacterActorPerformance performance) {
        Debug.Log("ENQUEUE " + performance.ToString() + " for " + gameObject.name);
        _performanceQueue.Enqueue(performance);
    }

    void SpineEventSubscribe() {
        if (IN_SPINE_MODE) {
            _spineSkin.AnimationState.Event += HandleSpineEvent;
        }
    }

    void HandleSpineEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
        if (e.Data.Name == "damage") {
            StartCoroutine(SpineFlashRedPerformance());
        }
    }

    void ChangeSpineSkinRed() {
        Color flashColor = Color.red;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
        _spineSkin.skeleton.A = flashColor.a;
    }

    void ChangeSpineSkinWhite() {
        Color flashColor = Color.white;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
        _spineSkin.skeleton.A = flashColor.a;
    }

    IEnumerator SpineFlashRedPerformance() {
        ChangeSpineSkinRed();
        yield return new WaitForSeconds(0.1f);
        ChangeSpineSkinWhite();
    }

    void GetDressed() {
        if (IN_SPINE_MODE) {
            _skin.gameObject.SetActive(false);
            _spineSkin.gameObject.SetActive(true);
            _spineSkin.initialSkinName = "default";
            _spineSkin.skeletonDataAsset = _character.Config.SpineSkeleton;
            _spineSkin.Initialize(true);
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.idle.ToString(), true);
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

    IEnumerator TakeDamagePerformance() {
        if (IN_SPINE_MODE) {
            bool hasPerformance = false;
            if (_spineSkin.skeletonDataAsset.GetSkeletonData(true).FindAnimation("damage") != null) {
                hasPerformance = true;
            }
            if (hasPerformance) {
                IsActing = true;
                _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.damage.ToString(), false, 0f);
                _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0f);
                yield return new WaitForSeconds(0.7f);
                IsActing = false;
            } else {
                IsActing = true;
                ChangeSpineSkinRed();
                yield return new WaitForSeconds(0.1f);
                ChangeSpineSkinWhite();
                IsActing = false;
            }
            yield return null;
        } else {
            yield return new WaitForSeconds(0.3f);
            _skin.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _skin.color = Color.white;
        }
    }

    IEnumerator BasicAttackPerformance() {
        if (IN_SPINE_MODE) {
            IsActing = true;
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.attack.ToString(), false);
            _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0.55f);
            yield return new WaitForSeconds(0.75f);
            IsActing = false;
        } else {
            Vector3 startingPosition = transform.position;
            Vector3 forwardPosition = transform.position;
            forwardPosition.x += 2f * (_character.Config.TeamType == TeamType.CPU ? 1 : -1);

            // move to forward position and back
            float moveSpeed = 20f;
            IsActing = true;
            while (transform.position != forwardPosition) {
                transform.position = Vector3.MoveTowards(transform.position, forwardPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            while (transform.position != startingPosition) {
                transform.position = Vector3.MoveTowards(transform.position, startingPosition, Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            IsActing = false;
        }
    }

    IEnumerator SpecialAbilityPerformance() {
        if (IN_SPINE_MODE) {
            bool hasPerformance = false;
            if (_spineSkin.skeletonDataAsset.GetSkeletonData(true).FindAnimation("special") != null) {
                hasPerformance = true;
            }
            if (hasPerformance) {
                IsActing = true;
                _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.special.ToString(), false);
                _spineSkin.AnimationState.AddAnimation(0, ActorAnimations.idle.ToString(), true, 0.55f);
                yield return new WaitForSeconds(0.75f);
                IsActing = false;
            }
        } else {
        }
            yield return null;
    }

    float DEATH_FADE_SPEED = 0.08f;
    float DEATH_FADE_INCREMENT = 0.1f;
    IEnumerator DeathPerformance() {
        if (IN_SPINE_MODE) {
            bool hasDeathPerformance = false;
            if (_spineSkin.skeletonDataAsset.GetSkeletonData(true).FindAnimation("death") != null) {
                hasDeathPerformance = true;
            }
            IsActing = true;
            if (hasDeathPerformance) {
                _spineSkin.AnimationState.AddAnimation(0, 
                ActorAnimations.death.ToString(), false, 0.35f);
                yield return new WaitForSeconds(2f);
            } else {
                yield return new WaitForSeconds(0.75f);
            }
            float alpha = 1f;
            while (alpha > 0f) {
                _spineSkin.skeleton.A = alpha;
                alpha -= DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _displayLayer.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            IsActing = false;
        } else {
            yield return new WaitForSeconds(0.75f);
            IsActing = true;
            float alpha = 1f;
            while (alpha > 0f) {
                _skin.color = new Color(1f, 1f, 1f, alpha);
                alpha -= DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _displayLayer.SetActive(false);
            _skin.color = Color.white;
            IsActing = false;
        }
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

    IEnumerator RevivedPerformance() {
        _displayLayer.SetActive(true);
        if (IN_SPINE_MODE) {
            IsActing = true;
            float alpha = 0f;
            while (alpha < 0.95f) {
                _spineSkin.skeleton.A = alpha;
                alpha += DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _spineSkin.skeleton.A = 1f;
            yield return new WaitForSeconds(0.75f);
            _spineSkin.AnimationState.SetAnimation(0, ActorAnimations.idle.ToString(), true);
            IsActing = false;
        } else {
            IsActing = true;
            float alpha = _skin.color.a;
            while (alpha < 0.95f) {
                _skin.color = new Color(1f, 1f, 1f, alpha);
                alpha += DEATH_FADE_INCREMENT;
                yield return new WaitForSeconds(DEATH_FADE_SPEED);
            }
            _skin.color = Color.white;
            IsActing = false;
        }
    }
}
