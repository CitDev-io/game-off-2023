using System.Collections.Generic;
using System.Linq;

public class EffectHeavyweightHeatwave : Effect
{
    public EffectHeavyweightHeatwave(string name = "Heavyweight Heatwave", string description = "Deals damage to all enemies based on how many stacks of Heatweight are on the caster") {
        Name = name;
        Description = description;
        TargetScope = EligibleTargetScopeType.NONE;
        IsAbility = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DotDamage");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> EnemyList)
    {
        var _e = new EffectPlan(source, target, this);


        BuffHeatwave myBuff = (BuffHeatwave) source.GetBuff<BuffHeatwave>();

        if (myBuff == null) {
            return _e;
        }

        int DamageDealt = source.GetSpecialAttackRoll(false);

        int AdjustedDamage = (int) (DamageDealt * (myBuff.Charges * 0.1f));

        EnemyList.ForEach(victim => {
            if (victim == null || victim.isDead) {
                return;
            }
            DamageOrder DamageToTarget = new DamageOrder(
                source,
                victim,
                AdjustedDamage,
                this
            );

            _e.Add(DamageToTarget);
        });

        return _e;
    }     
}
