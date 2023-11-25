using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// this fella should live in monospace on the same GameManager object
public class SpawnPointProvider : MonoBehaviour, IBattlefieldPositionProvider
{
    public CharacterConfig Summon_GHOST;
    public GameObject CharacterPuckPrefab;

    public BattlefieldPosition GetNextOpenBattlefieldPositionForTeam(
        List<int> TakenSpotIds, TeamType team) {

        for (var i=0; i<5; i++) {
            string childName = team == TeamType.PLAYER ? "Player Field" : "Enemy Field";
            SpawnPoint spawn = transform.Find(childName).transform.Find("SpawnPoint" + i.ToString()).GetComponent<SpawnPoint>();
            
            if (spawn == null) {
                return null;
            }

            if (!TakenSpotIds.Contains(spawn.SpotId)) {
                return spawn.GetComponent<SpawnPoint>().GetInfo();
            }
        }
        return null;
    }

    public CharacterConfig GetConfigForUnitType(SummonableUnit summonableUnit) {
        switch (summonableUnit) {
            case SummonableUnit.GHOST:
                return Summon_GHOST;
            default:
                return null;
        }
    }

    public Character InstantiateNewCharacterForConfig(CharacterConfig config) {
        GameObject newPc = Instantiate(CharacterPuckPrefab);
        newPc.transform.parent = transform;
        int countExisting = GameObject.FindObjectsOfType<Character>().Count(go => go.name.StartsWith(config.Name));

        if (countExisting > 1) {
            newPc.name = config.Name + " " + countExisting.ToString();
        } else {
            newPc.name = config.Name;
        }
        Character character = newPc.GetComponent<Character>();
        return character;
    }
}
