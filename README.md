![image](https://img.itch.zone/aW1nLzE0MTc1NjI5LnBuZw==/original/mT6Awx.png)
A 2D Turn-Based RPG by CitizenDevelopers (citdev.io)

## Created in 30 Days
- For [Game Off 2023](https://itch.io/jam/game-off-2023) Hosted by Github
- By [Creighcl](https://github.com/creighcl), [michaelsha](https://github.com/michaelsha), [cman1526](https://github.com/cman1526), and [studiochairo](https://github.com/studiochairo)

## Playable in Browser (WebGL)
- [https://citizendevelopers.itch.io/game-off-2023](https://citizendevelopers.itch.io/game-off-2023)

## What's in the Repo?
- Full Unity C# source code and game assets
- GitHub Actions pipeline that packages and deploys commits straight to Itch.io
- Mostly clean code and organization. The final 3 days were a bit messy!



## Code Layer Responsibility Map
Majority of C# game logic is written into `/Assets/Game/6 - Combat/Scripts/`. Each script should fall under one of these layers. (Plenty of UI/State are out of place ☹️)

### Orchestration
*Monobehaviour*

`CombatReferee` is the top-level element of the CombatScene. Responsible for step logic through the phases of a Character's turn. Obligated to wait when any visual layers (**Systems**) are in an `IsPerforming` state.
- Sets up and communicates with public methods within **State**, **Systems**, and **Helpers**


### State
*No Monobehaviours*

State objects scoped to different aspects of the game and combat. Responsible for holding state, lookup methods pertaining to state, and resolving state changes when prompted through public surface
- `CombatState` Responsible for resolving state changes provided by **Orchestration** layer involving **Subsystems**
- `EventProvider` creates event-driven communication for scripts that publish or subscribe
- `Character` special-case Monobehavior. Primary purpose is to hold current state for Combatants in the Combat Scene and reference character's configuration (scriptable object)

### Systems
*Monobehaviour*

Visualization scripts that act out state changes received from subscribed events in the `EventProvider`. 
- `StageChoreographer` responsible for directing "performances" of "Actors" in the scene
- `UIManager` responsible for directing all UI transitions, menus, and User Input listeners with its small army of `UI_` scripts
- `ActorCharacter` motor script that Queues and runs actor "performances"
- Subscribes to events through `EventProvider`. Generates only user-input related events.


### Subsystems
*No monobehaviours*

State resolvers that can be instantiated and passed by reference to manipulate game state.
- `Buff`s are added to characters. Built-in resolvers are triggered by **State** and **Orchestration** layers based on their phase resolution logic
- `Effect`s are instantiated and added to `EffectPlan`s that will collect up all of the game logic that must resolve. (Abilities, Triggers, Damage Buff Ticks, etc)
- `Boon`s are applied to eligible characters. Built-in resolvers are triggered by **State** layer to upgrade abilities in the **State**
- Does not affect state. Does not subscribe or publish to `EventProvider`

### Helpers and Enums
*No monobehaviours*

Mostly lookups and mapping-based objects
- All Characters, Waves, Stages are configurable within the Unity editor using `ScriptableObjects`. Most helpers are helping factories convert config files into usable C# objects
- Does not affect state. Does not subscribe or publish to `EventProvider`


