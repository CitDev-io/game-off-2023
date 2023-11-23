public static class AttackTypeToAbility
{
    public static Effect Lookup(UserAbilitySelection type, CharacterConfig reference) {
        Effect ability = type switch
        {
            UserAbilitySelection.SHIELDBASH => new AbilityShieldBash(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.SNEAKATTACK => new AbilitySneakAttack(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.CURSEOFSTRENGTH => new AbilityCurseOfStrength(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.BLESSING => new AbilityBlessing(reference.AttackTreeLevel, reference.SupportTreeLevel),
            UserAbilitySelection.DEADLYPOUNCE => new AbilityDeadlyPounce(),
            UserAbilitySelection.CELESTIALBARRAGE => new AbilityCelestialBarrage(),
            UserAbilitySelection.NIBBLE => new AbilityNibble(),
            UserAbilitySelection.HEAVYWEIGHTHEATWAVE => new AbilityHeavyweightHeatwave(),
            UserAbilitySelection.CINDERSLAP => new AbilityCinderSlap(),
            UserAbilitySelection.FLAREBLITZ => new AbilityFlareBlitz(),
            UserAbilitySelection.COUNTERBARK => new AbilityCounterbark(),
            UserAbilitySelection.PEGLEGPLAGUE => new AbilityPegLegPlague(),
            UserAbilitySelection.SKELETALSHIELD => new AbilitySkeletalShield(),
            UserAbilitySelection.SEARINGSTUN => new AbilitySearingStun(),
            UserAbilitySelection.COUNTERBLAZE => new AbilityCounterblaze(),
            UserAbilitySelection.BLAZEBARRAGE => new AbilityBlazeBarrage(),
            UserAbilitySelection.CAUTERIZE => new AbilityCauterize(),
            UserAbilitySelection.YOHOINFERNO => new AbilityYoHoInferno(),
            UserAbilitySelection.HOLLOWHOWL => new AbilityHollowHowl(),
            UserAbilitySelection.POLYMORPH => new AbilityPolymorph(),
            UserAbilitySelection.TIMBERTIP => new AbilityTimberTip(),
            UserAbilitySelection.MASSHEAL => new AbilityMassHeal(),
            UserAbilitySelection.BASICATTACK or _ => new AbilityBasicAttack(),
        };
        
        return ability;
    }
}
