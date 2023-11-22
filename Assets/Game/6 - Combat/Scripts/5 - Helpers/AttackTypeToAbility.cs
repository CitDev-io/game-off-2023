public static class AttackTypeToAbility
{
    public static BaseAbilityResolver Lookup(UserAbilitySelection type, CharacterConfig reference) {
        BaseAbilityResolver ability = type switch
        {
            UserAbilitySelection.SHIELDBASH => new AbilityShieldBash(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.SNEAKATTACK => new AbilitySneakAttack(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.CURSEOFSTRENGTH => new AbilityCurseOfStrength(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.BLESSING => new AbilityBlessing(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.DEADLYPOUNCE => new AbilityDeadlyPounce(),
            UserAbilitySelection.CELESTIALBARRAGE => new AbilityCelestialBarrage(),
            UserAbilitySelection.NIBBLE => new AbilityNibble(),
            UserAbilitySelection.CINDERSLAP => new AbilityCinderSlap(),
            UserAbilitySelection.FLAREBLITZ => new AbilityFlareBlitz(),
            UserAbilitySelection.SEARINGSTUN => new AbilitySearingStun(),
            UserAbilitySelection.BLAZEBARRAGE => new AbilityBlazeBarrage(),
            UserAbilitySelection.CAUTERIZE => new AbilityCauterize(),
            UserAbilitySelection.POLYMORPH => new AbilityPolymorph(),
            UserAbilitySelection.MASSHEAL => new AbilityMassHeal(),
            UserAbilitySelection.BASICATTACK or _ => new AbilityBasicAttack(),
        };
        
        return ability;
    }
}
