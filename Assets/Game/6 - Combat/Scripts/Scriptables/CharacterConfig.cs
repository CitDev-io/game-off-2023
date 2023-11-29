using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "GameOff2023/New Character Configuration")]
public class CharacterConfig : ScriptableObject
{
    [Header("Character Info")]
    [Tooltip("Name of the character")]
    [SerializeField]
    private string _Name;
    public string Name => _Name;
    
    [Tooltip("Elemental affinity of the character")]
    [SerializeField]
    private PowerType _PowerType;
    public PowerType PowerType => _PowerType;

    [Tooltip("Team the character is on")]
    [SerializeField]
    private TeamType _TeamType;
    public TeamType TeamType => _TeamType;
    [SerializeField]
    [Tooltip("Select NONE for Enemies")]
    private PCAdventureClassType _PlayerClass;
    public PCAdventureClassType PlayerClass => _PlayerClass;
    public int ScaleBounty = 1;

    [Header("Art")]
    [Tooltip("Portrait Sprite")]
    [SerializeField]
    private Sprite _Portrait;
    public Sprite Portrait => _Portrait;
    [Tooltip("Skin Sprite")]
    [SerializeField]
    private Sprite _Skin;
    public Sprite Skin => _Skin;
    [Tooltip("Animated Spine Asset")]
    [SerializeField]
    private SkeletonDataAsset _SpineSkeleton;
    public SkeletonDataAsset SpineSkeleton => _SpineSkeleton;


    [Header("Abilities")]
    [Tooltip("Special Attack")]
    [SerializeField]
    private UserAbilitySelection _SpecialAttack;
    public UserAbilitySelection SpecialAttack => _SpecialAttack;
    
    [Tooltip("Ultimate Ability")]
    [SerializeField]
    private UserAbilitySelection _UltimateAbility;
    public UserAbilitySelection UltimateAbility => _UltimateAbility;
    [Tooltip("Native Buff")]
    [SerializeField]
    private NativeBuffOption _NativeBuff;
    public NativeBuffOption NativeBuff => _NativeBuff;
    

    

    [Header("Starting Stats")]
    [Tooltip("Whole number percentage of base damage reduction")]
    [SerializeField]
    private int _BaseMitigation = 0;
    public int BaseMitigation => _BaseMitigation;

    [Tooltip("Starting Hit Point Maximum")]
    [SerializeField]
    private int _BaseHP = 0;
    public int BaseHP => _BaseHP;

    [Tooltip("Starting Stagger Point Maximum")]
    [SerializeField]
    private int _BaseSP = 0;
    public int BaseSP => _BaseSP;

    [Tooltip("Base Attack Minimum Damage")]
    [SerializeField]
    private int _BaseAttackMin = 0;
    public int BaseAttackMin => _BaseAttackMin;

    [Tooltip("Base Attack Maximum Damage")]
    [SerializeField]
    private int _BaseAttackMax = 0;
    public int BaseAttackMax => _BaseAttackMax;

    [Tooltip("Base Special Minimum Damage")]
    [SerializeField]
    private int _BaseSpecialMin = 0;
    public int BaseSpecialMin => _BaseSpecialMin;

    [Tooltip("Base Special Maximum Damage")]
    [SerializeField]
    private int _BaseSpecialMax = 0;
    public int BaseSpecialMax => _BaseSpecialMax;


    [Header("Starting Boon Values")]
    [HideInInspector]
    public int SupportTreeLevel = 0;
    [HideInInspector]
    public int AttackTreeLevel = 0;

    [Header("Boss Settings")]
    public bool IsBoss = false;
    public string WaveIntroTaunt = "";
    public string WaveDefeatTaunt = "";
}
