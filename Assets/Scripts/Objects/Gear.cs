using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Gear class represents the gear. Used to pass around gear info into an AdaptiveGearController
public class Gear : SelectableObject 
{
    public int teeth;
    public List<int> abilitySlots; //because this doesn't get used to store abilities, only the slot positions matters in this object

    //for if there is no ability cores, and called by others
    public Gear(int t)
    {
        if (t > 100)
        {
            t = 100;
        }
        if(t < 8)
        {
            t = 8;
        }
        this.teeth = t;
    }

    //debug only: random ability cores. bool doesn't matter, just here to make a diff constructor
    public Gear(int t, bool b) : this(t)
    {
        //decide how many ability cores
        int temp;
        if(t < 8)
        {
            t = 8;
        }
        if (t >= 8 && t <= 16)
        {
            temp = Random.Range(0, 1);
        }
        else if(t > 16 && t<= 21)
        {
            temp = Random.Range(0, 2);
        }
        else if(t > 22 && t<=31)
        {
            temp = Random.Range(1, 3);
        }
        else
        {
            temp = Random.Range(1, 4);
        }

        abilitySlots = PlaceSlots(t, temp);
    }

    //for if you do not care where the slots go
    public Gear(int t, int n) : this(t)
    {
        abilitySlots = PlaceSlots(t, n);
    }

    public Gear(int t, List<int> o) : this(t)
    {
        abilitySlots = o;
    }

    private List<int> PlaceSlots(int teeth, int numSlots)
    {
        List<int> o = new List<int>();
        if (numSlots > 0)
        {
            for (int i = 0; i < numSlots; i++)
            {
                //add a core to a random tooth that doesn't already have a core
                int temp;
                do
                {
                    temp = Random.Range(0, teeth); //get a position
                } while (Enumerable.Range(temp - 2, 5).Any(o.Contains)); //make sure it's open
                o.Add(temp);
            }
        }
        o.Sort();
        return o;
    }
}
