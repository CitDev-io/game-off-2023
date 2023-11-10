using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveProvider : IWaveProvider
{
    private PartyConfig _partyConfig;
    private EnemySetList _enemySetList;

    public WaveProvider(PartyConfig partyConfig, EnemySetList enemySetList)
    {
        _partyConfig = partyConfig;
        _enemySetList = enemySetList;
    }

    public List<CharacterConfig> GetPCParty()
    {
        return _partyConfig.PartyMembers;
    }

    public List<CharacterConfig> GetEnemyWave(int stageNumber, int waveNumber)
    {
        if (_enemySetList.GameStages[stageNumber-1] == null) {
            return _enemySetList.GameStages[0].Wave1;
        }
        switch (waveNumber) {
            case 1: return _enemySetList.GameStages[stageNumber-1].Wave1;
            case 2: return _enemySetList.GameStages[stageNumber-1].Wave2;
            case 3: return _enemySetList.GameStages[stageNumber-1].Wave3;
            case 4: return _enemySetList.GameStages[stageNumber-1].Wave4;
            case 5: return _enemySetList.GameStages[stageNumber-1].Wave5;
            case 6: return _enemySetList.GameStages[stageNumber-1].Wave6;
            case 7: return _enemySetList.GameStages[stageNumber-1].Wave7;
            default: return _enemySetList.GameStages[stageNumber-1].Wave7;
        }
    }
}
