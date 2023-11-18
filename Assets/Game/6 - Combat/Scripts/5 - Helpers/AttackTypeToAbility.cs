public static class AttackTypeToAbility
{
    public static BaseAbilityResolver Lookup(UserAbilitySelection type) {
        BaseAbilityResolver ability = type switch
        {
            UserAbilitySelection.SHIELDBASH => new AbilityShieldBash(),
            UserAbilitySelection.SNEAKATTACK => new AbilitySneakAttack(),
            UserAbilitySelection.CURSEOFSTRENGTH => new AbilityCurseOfStrength(),
            UserAbilitySelection.BLESSING => new AbilityBlessing(),
            UserAbilitySelection.BASICATTACK or _ => new AbilityBasicAttack(),
        };
        
        return ability;
    }
}
