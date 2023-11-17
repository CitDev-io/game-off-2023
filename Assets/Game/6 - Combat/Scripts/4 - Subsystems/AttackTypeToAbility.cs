public static class AttackTypeToAbility
{
    public static Ability Lookup(UserAbilitySelection type) {
        Ability ability = type switch
        {
            UserAbilitySelection.SHIELDBASH => new AbilityShieldBash(),
            UserAbilitySelection.SNEAKATTACK => new AbilitySneakAttack(),
            UserAbilitySelection.BASICATTACK or _ => new AbilityBasicAttack(),
        };
        
        return ability;
    }
}
