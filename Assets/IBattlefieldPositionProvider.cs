using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefieldPositionProvider {
    public BattlefieldPosition GetNextOpenBattlefieldPositionForTeam(
        List<int> TakenSpotIds, TeamType team);

    public Character InstantiateNewCharacterForConfig(CharacterConfig config);

    public CharacterConfig GetConfigForUnitType(SummonableUnit summonableUnit);
}
