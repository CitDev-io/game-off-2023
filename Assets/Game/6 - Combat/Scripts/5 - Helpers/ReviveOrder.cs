public class ReviveOrder {
    public Character character;
    public int percentHealth = 100;
    public BaseAbilityResolver InitiatingAbility;
    public ReviveOrder(Character character, int percentHealth, BaseAbilityResolver initiator) {
        this.character = character;
        this.percentHealth = percentHealth;
        InitiatingAbility = initiator;
    }
}
