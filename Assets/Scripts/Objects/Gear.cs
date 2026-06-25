using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Gear class represents the gear. Used to pass around gear info into an AdaptiveGearController
public class Gear 
{
    public int teeth;
    public List<int> abilitySlots; //because this doesn't get used to store abilities, only the slot positions matters in this object

    //for if there is no ability cores, and called by others
    public Gear(int t)
    {
        this.teeth = t;
    }

    //debug only: random ability cores. bool doesn't matter, just here to make a diff constructor
    public Gear(int t, bool b) : this(t)
    {
        //decide how many ability cores
        int temp;
        if (t > 8)
        {
            temp = Random.Range(0, 1);
        }
        else if(t > 16)
        {
            temp = Random.Range(0, 2);
        }
        else if(t > 22)
        {
            temp = Random.Range(1, 3);
        }
        else
        {
            temp = Random.Range(1, 4);
        }

        if (temp == 0)
        {
            temp = 1;
        }

        List<int> o = new List<int>();
        if(temp > 0)
        {
            for (int i = 0; i < temp; i++)
            {
                //add a core to a random tooth that doesn't already have a core
                int temp2;
                do
                {
                    temp2 = Random.Range(0, t);
                } while (o.Contains(temp2));
                o.Add(temp2);
            }
        }
        abilitySlots = o;
    }

    //for if there is only 1 ability core
    public Gear(int t, int i) : this(t)
    {
        abilitySlots = new List<int> { i };
    }
}
