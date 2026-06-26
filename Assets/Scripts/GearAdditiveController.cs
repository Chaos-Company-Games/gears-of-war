using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Controller for the GearAdditive scene.
//Main job is mananging gears visually
public class GearAdditiveController : MonoBehaviour
{
    public static GearAdditiveController Instance { get; private set; }

    AbilitySocketController abilitySocketControllerPrefab; //prefab holder

    AdaptiveGearController gearControllerPrefab; //prefab gear controller
    //List of AdaptiveGearControllers; order matters
    List<AdaptiveGearController> gearControllers;
    List<AbilitySocketController> storedAbilities; //holds the stored abilities in the storage area

    [SerializeField] AdaptiveGearController heartGear; //the player's main gear (undeletable)
    [SerializeField] GameObject gearHolder; //common parent of all gears
    [SerializeField] GameObject abilityStorage; //reference to the common parent of stored abilities

    //awake is called before Start
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        abilitySocketControllerPrefab = Resources.Load<AbilitySocketController>("AbilitySocket");
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gearControllerPrefab = Resources.Load<AdaptiveGearController>("AdaptiveGear");
        gearControllers = new List<AdaptiveGearController>();
        storedAbilities = new List<AbilitySocketController>();
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

    //places the stored abilities whenever it needs to be re-calced.
    private void PlaceStoredAbilities()
    {
        for(int i = 0; i < storedAbilities.Count; i++)
        {
            //set its position
            storedAbilities[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, i * -20, 0);
        }
    }

    //debug: forces all AbilityCoreControllers to enable their buttons 
    public void ShowButtons()
    {
        foreach (AdaptiveGearController gearController in gearControllers)
        {
            gearController.EnableButtons();
        }
    }

    //turns on the buttons for empty ability controllers
    //needed for placing abilities
    public void EnableEmptySockets()
    {
        foreach(AdaptiveGearController gearController in gearControllers)
        {
            gearController.EnableEmptySockets();
        }
    }

    public void DisableButtons()
    {
        foreach (AdaptiveGearController gearController in gearControllers)
        {
            gearController.DisableButtons();
        }
    }

    //triggers when a highlighted ability is clicked on, when an ability is active
    public void AbilityButtonClicked(int tooth, AdaptiveGearController gearController, AbilitySocketController socketController)
    {
        DisableButtons();
        //USE STORED ABILITY, INSTEAD OF RANDOM
        //int temp = Random.Range(0, 3);
        //if (temp == 0) { socketController.addAbility(new Ability(AbilityName.Slash)); }
        //else if (temp == 1) { socketController.addAbility(new Ability(AbilityName.Smash)); }
        //else if (temp == 2) { socketController.addAbility(new Ability(AbilityName.Slap)); }
        //else
        //{
        //    socketController.addAbility(new Ability(AbilityName.Slap));
        //}
    }
    //when a stored ability is clicked, highlight the slots it can go into.
    public void StoredAbilityClicked(AbilitySocketController ac)
    {

    }

    //add an ability to the ability storage area
    public void AddAbility(Ability a)
    {
        AbilitySocketController temp = Instantiate(abilitySocketControllerPrefab); //make the socket
        temp.transform.SetParent(abilityStorage.transform); //give it a parent
        temp.setUp();
        storedAbilities.Add(temp); //add it to the list
        PlaceStoredAbilities();
    }

    //debug function: add random ability
    public void AddRandomAbility()
    {
        AddAbility(new Ability((AbilityName)Random.Range(0, 3)));

    }

    #region Gear List Management
    //add a gear, pass in the gear class to add to the chain
    public void AddGear(Gear g)
    {
        AdaptiveGearController temp = Instantiate(gearControllerPrefab);
        temp.SetTeethAndSlots(g.teeth, g.abilitySlots);
        temp.transform.SetParent(gearHolder.transform);
        gearControllers.Add(temp);
        PlaceGears();
    }

    //testing function: adding randomly sized gears
    public void AddRandomGear()
    {
        AddGear(new Gear(Random.Range(6, 40), true));
    }

    public void RemoveGear(int i)
    {
        if(gearControllers.Count == 0) return;
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
