using UnityEngine;

public class SelectableObject 
{
    public Rarity rarity;
}

public enum Rarity
{
    COMMON, //16-31
    UNCOMMON, //roll: 8-15
    RARE, //1 slot gear //roll: 4-7
    EPIC, //2 slot gear //roll: 2-3
    MYTHIC //3 slot gear //roll: 1
}
