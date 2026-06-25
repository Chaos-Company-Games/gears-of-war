using UnityEngine;
using UnityEngine.UI;

//used to manage AbilitySockets in the game. Takes care of gameobjects, colors, buttons, etc etc
public class AbilitySocketController : MonoBehaviour
{
    public AdaptiveGearController parentController; //the gear that this thing is a part of. probably a better way to do this, but we ball

    [SerializeField] GameObject abilityGem; //off at the start, changes color when an ability is put into this
    [SerializeField] Button abilityButton; //the button that the player clicks to socket an ability into it
    public Ability ability = null; //ability is null until something is given to it
    public int tooth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilityButton.onClick.AddListener(() => GearAdditiveController.Instance.AbilityButtonClicked(tooth, parentController, this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addAbility(Ability a)
    {
        abilityGem.SetActive(true);
        Debug.Log("ability core filled, ability is " + a.ability.ToString());
        if (a.ability == AbilityName.Slash) 
        {
            abilityGem.GetComponent<RawImage>().color = new Color(1f, 0, 0);
        }
        else if (a.ability == AbilityName.Smash) 
        {
            abilityGem.GetComponent<RawImage>().color = new Color(0, 1f, 0);
        }
        else if (a.ability == AbilityName.Slap) 
        {
            abilityGem.GetComponent<RawImage>().color = new Color(0, 0, 1f);
        }
        ability = a;
    }

    //this function is used to enable the button for this gameobject.
    //the button is critical for socketing gems, but is not always available.
    public void manageButtonStatus(bool ableState)
    {
        if (ableState)
        {
            abilityButton.gameObject.SetActive(true);
        }
        else
        {
            abilityButton.gameObject.SetActive(false);
        }
    }

    //turns on the button if the socket is empty, so that a new abilities can be given to it
    public void enableIfEmpty()
    {
        if(ability == null)
        {
            abilityButton.gameObject.SetActive(true);
        }
    }
}
