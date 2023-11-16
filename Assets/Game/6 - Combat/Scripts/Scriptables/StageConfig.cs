using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageConfig", menuName = "GameOff2023/New Stage+Waves Configuration")]
public class StageConfig : ScriptableObject
{
    public List<CharacterConfig> Wave1;
    public List<CharacterConfig> Wave2;
    public List<CharacterConfig> Wave3;
    public List<CharacterConfig> Wave4;
    public List<CharacterConfig> Wave5;
    public List<CharacterConfig> Wave6;
    public List<CharacterConfig> Wave7;
}
