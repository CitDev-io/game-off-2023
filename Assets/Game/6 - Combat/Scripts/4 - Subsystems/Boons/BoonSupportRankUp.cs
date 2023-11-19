using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoonSupportRankUp : BaseBoonResolver
{
    PCAdventureClassType EligibleType = PCAdventureClassType.NONE;

    int MAX_RANK = 3;

    public BoonSupportRankUp(PCAdventureClassType classType) {
        Name = classType.ToString() + " Support Tier";
        Description = "Upgrade " + classType.ToString() + " Special Attack (Support)";
        PortraitArt = null;
        EligibleType = classType;
    }

    public override void ApplyToEligible(List<CharacterConfig> characters) {
        foreach (var characterConfig in characters.Where(IsEligible)) {
            characterConfig.SupportTreeLevel += 1;
        }
    }

    public override void RemoveFromOwning(List<CharacterConfig> characters) {
        foreach (var characterConfig in characters.Where(IsEligible)) {
            characterConfig.SupportTreeLevel -= 1;
        }
    }

    public override bool IsEligible(CharacterConfig character) {
        return character.PlayerClass == EligibleType && character.SupportTreeLevel < MAX_RANK;
    }
}
