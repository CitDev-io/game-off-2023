using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveProvider : IWaveProvider
{
    private PartyConfig _partyConfig;
    private EnemySetList _enemySetList;

    public int StageCount => _enemySetList.GameStages.Count;

    public StageConfig GetStageConfig(int stageNum) {
        if (_enemySetList.GameStages.Count < stageNum) {
            return _enemySetList.GameStages[0];
        }
        return _enemySetList.GameStages[stageNum-1];
    }

    public int WaveCountInStage(int stageNum) {
        if (_enemySetList.GameStages.Count < stageNum) {
            return 1;
        } else if (_enemySetList.GameStages[stageNum-1].Wave7.Count > 0) {
            return 7;
        } else if (_enemySetList.GameStages[stageNum-1].Wave6.Count > 0) {
            return 6;
        } else if (_enemySetList.GameStages[stageNum-1].Wave5.Count > 0) {
            return 5;
        } else if (_enemySetList.GameStages[stageNum-1].Wave4.Count > 0) {
            return 4;
        } else if (_enemySetList.GameStages[stageNum-1].Wave3.Count > 0) {
            return 3;
        } else if (_enemySetList.GameStages[stageNum-1].Wave2.Count > 0) {
            return 2;
        } else if (_enemySetList.GameStages[stageNum-1].Wave1.Count > 0) {
            return 1;
        }
        return 0;
    }

    public WaveInfo GetWaveInfo(int stageNum, int waveNum) {
        CharacterConfig Boss = GetEnemyWave(stageNum, waveNum).FirstOrDefault(c => c.IsBoss);
        return new WaveInfo(
            Boss,
            waveNum,
            stageNum
        );
    }

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
        if (_enemySetList.GameStages.Count < stageNumber) {
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
