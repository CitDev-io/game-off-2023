public class SummonOrder {
    public SummonableUnit Unit;
    public TeamType Team;
    public BaseAbilityResolver InitiatingAbility;

    public SummonOrder(SummonableUnit summonableUnit, TeamType team, BaseAbilityResolver initiatingAbility) {
        Unit = summonableUnit;
        Team = team;
        InitiatingAbility = initiatingAbility;
    }
}
