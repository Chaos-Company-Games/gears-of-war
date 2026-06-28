using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private AbilitySocketController tempSelectedAbility; //temporary storage of which ability was selected

    [SerializeField] AdaptiveGearController heartGear; //the player's main gear (undeletable)
    [SerializeField] GameObject gearHolder; //reference to the common parent of all gears
    [SerializeField] GameObject abilityStorage; //reference to the common parent of stored abilities
    [SerializeField] GameObject abilityPlacementCancel; //reference to the button which cancels the placement of an ability

    //Hover Over Menu
    [SerializeField] GameObject hoverOverObject;
    [SerializeField] TextMeshProUGUI hoverOverTitle;
    [SerializeField] TextMeshProUGUI hoverOverDesc;
    private RectTransform hoverOverObjectRect; //internal reference to hoverOverObject's rect transform, so we don't need to constantly find it

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
    //Update - unused

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gearControllerPrefab = Resources.Load<AdaptiveGearController>("AdaptiveGear");
        gearControllers = new List<AdaptiveGearController>();
        storedAbilities = new List<AbilitySocketController>();

        AddGear(new Gear(16, new List<int> { 4 })); //give the player a starting gear
        AddAbility(LevelUpMenu.instance.GenerateAbility()); //give the player a starting ability

        hoverOverObjectRect = hoverOverObject.GetComponent<RectTransform>(); //we use this a lot, set it up once and then use repeatedly
    }

    public void HoverOverEnable(Ability a, float teeth)
    {
        if(hoverOverObject.activeSelf == false){
            hoverOverObject.SetActive(true); //set hover over to active 
            hoverOverTitle.text = a.ability.ToString(); //change the text

            if(a.ability == AbilityName.Blap)
            {
                if (teeth == 0) 
                {
                    hoverOverDesc.text = "Shoot the nearest enemy.";
                }
                else
                {
                    hoverOverDesc.text = "Shoot the nearest enemy for <color=#C41E3A>" + (5 * (1f + (0.1f * (int)a.rarity)) + teeth / 10f).ToString() + "</color> damage.";

                }
            }
            else if(a.ability == AbilityName.Smash)
            {
                if (teeth == 0)
                {
                    hoverOverDesc.text = "Attack all enemies in melee range.";
                }
                else
                {
                    hoverOverDesc.text = "Attack all enemies in melee range for <color=#C41E3A>" + (5 * (.6f + (0.1f * (int)a.rarity)) + teeth / 10f).ToString() + "</color> damage.";
                }
            }
            else if(a.ability == AbilityName.Skewer)
            {
                if (teeth == 0) 
                {
                    hoverOverDesc.text = "Attack an enemy in melee range, and then heal for 1/4 that.";
                }
                else
                {
                    hoverOverDesc.text = "Attack an enemy in melee range for <color=#C41E3A>" + (5 * (.5f + (0.1f * (int)a.rarity)) + teeth / 10f).ToString() + "</color> damage, and then heal for 1/4 that.";
                }
            }
            else if(a.ability == AbilityName.Stagger)
            {
                if (teeth == 0)
                {
                    hoverOverDesc.text = "Stun the nearest enemy temporarily.";
                }
                else
                {
                    hoverOverDesc.text = "Stun the nearest enemy for <color=#C41E3A>" + (teeth / 10f).ToString() + "</color> seconds.";
                }
            }
            else if(a.ability == AbilityName.Sling)
            {
                if(teeth == 0)
                {
                    hoverOverDesc.text = "Shoot the nearest enemy and enemies around.";
                }
                else 
                {
                    hoverOverDesc.text = "Shoot the nearest enemy and enemies around it for <color=#C41E3A>" + (5 * (0.4f + (0.1f * (int)a.rarity)) + teeth / 10f).ToString() + "</color> damage.";
                }
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(hoverOverObject.transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null, out Vector2 localPos); //convert mouse pos to screen space relative to canvas
            localPos.x = localPos.x + hoverOverObjectRect.rect.width / 2; //move it so that it has the mouse at the top left
            localPos.y = localPos.y - hoverOverObjectRect.rect.height / 2;

            hoverOverObjectRect.localPosition = localPos; //set local position of hoverover
        }
    }

    public void HoverOverDisable()
    {
        if (hoverOverObject.activeSelf == true)
        {
            hoverOverObject.SetActive(false); //turn it back off
        }
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
            RectTransform temp = storedAbilities[i].GetComponent<RectTransform>();

            //place it in its position: pivot point is setup earlier, so just place it 'i' down
            temp.anchoredPosition = new Vector3(5, -10 + (i * -50), 0);
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

    //triggers when an empty socket is clicked on, when a stored ability is selected
    public void AbilityButtonClicked(int tooth, AdaptiveGearController gearController, AbilitySocketController socketController)
    {
        DisableButtons(); //turn the empty sockets back off
        socketController.addAbility(tempSelectedAbility.ability); //move the ability into the selected empty socket
        abilityPlacementCancel.SetActive(false); //turn the cancel button back off
        storedAbilities.Remove(tempSelectedAbility); //take it out of the list
        Destroy(tempSelectedAbility.gameObject); //destroy the gameobject
        tempSelectedAbility = null; //remove it from the select ability storage
        PlaceStoredAbilities(); //replace the other abilities

    }
    //when a stored ability is clicked, highlight the slots it can go into, and remember it
    public void StoredAbilityClicked(AbilitySocketController ac)
    {
        abilityPlacementCancel.SetActive(true); //turn on the cancel button
        EnableEmptySockets();       //highlight sockets it can sit in
        tempSelectedAbility = ac; //remember it
    }

    //cancels the placement of a stored ability
    public void CancelAbilityPlacement()
    {
        abilityPlacementCancel.SetActive(false);
        DisableButtons();
        tempSelectedAbility = null;
    }

    //add an ability to the ability storage area
    public void AddAbility(Ability a)
    {
        AbilitySocketController temp = Instantiate(abilitySocketControllerPrefab); //make the socket
        temp.transform.SetParent(abilityStorage.transform); //give it a parent
        temp.setUp(a); //setup the color and on click event of that socket
        storedAbilities.Add(temp); //add it to the list

        //set it up so that it uses the parent as a pivot point
        RectTransform rectTrans = temp.GetComponent<RectTransform>();
        rectTrans.anchorMin = new Vector2(0, 1);
        rectTrans.anchorMax = new Vector2(0, 1);
        rectTrans.pivot = new Vector2(0, 1);

        PlaceStoredAbilities();
    }

    //debug function: add random ability
    public void AddRandomAbility()
    {
        AddAbility(new Ability((AbilityName)Random.Range(0, 5)));

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
        AddGear(new Gear(Random.Range(8, 60), true));
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
