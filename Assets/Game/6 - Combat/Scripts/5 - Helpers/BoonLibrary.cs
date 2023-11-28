using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoonLibrary
{
    public List<BaseBoonResolver> Boons = new List<BaseBoonResolver>();
    public List<CharacterConfig> PartyMembers;

    public BoonLibrary(PartyConfig PlayerParty) {
        Effect PaladinSpecial = new AbilityShieldBash();

        Boons.Add(new BoonAttackRankUp(
            PCAdventureClassType.PALADIN,
            PaladinSpecial.Name,
            PaladinSpecial.Description,
            "",
            ""
        ));

        Effect RogueSpecial = new AbilitySneakAttack();
        Boons.Add(new BoonAttackRankUp(
            PCAdventureClassType.ROGUE,
            RogueSpecial.Name,
            RogueSpecial.Description,
            "",
            ""
        ));

        Effect WarlockSpecial = new AbilityCurseOfStrength();
        Boons.Add(new BoonAttackRankUp(
            PCAdventureClassType.WARLOCK,
            WarlockSpecial.Name,
            WarlockSpecial.Description,
            "",
            ""
        ));

        Effect PriestSpecial = new AbilityBlessing();
        Boons.Add(new BoonAttackRankUp(
            PCAdventureClassType.PRIEST,
            PriestSpecial.Name,
            PriestSpecial.Description,
            "",
            ""
        ));

        Boons.Add(new BoonSupportRankUp(
            PCAdventureClassType.PALADIN,
            PaladinSpecial.Name,
            PaladinSpecial.Description,
            "",
            ""
        ));

        Boons.Add(new BoonSupportRankUp(
            PCAdventureClassType.ROGUE,
            RogueSpecial.Name,
            RogueSpecial.Description,
            "",
            ""
        ));

        Boons.Add(new BoonSupportRankUp(
            PCAdventureClassType.WARLOCK,
            WarlockSpecial.Name,
            WarlockSpecial.Description,
            "",
            ""
        ));

        Boons.Add(new BoonSupportRankUp(
            PCAdventureClassType.PRIEST,
            PriestSpecial.Name,
            PriestSpecial.Description,
            "",
            ""
        ));

        PartyMembers = PlayerParty.PartyMembers;
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

            BaseBoonResolver randomBoon = eligibleBoons[randomIndex];

            WordifyBoon(randomBoon);

            randomBoons.Add(eligibleBoons[randomIndex]);
            eligibleBoons.RemoveAt(randomIndex);
        }

        return randomBoons;
    }

    void WordifyBoon(BaseBoonResolver thisBoon) {
        CharacterConfig matchingMember = PartyMembers.FirstOrDefault(p => p.PlayerClass == thisBoon.Character);
        
        if (matchingMember == null) {
            return;
        }

        if (thisBoon.UpgradeType == "Enhanced Attack") {
            int level = matchingMember.AttackTreeLevel + 1;
            thisBoon.Level = "Level " + level;

            thisBoon.UpgradeDescription = GetAttackDescriptionByClassLevel(thisBoon.Character, level);
        } else if (thisBoon.UpgradeType == "Enhanced Support") {
            int level = matchingMember.SupportTreeLevel + 1;
            thisBoon.Level = "Level " + level;

            thisBoon.UpgradeDescription = GetSupportDescriptionByClassLevel(thisBoon.Character, level);
        }
    }

    public static string GetAttackDescriptionByClassLevel(PCAdventureClassType classType, int level) {
        switch(classType) {
            case PCAdventureClassType.PALADIN:
                switch(level) {
                    case 1:
                        return "Add <b>PARTIAL DAMAGE</b> to enemies adjacent to target";
                    case 2:
                        return "<b>INCREASE DAMAGE</b> to target & adjacent enemies";
                    case 3:
                        return "<b>CHANCE</b> to <b>STUN</b> target & adjacent enemies";
                }
                break;
            case PCAdventureClassType.ROGUE:
                switch(level) {
                    case 1:
                        return "Add <b>PARTIAL DAMAGE</b> to random enemy";
                    case 2:
                        return "Add <b>PARTIAL DAMAGE</b> to 2 random enemies";
                    case 3:
                        return "Increase <b>CRITICAL STRIKE CHANCE</b>";
                }
                break;
            case PCAdventureClassType.PRIEST:
                switch(level) {
                    case 1:
                        return "Blessing can deal <b>DAMAGE</b> to target enemy";
                    case 2:
                        return "Add <b>PARTIAL HEAL</b> to 1 random ally";
                    case 3:
                        return "Target enemy takes <b>INCREASED AFFINITY DAMAGE</b> for 2 rounds";
                }
                break;
            case PCAdventureClassType.WARLOCK:
                switch(level) {
                    case 1:
                        return "Add <b>DAMAGE</b> to target enemy";
                    case 2:
                        return "Add <b>CHANCE</b> to <b>CHARM</b> target enemy for 1 round";
                    case 3:
                        return "Charm is upgraded to <b>GREATER CHARM</b>";
                }
            break;
        }
        return "";
    }

    public static string GetSupportDescriptionByClassLevel(PCAdventureClassType classType, int level) {
        switch(classType) {
            case PCAdventureClassType.PALADIN:
                switch(level) {
                    case 1:
                        return "Add <b>SHIELD</b> to random hero for 2 rounds";
                    case 2:
                        return "<b>TAUNT</b> target enemy for 1 round";
                    case 3:
                        return "Add <b>SHIELD</b> to all heroes";
                }
                break;
            case PCAdventureClassType.ROGUE:
                switch(level) {
                    case 1:
                        return "Add <b>BLIND</b> to target enemy for 1 round";
                    case 2:
                        return "<b>SMOKE BOMB</b> target enemy for 2 rounds";
                    case 3:
                        return "<b>CHANCE</b> to SILENCE</b> target enemy for 2 rounds";
                }
                break;
            case PCAdventureClassType.PRIEST:
                switch(level) {
                    case 1:
                        return "<b>REMOVE DEBUFF</b> from target hero";
                    case 2:
                        return "Increase <b>HEAL</b> amount";
                    case 3:
                        return "Automatically <b>REVIVE</b> 1 downed hero 1x per wave";
                }
                break;
            case PCAdventureClassType.WARLOCK:
                switch(level) {
                    case 1:
                        return "<b>CHANCE to add <b>AFFINITY WEAKNESS</b> for 1 round";
                    case 2:
                        return "Strengthened hero <b>STEALS HEALTH</b> from random hero and triples it";
                    case 3:
                        return "Strengthened hero <b>ATTACKS TWICE</b> for 1 round";
                }
            break;
        }
        return "";
    }
}
