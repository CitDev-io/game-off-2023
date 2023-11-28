using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoonAttackRankUp : BaseBoonResolver
{
    PCAdventureClassType EligibleType = PCAdventureClassType.NONE;
    int MAX_RANK = 3;

    public BoonAttackRankUp(
        PCAdventureClassType classType,
        string name,
        string description,
        string upgradeDescription,
        string level) {
        Name = name;
        Description = description;
        UpgradeType = "Enhanced Attack";
        UpgradeDescription = upgradeDescription;
        Level = level;
        Character = classType;

        EligibleType = classType;
    }

    public override void ApplyToEligible(List<CharacterConfig> characters) {
        foreach (var character in characters.Where(IsEligible)) {
            character.AttackTreeLevel += 1;
        }
    }

    public override void RemoveFromOwning(List<CharacterConfig> characters) {
        foreach (var character in characters.Where(IsEligible)) {
            character.AttackTreeLevel -= 1;
        }
    }

    public override bool IsEligible(CharacterConfig character) {
        return character.PlayerClass == EligibleType && character.AttackTreeLevel < MAX_RANK;
    }
}
