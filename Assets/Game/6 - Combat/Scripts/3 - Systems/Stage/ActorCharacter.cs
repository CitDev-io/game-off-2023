using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ActorCharacter : MonoBehaviour
{
    [Header("Temp Exposed Plumbing")]
    [SerializeField]
    public TMP_Text HealthTicker;
    [SerializeField]
    public TMP_Text NameTicker;
    [SerializeField]
    public TMP_Text StaggerTicker; 
    GameObject _displayLayer;
    SpriteRenderer _skin;
    SkeletonAnimation _spineSkin;
    bool IN_SPINE_MODE = false;
    bool IsActing = false;
    public bool IsPerforming = false;
    Queue<CharacterActorPerformance> _performanceQueue = new Queue<CharacterActorPerformance>();
    [Header("Polymorph")]
    public SkeletonDataAsset CritterSkin;
    public Sprite CritterSkinSprite;
    public SkeletonDataAsset CritterSkin2;
    public Sprite CritterSkinSprite2;
    
    [Header("Floating Prefabs")]
    public GameObject DamageTextPrefab;
    public GameObject BuffUpPrefab;
    public GameObject BuffDownPrefab;
    
    [Header("Static Decorators")]
    public GameObject FloatingBrokenIcon;
    public GameObject TurnIndicator;
    public Sprite HighlightIndicatorLight;
    public Sprite HighlightIndicatorShadow;
    public GameObject HighlightIndicator;

    [HideInInspector]
    public Character _character;

    void Awake() {
        _character = GetComponent<Character>();
        _displayLayer = transform.Find("DisplayLayer").gameObject;
        _skin = _displayLayer.transform.Find("Skin").GetComponent<SpriteRenderer>();
        _spineSkin = _displayLayer.transform.Find("SpineSkin").GetComponent<SkeletonAnimation>();
        FloatingBrokenIcon.SetActive(false);
        FloatingBrokenIcon.transform.position = OverHeadPosition() + new Vector3(0f, 0.5f, 0f);
    }

    void Start()
    {
        IN_SPINE_MODE = _character.Config.SpineSkeleton != null;
        // ElementIndicator.SetPowerType(_character.Config.PowerType);
        GetDressed();
        SpineEventSubscribe();
        StartCoroutine(BrilliantActing());
    }

    IEnumerator BrilliantActing() {
        while (true) {
            yield return null;

            IsPerforming = IsActing || _performanceQueue.Count > 0;

            if (!IsActing && _performanceQueue.Count > 0) {
                CharacterActorPerformance performance = _performanceQueue.Dequeue();

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
                    case CharacterActorPerformance.POLYMORPH:
                        StartCoroutine(PolymorphPerformance());
                        break;
                    case CharacterActorPerformance.UNPOLYMORPH:
                        StartCoroutine(UnpolymorphPerformance());
                        break;
                    case CharacterActorPerformance.FADEOUT:
                        StartCoroutine(FadeOutPerformance());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void FloatingDamageText(int dmg) {
        if (dmg < 0) {
            FloatingDamageText((-dmg).ToString(), Color.green);
        } else if (dmg == 0) {
            FloatingDamageText("miss", Color.white);    
        } else {
            FloatingDamageText((-dmg).ToString(), Color.red);
        }
    }

    Vector3 OverHeadPosition() {
        if (IN_SPINE_MODE) {
            var bone = _spineSkin.skeleton.FindBone("text");
            if (bone == null) {
                return transform.position;
            }
            return bone.GetWorldPosition(_spineSkin.transform);
        }
        return transform.position + new Vector3(0f, 2f, 0f);
    }

    void FloatingDamageText(string dmg, Color color) {
        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0f);
        GameObject floatingText = Instantiate(DamageTextPrefab, OverHeadPosition() + offset, Quaternion.identity);
        floatingText.GetComponent<UI_FloatingText>().Float(dmg, color);
    }

    public void FloatingBuffUp(Buff buff) {
        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0f);
        GameObject floatingBuff = Instantiate(BuffUpPrefab, OverHeadPosition() + offset, Quaternion.identity);
        floatingBuff.GetComponent<UI_FloatingBuffUp>().ShowBuff(buff);
    }

    public void FloatingBuffDown(Buff buff) {
        Vector3 offset = new Vector3(0f, 1.5f, 0f);
        GameObject floatingBuff = Instantiate(BuffDownPrefab, OverHeadPosition()+offset, Quaternion.identity);
        floatingBuff.GetComponent<UI_FloatingBuffDown>().ShowBuff(buff);
    }

    public void EnqueuePerformance(CharacterActorPerformance performance) {
        if (performance == CharacterActorPerformance.DIE && _performanceQueue.Contains(CharacterActorPerformance.DIE)) {
            return;
        }
        IsPerforming = true;
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

        if (e.Data.Name == "attack") {
            FindFirstObjectByType<GameController_DDOL>().PlaySound("Swing_" + Random.Range(1, 4).ToString());
        }
    }

    void ChangeSpineSkinRed() {
        Color flashColor = Color.red;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
    }

    void ChangeSpineSkinWhite() {
        Color flashColor = Color.white;
        _spineSkin.skeleton.R = flashColor.r;
        _spineSkin.skeleton.G = flashColor.g;
        _spineSkin.skeleton.B = flashColor.b;
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
            SetAnimation(0, ActorAnimations.idle, true);
        } else {
            _skin.gameObject.SetActive(true);
            _spineSkin.gameObject.SetActive(false);
            _skin.sprite = _character.Config.Skin;
        }
    }

    void OnMouseDown()
    {
        if (_character.isDead) return;

        GameObject.Find("GameManager").GetComponent<UIManager>().SelectTargetIfEligible(_character);
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

        if (_character.IsHighlighted) {
            HighlightIndicator.GetComponent<SpriteRenderer>().sprite = _character.Config.PowerType == PowerType.LIGHT ? HighlightIndicatorLight : HighlightIndicatorShadow;
            HighlightIndicator.SetActive(true);
        } else {
            HighlightIndicator.SetActive(false);
        }

        if (_character.Config.BaseSP > 0 && _character.currentStagger == 0 && !_character.isDead) {
            FloatingBrokenIcon.SetActive(true);
        } else {
            FloatingBrokenIcon.SetActive(false);
        }
    }

    bool HasAnimation(ActorAnimations animation) {
        if (_spineSkin.skeletonDataAsset.GetSkeletonData(true).FindAnimation(animation.ToString()) != null) {
            return true;
        }
        return false;
    }

    void SetAnimation(int trackIndex, ActorAnimations animation, bool loop) {
        if (IN_SPINE_MODE && HasAnimation(animation)) {
            _spineSkin.AnimationState.SetAnimation(trackIndex, animation.ToString(), loop);
        }
    }

    void AddAnimation(int trackIndex, ActorAnimations animation, bool loop, float delay) {
        if (IN_SPINE_MODE && HasAnimation(animation)) {
            _spineSkin.AnimationState.AddAnimation(trackIndex, animation.ToString(), loop, delay);
        }
    }

    IEnumerator TakeDamagePerformance() {
        if (IN_SPINE_MODE) {
            bool hasPerformance = false;
            if (HasAnimation(ActorAnimations.damage)) {
                hasPerformance = true;
            }
            if (hasPerformance) {
                IsActing = true;
                AddAnimation(0, ActorAnimations.damage, false, 0f);
                AddAnimation(0, ActorAnimations.idle, true, 0f);
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
            SetAnimation(0, ActorAnimations.attack, false);
            AddAnimation(0, ActorAnimations.idle, true, 0.55f);
            yield return new WaitForSeconds(0.75f);
            IsActing = false;
        } else {
            IsActing = true;
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
            IsActing = false;
        }
    }

    IEnumerator SpecialAbilityPerformance() {
        if (IN_SPINE_MODE) {
            IsActing = true;
            SetAnimation(0, ActorAnimations.special, false);
            AddAnimation(0, ActorAnimations.idle, true, 0.55f);
            yield return new WaitForSeconds(0.75f);
            IsActing = false;
        }
            yield return null;
    }

    float DEATH_FADE_SPEED = 0.08f;
    float DEATH_FADE_INCREMENT = 0.1f;
    IEnumerator DeathPerformance() {
        if (IN_SPINE_MODE) {
            bool hasDeathPerformance = false;
            if (HasAnimation(ActorAnimations.death)) {
                hasDeathPerformance = true;
            }
            IsActing = true;
            if (hasDeathPerformance) {
                AddAnimation(0, 
                ActorAnimations.death, false, 0.35f);
                yield return new WaitForSeconds(2f);
            } else {
                yield return new WaitForSeconds(0.75f);
            }
            if (_character.Config.TeamType != TeamType.PLAYER) {
                float alpha = 1f;
                while (alpha > 0f) {
                    _spineSkin.skeleton.A = alpha;
                    alpha -= DEATH_FADE_INCREMENT;
                    yield return new WaitForSeconds(DEATH_FADE_SPEED);
                }
                _displayLayer.SetActive(false);
                yield return new WaitForSeconds(1.0f);
            }
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

    IEnumerator FadeOutPerformance() {
        if (IN_SPINE_MODE) {
            IsActing = true;
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
        yield return null;
        // _skin.color = new Color(1f, 0f, 0f, 0.8f);
        // yield return new WaitForSeconds(0.1f);
        // _skin.color = new Color(1f, 1f, 1f, 1f);
        // yield return new WaitForSeconds(0.1f);
        // _skin.color = new Color(1f, 0f, 0f, 0.8f);
        // yield return new WaitForSeconds(0.1f);
        // _skin.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator RevivedPerformance() {
        _displayLayer.SetActive(true);
        if (IN_SPINE_MODE) {
            IsActing = true;
            if (_spineSkin.skeleton.A < 4f) {
                float alpha = 0f;
                while (alpha < 0.95f) {
                    _spineSkin.skeleton.A = alpha;
                    alpha += DEATH_FADE_INCREMENT;
                    yield return new WaitForSeconds(DEATH_FADE_SPEED);
                }
                _spineSkin.skeleton.A = 1f;
            }
            yield return new WaitForSeconds(0.75f);
            SetAnimation(0, ActorAnimations.idle, true);
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

    IEnumerator PolymorphPerformance() {
        if (IN_SPINE_MODE) {
            IsActing = true;

            // _spineSkin.skeletonDataAsset = _character.Config.SpineSkeleton;
            bool flipACoin = UnityEngine.Random.Range(0, 2) == 0;

            if (flipACoin) {
                _spineSkin.skeletonDataAsset = CritterSkin;
                _character.AlternativePortrait = CritterSkinSprite;
            } else {
                _spineSkin.skeletonDataAsset = CritterSkin2;
                _character.AlternativePortrait = CritterSkinSprite2;
            }
            _spineSkin.Initialize(true);
            SetAnimation(0, ActorAnimations.idle, true);
            yield return new WaitForSeconds(0.75f);
            IsActing = false;
        }
            yield return null;
    }

    IEnumerator UnpolymorphPerformance() {
        if (IN_SPINE_MODE) {
            IsActing = true;

            _spineSkin.skeletonDataAsset = _character.Config.SpineSkeleton;
            _character.AlternativePortrait = null;
            _spineSkin.Initialize(true);
            SetAnimation(0, ActorAnimations.idle, true);
            yield return new WaitForSeconds(0.75f);
            IsActing = false;
        }
            yield return null;
    }
}
