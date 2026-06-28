using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//used to manage AbilitySockets in the game. Takes care of gameobjects, colors, buttons, etc etc
public class AbilitySocketController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AdaptiveGearController parentController; //the gear that this thing is a part of. if not in a gear, is null.

    [SerializeField] GameObject abilityGem; //off at the start, changes color when an ability is put into this
    [SerializeField] Button abilityButton; //the button that the player clicks to socket an ability into it
    public Ability ability = null; //ability is null until something is given to it
    public int tooth;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ability != null) 
        {
            GearAdditiveController.Instance.HoverOverEnable(ability);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(ability != null) 
        {
            GearAdditiveController.Instance.HoverOverDisable();

        }
    }

    public void addAbility(Ability a)
    {
        abilityGem.SetActive(true);
        //Debug.Log("ability core filled, ability is " + a.ability.ToString());
        if ((int)a.ability == 0)
        {
            abilityGem.GetComponent<RawImage>().color = new Color(1f, 0, 0);
        }
        else if ((int)a.ability == 1)
        {
            abilityGem.GetComponent<RawImage>().color = Color.forestGreen;
        }
        else if ((int)a.ability == 2)
        {
            abilityGem.GetComponent<RawImage>().color = new Color(0, 0, 1f);
        }
        else if ((int)a.ability == 3)
        {
            abilityGem.GetComponent<RawImage>().color = Color.mediumPurple;
        }
        else if((int)a.ability == 4)
        {
            abilityGem.GetComponent<RawImage>().color = Color.darkOrange;
        }
        ability = a;
    }
    //Setup functions setup the button's onClick function. different functions, depending on if in ability storage or on a gear
    #region SetUp
    //this setUp function is called from the GearAdditiveController
    public void setUp(Ability a)
    {
        parentController = null;
        abilityButton.onClick.AddListener(() => GearAdditiveController.Instance.StoredAbilityClicked(this));
        abilityButton.gameObject.SetActive(true);
        abilityButton.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0f);
        addAbility(a);
    }
    //this setUp function is called from AdaptiveGearController
    public void setUp(AdaptiveGearController pC) 
    {
        parentController = pC;
        abilityButton.onClick.AddListener(() => GearAdditiveController.Instance.AbilityButtonClicked(tooth, parentController, this));
    }
    #endregion
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
    //when an ability is triggered, make the socket flicker
    public IEnumerator Flash()
    {
        Color temp = abilityGem.GetComponent<RawImage>().color;
        abilityGem.GetComponent<RawImage>().color = Color.Lerp(temp, Color.antiqueWhite, .40f); //temporarily change color to whiter version
        yield return new WaitForSeconds(.35f); //wait
        abilityGem.GetComponent<RawImage>().color = temp; //return color to original
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
