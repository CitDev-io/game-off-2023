using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoonAttSupDualRankUp : BaseBoonResolver
{
    PCAdventureClassType EligibleType = PCAdventureClassType.NONE;

    int MAX_RANK = 3;

    public BoonAttSupDualRankUp(PCAdventureClassType classType) {
        Name = classType.ToString() + " Dual Support/Attack Tier";
        Description = "Upgrade " + classType.ToString() + " Special Attack (Support & Attack)";
        PortraitArt = null;
        EligibleType = classType;
    }

    public override void ApplyToEligible(List<CharacterConfig> characters) {
        foreach (var character in characters.Where(IsEligible)) {
            character.SupportTreeLevel += 1;
            character.AttackTreeLevel += 1;
        }
    }

    public override void RemoveFromOwning(List<CharacterConfig> characters) {
        foreach (var character in characters.Where(IsEligible)) {
            character.SupportTreeLevel -= 1;
            character.AttackTreeLevel -= 1;
        }
    }

    public override bool IsEligible(CharacterConfig character) {
        return character.PlayerClass == EligibleType && character.SupportTreeLevel < MAX_RANK && character.AttackTreeLevel < MAX_RANK;
    }
}
