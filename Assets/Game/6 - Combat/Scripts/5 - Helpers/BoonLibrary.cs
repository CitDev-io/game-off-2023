using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoonLibrary
{
    public List<BaseBoonResolver> Boons = new List<BaseBoonResolver>();
    public List<CharacterConfig> PartyMembers;

    public BoonLibrary(PartyConfig PlayerParty) {
        Boons.Add(new BoonAttackRankUp(PCAdventureClassType.PALADIN));
        Boons.Add(new BoonAttackRankUp(PCAdventureClassType.ROGUE));
        Boons.Add(new BoonAttackRankUp(PCAdventureClassType.WARLOCK));
        Boons.Add(new BoonAttackRankUp(PCAdventureClassType.PRIEST));

        Boons.Add(new BoonSupportRankUp(PCAdventureClassType.PALADIN));
        Boons.Add(new BoonSupportRankUp(PCAdventureClassType.ROGUE));
        Boons.Add(new BoonSupportRankUp(PCAdventureClassType.WARLOCK));
        Boons.Add(new BoonSupportRankUp(PCAdventureClassType.PRIEST));
    }

    List<BaseBoonResolver> GetBoonsForParty() {
        return Boons.Where(boon => boon.AvailableForAny(PartyMembers)).ToList();
    }

    public List<BaseBoonResolver> GetRandomBoonOptionsForParty(int count = 1) {
        List<BaseBoonResolver> eligibleBoons = GetBoonsForParty();

        List<BaseBoonResolver> randomBoons = new List<BaseBoonResolver>();

        for (int i = 0; i < count; i++) {
            if (eligibleBoons.Count == 0) {
                break;
            }

            int randomIndex = Random.Range(0, eligibleBoons.Count);
            randomBoons.Add(eligibleBoons[randomIndex]);
            eligibleBoons.RemoveAt(randomIndex);
        }

        return randomBoons;
    }
}
