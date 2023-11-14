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
