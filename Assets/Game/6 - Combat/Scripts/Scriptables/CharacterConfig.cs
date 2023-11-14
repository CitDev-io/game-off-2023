using System.Collections;
using System.Collections.Generic;
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



    [Header("Art")]
    [Tooltip("Portrait Sprite")]
    [SerializeField]
    private Sprite _Portrait;
    public Sprite Portrait => _Portrait;
    [Tooltip("Skin Sprite")]
    [SerializeField]
    private Sprite _Skin;
    public Sprite Skin => _Skin;


    [Header("Abilities")]
    [Tooltip("Special Attack")]
    [SerializeField]
    private AttackType _SpecialAttack;
    public AttackType SpecialAttack => _SpecialAttack;

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
}
