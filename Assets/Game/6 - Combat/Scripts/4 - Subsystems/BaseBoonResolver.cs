using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseBoonResolver {
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }

    public abstract void ApplyToEligible(List<CharacterConfig> character);

    public abstract void RemoveFromOwning(List<CharacterConfig> characters);

    public abstract bool IsEligible(CharacterConfig character);

    public bool AvailableForAny(List<CharacterConfig> characters) {
        return characters.Any(character => IsEligible(character));
    }
}
