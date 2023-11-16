using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyConfig", menuName = "GameOff2023/New Party Configuration", order = 1)]
public class PartyConfig : ScriptableObject
{
    public string DeveloperDescription = "";
    public List<CharacterConfig> PartyMembers;
}
