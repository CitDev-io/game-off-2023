public class SummonOrder {
    public SummonableUnit Unit;
    public TeamType Team;
    public Effect InitiatingAbility;

    public SummonOrder(SummonableUnit summonableUnit, TeamType team, Effect initiatingAbility) {
        Unit = summonableUnit;
        Team = team;
        InitiatingAbility = initiatingAbility;
    }
}
