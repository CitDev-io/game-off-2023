using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySetList", menuName = "GameOff2023/New Enemy Set List")]
public class EnemySetList : ScriptableObject
{
    public List<StageConfig> GameStages;
}
