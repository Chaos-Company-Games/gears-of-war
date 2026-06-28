using UnityEngine;

public class Ability : SelectableObject
{
    public AbilityName ability;
    public Ability(AbilityName a)
    {
        ability = a;
    }
}

//named list of abilities
public enum AbilityName
{
    Blap,
    Smash,
    Sling,
    Skewer,
    Stagger
}
