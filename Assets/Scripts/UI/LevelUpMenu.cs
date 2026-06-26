using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

//Attach this to level up canvas UI, and disable it by default
public class LevelUpMenu : MonoBehaviour
{
    public static LevelUpMenu instance {get; private set; }

    [Header("UI References")]
    public GameObject menuPanel;
    public Button[] optionButtons; //3 buttons
    public TextMeshProUGUI[] optionLabels; //label to each button
    public TextMeshProUGUI[] rarityLabels;
    public TextMeshProUGUI levelLabel;

    //Placeholder option names, we change this later
    private SelectableObject[] levelUpOptions = new SelectableObject[3];


    void Awake()
    {
        //Singelton stuff
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            menuPanel.SetActive(false);

            //Wire up buttons and such
            for(int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[i].onClick.AddListener(() => OnOptionChosen(index));
            }
        }
    }

    public void Show(int newLevel)
    {
        menuPanel.SetActive(true);

        if (levelLabel != null)
        {
            levelLabel.text = $"Level {newLevel}!";
        }

        //Pick 3 random options from the pool, for now just show placeholders
        for(int i = 0; i < optionLabels.Length; i++)
        {
            levelUpOptions[i] = GenerateSelectableObject();
            if (optionLabels[i] != null)
            {
                rarityLabels[i].text = levelUpOptions[i].rarity.ToString();

                if (levelUpOptions[i] is Ability)
                {
                    optionLabels[i].text = ((Ability)levelUpOptions[i]).ability.ToString();
                }
                else if (levelUpOptions[i] is Gear)
                {
                    optionLabels[i].text = $"{((Gear)levelUpOptions[i]).teeth} : tooth Gear";
                }
                else
                {
                    Debug.LogError("Oh come on now this isnt a real option");
                }
                //optionLabels[i].text = placeHolderOptions[i];
            }
        }

    }

    void OnOptionChosen(int index)
    {
        if (levelUpOptions[index] is Ability)
        {
            GearAdditiveController.Instance.AddAbility((Ability)levelUpOptions[index]);
        }
        else if (levelUpOptions[index] is Gear)
        {
            GearAdditiveController.Instance.AddGear((Gear)levelUpOptions[index]);
        }
        else
        {
            Debug.LogError("Mamma miaaaa");
        }
        //Apply upgrade here
        //For example: UpgradeManager.instance.Apply(chosenUpgrade);

        Close();
    }

    void Close()
    {
        menuPanel.SetActive(false);
        //Unpause game
        //Time.timeScale = 1f;
    }

    SelectableObject GenerateSelectableObject()
    {

        SelectableObject s = new SelectableObject();
        //First, we pick the rarity
        int rarity = Random.Range(1,32);

        if (rarity <= 31 && rarity >= 16)
        {
            //Rolled a common
            s = GenerateAbility();
            s.rarity = Rarity.COMMON;
            
        }
        else if (rarity <= 15 && rarity >= 8)
        {
            //Rolled an uncommon
            s = GenerateAbility();
            s.rarity = Rarity.UNCOMMON;
        }
        else if (rarity <= 7 && rarity >= 4)
        {
            //Rolled a rare
            
            //50-50 chance of being an ability or a gear
            int AorG = Random.Range(0,2);
            if (AorG == 0)
            {
                //Ability
                s = GenerateAbility();
            }
            else if (AorG == 1)
            {
                //Gear
                int numTeeth = Random.Range(6,100);
                Gear g = new Gear (numTeeth, true);
                s = g;
            }
            else
            {
                Debug.LogError("This isnt a gear or an ability (what is it???)");
            }
            s.rarity = Rarity.RARE;
        }
        else if (rarity <= 3 && rarity >= 2)
        {
            //Rolled an epic
            //50-50 chance of being an ability or a gear
            int AorG = Random.Range(0,2);
            if (AorG == 0)
            {
                //Ability
                s = GenerateAbility();
                
            }
            else if (AorG == 1)
            {
                //Gear
                int numTeeth = Random.Range(6,100);
                Gear g = new Gear (numTeeth, true);
                s = g;
            }
            else
            {
                Debug.LogError("This isnt a gear or an ability (what is it???)");
            }
            s.rarity = Rarity.EPIC;
        }
        else if (rarity == 1)
        {
            //Rolled a mythic //always a 3 slot gear btw with >= 10 teeth
            int numTeeth = Random.Range(10, 100);
            Gear g = new Gear(numTeeth, true);
            s = g;
            s.rarity = Rarity.MYTHIC;
        }
        else
        {
            //Rolled an error
            Debug.LogError("BRUH!, we rolled a legendary (does not exist)");
        }

        return s;
    }

    Ability GenerateAbility()
    {
        int a = Random.Range(0,  System.Enum.GetNames(typeof(AbilityName)).Length);
        Ability ability = new Ability((AbilityName)a);
        return ability;
    }
}
