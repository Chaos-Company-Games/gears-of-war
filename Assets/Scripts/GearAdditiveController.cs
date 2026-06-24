using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Controller for the GearAdditive scene.
//Main job is mananging gears visually
public class GearAdditiveController : MonoBehaviour
{

    AdaptiveGearController gearControllerPrefab; //prefab gear controller
    //List of AdaptiveGearControllers; order matters
    List<AdaptiveGearController> gearControllers;

    [SerializeField] AdaptiveGearController heartGear; //the player's main gear (undeletable)
    [SerializeField] GameObject gearHolder; //common parent of all gears

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gearControllerPrefab = Resources.Load<AdaptiveGearController>("AdaptiveGear");
        Debug.Log(gearControllerPrefab);
        gearControllers = new List<AdaptiveGearController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Whenever the number of gears changes, recalc their positions and spin directions
    private void PlaceGears()
    {
        for(int i = 0; i < gearControllers.Count; i++) //for each gear that we're using:
        {
            //set its position
            if(i == 0) //if it's the first gear, go based upon the heartGear
            {
                gearControllers[i].transform.position = new Vector3(heartGear.GetRadius() + gearControllers[i].GetRadius(), 0, 0) + heartGear.transform.position;
            }
            else //otherwise, go off of the previous gear
            {
                gearControllers[i].transform.position = new Vector3(gearControllers[i-1].GetRadius() + gearControllers[i].GetRadius(), 0, 0) + gearControllers[i-1].transform.position;
            }
            //if i is even, rotate counterclockwise: if i is odd, rotate clockwise
            if (i % 2 == 1)
            {
                gearControllers[i].SetSpin(SpinDir.Clockwise);
            }
            else
            {
                gearControllers[i].SetSpin(SpinDir.CounterClockwise);
            }
        }
    }

    #region Gear List Management
    //add a gear, pass in the gear class to add to the chain
    public void AddGear(Gear g)
    {
        AdaptiveGearController temp = Instantiate(gearControllerPrefab);
        temp.SetTeeth(g.teeth);
        temp.transform.SetParent(gearHolder.transform);
        gearControllers.Add(temp);
        PlaceGears();
    }

    //testing function: adding randomly sized gears
    public void AddRandomGear()
    {
        AddGear(new Gear(Random.Range(6, 40)));
    }

    public void RemoveGear(int i)
    {
        Destroy(gearControllers[i].gameObject);
        gearControllers.RemoveAt(i);
        PlaceGears();
    }

    //testing function: remove a random gear
    public void RemoveRandom()
    {
        RemoveGear(Random.Range(0, gearControllers.Count));
    }
    #endregion
}
