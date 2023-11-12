## Tips

### Setup
- Clone this repository using Git
```sh
git clone git@github.com:CitDev-io/game-off-2023.git
```
or
```sh
git clone https://github.com/CitDev-io/game-off-2023.git
```

### Unity
- Open this project in Unity `2021.3.22f1`
- Open the `CombatScene` from `Assets/Scenes/`
- Do not rename the `Assets/Game/6 - Combat/CombatConfig/Set List 1` file. Make sure this contains the official stage list you want in the published game!
- *note:* We're going to avoid sending changes to the Unity Scenes when we ship your changes.

### Saving Changes Back
- We do NOT want to send any changes to Unity Scenes (Assets/Scenes/) back to GitHub. 
- All other changes you make are safe to send

```sh
> git add .
> git reset HEAD -- ./Assets/Scenes/
> git commit -m "Describe Changes"
> git push origin develop
```

- This will send all of your changes to GitHub and will be built and shipped to the itch.io game.


### Configuration Files
1. Enemy Set List: Contains a list of Stages
1. Stage/Wave: Contains a list of enemies to include in each wave of a stage
1. Party: Pre-set list for the Player Party, we really just need the one right now
1. Character: A combat scene participant, good or bad. 

- These are contained in the `Assets/Game/6 - Combat/CombatConfig/` folder.
- Create new ones by right clicking in the folder you want it to be created in > Create > GameOff2023 > New [ConfigType], Name the file
- Any changes made to values in these files sticks. I can help with reverting anything you didn't mean to adjust.