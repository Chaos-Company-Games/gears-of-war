using UnityEngine;

public class Ability
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
    Slap,
    Smash,
    Slash
}
