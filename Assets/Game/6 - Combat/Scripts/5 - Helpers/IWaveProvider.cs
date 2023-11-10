using System.Collections.Generic;

public interface IWaveProvider
{
    public List<CharacterConfig> GetPCParty();

    public List<CharacterConfig> GetEnemyWave(int stageNumber, int waveNumber);
}
