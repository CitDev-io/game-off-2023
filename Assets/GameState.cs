using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameState
{
    public GameState(List<Combatant> _inCombatants) {
        combatants = _inCombatants;
    }
    public int StageNumber = 1;
    public int WaveNumber = 1;
    public GamePhase gamePhase = GamePhase.SETUP;
    public int ScalesOwned = 0;
    public List<Combatant> combatants;

    public List<Combatant> GetAlivePCs() {
        return combatants.FindAll(combatant => combatant is PlayerCharacter && !combatant.isDead);
    }

    public List<Combatant> GetAliveCPUs() {
        return combatants.FindAll(combatant => combatant is CpuCharacter && !combatant.isDead);
    }

    public List<Combatant> GetAllPCs() {
        return combatants.FindAll(combatant => combatant is PlayerCharacter);
    }

    public Combatant getRandomPlayerCharacter() {
        List<Combatant> playerCharacters = GetAlivePCs();

        Combatant randomPc = playerCharacters[Random.Range(0, playerCharacters.Count)];

        return randomPc;
    }
}
