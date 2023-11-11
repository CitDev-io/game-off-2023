using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameState
{
    public int StageNumber = 1;
    public int WaveNumber = 1;
    public int ScalesOwned = 0;
    public List<Character> combatants = new List<Character>();

    public List<Character> GetAlivePCs() {
        return combatants.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && !combatant.isDead);
    }

    public List<Character> GetAliveCPUs() {
        return combatants.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && !combatant.isDead);
    }

    public List<Character> GetDefeatedPCs() {
        return combatants.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && combatant.isDead);
    }

    public List<Character> GetDefeatedCPUs() {
        return combatants.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && combatant.isDead);
    }

    public List<Character> GetAllPCs() {
        return combatants.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER);
    }

    public Character getRandomPlayerCharacter() {
        List<Character> playerCharacters = GetAlivePCs();

        Character randomPc = playerCharacters[Random.Range(0, playerCharacters.Count)];

        return randomPc;
    }
}
