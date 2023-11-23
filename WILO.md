# 11-13-2023
----------

- We're now prompting for ability selection. if you have a special ability, it'll prompt
- We can put the special ability name. The UI has access to the current combatant. CurCom -> Config -> Special Attack -> Translate enum to pretty name


### Where we need to go:
- Time to write up the actual ability attack
- Probably time to carve this out of the character object
- All info needed to process is there and can make effects to charsheet public access methods
- Need to be able to say "do shieldbash" and the damage is applied and buffs are applied.
- Resolver should be built with a ref to gameState for random character choices


### Things I'm Thinking About
- UI and Choreo need to start doing some acting. Let's not wait until the end to have acting breaks and hope UI transitions well.


# 11-15-2023
-------------
- Now have acting within the phases and acting breaks
- Text scrawl is working
- manual selections (ability/target) is working
- ready for abilities+buffs
- ability resolution is ready to carve out
- boon selection is ready to drop in
- able to do minor performing
- could really use elemental icons
- buff icons would be nice
- how will we "paladin is stunned and cannot act" on the scrawl?
- god damn. clear path from here. truly.
- changing ability selection to 3 is gonna be meh. use target for tips
- if we have multiple targets beyond 6 slots, consider making the 5th one "NEXT >" and "< BACK" and toggle which we are showing
- maybe decoration for eligible targets? choreo has a hook.
- UI can subsystem out the text generation/translation of attacks
- UI should onboard win/lose
- consider for selector menu items:
    - tooltip/select-but-dont-choose should show ability info on scrawl
    - maybe (i) info for tappers
- tapping a mob when not in choose-target mode should zoom in and give more information, stat sheet
- ready for Turn Order logic/acting, easy to feed.



11/17/2023
-------------
- buff doesn't need an agingphase, likely. keep an eye out. cut when ready
- buff application not using pretty name!



11-21-2023
------------
ExecutedAbility should just be the order sheet. "ExecutableAbility" and resolve stack style. BaseAbilityResolver is just an "Effect". Put IBUFF on one and now it doesn't need the passthrough like for DOT.


11-22-2023
--------------
ExecutedAbility is now "EffectPlan"
BaseAbilityRsolver is now "Effect"

Buffs might be able to be Effects, but more likely a polymorphic class "BuffEffect"

...then we can have "AbilityEffect" and be able to sniff out on the resolver if we're treating this like an ability cast or not (and for performance's sake)

I created the Counterattack buff but can't see it triggering. Revisit this logic.

If all types of Orders used the same interface, they could be placed on a stack and resolved within the Combat State resolver.

DMG, BUFF need to be resolved all in one go from an effect plan so this might turn order stacks into something that needs more planning.
