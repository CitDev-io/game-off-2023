public class ReviveOrder {
    public Character character;
    public int percentHealth = 100;
    public Effect InitiatingAbility;
    public ReviveOrder(Character character, int percentHealth, Effect initiator) {
        this.character = character;
        this.percentHealth = percentHealth;
        InitiatingAbility = initiator;
    }
}
