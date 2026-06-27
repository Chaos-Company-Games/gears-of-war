using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
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
        if(t <= 8)
        {
            t = 8;
        }
        if (t > 8 && t <= 16)
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

        List<int> o = new List<int>();
        if(temp > 0)
        {
            for (int i = 0; i < temp; i++)
            {
                //add a core to a random tooth that doesn't already have a core
                int temp2;
                int temp3;
                int temp4;
                do
                {
                    temp2 = Random.Range(0, t);
                    temp3 = temp2 + 1;
                    temp4 = temp2 - 1;
                } while (o.Contains(temp2) || o.Contains(temp3) || o.Contains(temp4)); //ensure there is not already a socket there or adjacent
                o.Add(temp2);
            }
        }
        abilitySlots = o;
    }

    //for if you do not care where the slots go
    public Gear(int t, int n) : this(t)
    {
        List<int> o = new List<int>();
        if (n > 0)
        {
            for (int i = 0; i < n; i++)
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
}
