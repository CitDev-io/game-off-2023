using System.Collections.Generic;
using System.Linq;

public static class CombatantListFilter
{
    public static Character RandomByScope(List<Character> SearchSpace, Character referenceCharacter, EligibleTargetScopeType type) {
        List<Character> matches = ByScope(SearchSpace, referenceCharacter, type);
        if (matches.Count == 0) return null;
    
        return matches[UnityEngine.Random.Range(0, matches.Count)];
    }

    public static List<Character> ByScope(List<Character> SearchSpace, Character referenceCharacter, EligibleTargetScopeType type) {

        TeamType referenceTeam = referenceCharacter.Config.TeamType;

        if (referenceCharacter.HasBuff<BuffCharmed>()) {
            referenceTeam = referenceTeam == TeamType.PLAYER ? TeamType.CPU : TeamType.PLAYER;
        }

        bool AllowDead = false;
        bool AllowAlive = true;
        bool AllowSelf = false;
        bool AllowEnemy = true;
        bool AllowFriendly = false;

        switch(type) {
            case EligibleTargetScopeType.ANYALIVE:
                AllowDead = false;
                AllowAlive = true;
                AllowSelf = true;
                AllowEnemy = true;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.ANYDEAD:
                AllowDead = true;
                AllowAlive = false;
                AllowSelf = true;
                AllowEnemy = true;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.ENEMY:
                AllowDead = false;
                AllowAlive = true;
                AllowSelf = false;
                AllowEnemy = true;
                AllowFriendly = false;
                break;
            case EligibleTargetScopeType.FRIENDLYORSELF:
                AllowDead = false;
                AllowAlive = true;
                AllowSelf = true;
                AllowEnemy = false;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.ANYOTHERALLY:
                AllowDead = false;
                AllowAlive = true;
                AllowSelf = false;
                AllowEnemy = false;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.DEADENEMY:
                AllowDead = true;
                AllowAlive = false;
                AllowSelf = false;
                AllowEnemy = true;
                AllowFriendly = false;
                break;
            case EligibleTargetScopeType.DEADFRIENDLY:
                AllowDead = true;
                AllowAlive = false;
                AllowSelf = true;
                AllowEnemy = false;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.ANYATALL:
                AllowDead = true;
                AllowAlive = true;
                AllowSelf = true;
                AllowEnemy = true;
                AllowFriendly = true;
                break;
            case EligibleTargetScopeType.NONE:
            default:
                AllowDead = false;
                AllowAlive = false;
                AllowSelf = false;
                AllowEnemy = false;
                AllowFriendly = false;
                break;
        }

        List<Character> matches = SearchSpace.Where(candidate =>
            (!candidate.isDead || AllowDead)
            && (candidate.isDead || AllowAlive)
            && (candidate != referenceCharacter || AllowSelf)
            && (candidate.Config.TeamType == referenceTeam || AllowEnemy)
            && (candidate.Config.TeamType != referenceTeam || AllowFriendly)
        ).ToList();

        return matches;
    }
}
