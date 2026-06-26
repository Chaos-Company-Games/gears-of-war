using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Attach this to level up canvas UI, and disable it by default
public class LevelUpMenu : MonoBehaviour
{
    public static LevelUpMenu instance {get; private set; }

    [Header("UI References")]
    public GameObject menuPanel;
    public Button[] optionButtons; //3 buttons
    public TextMeshProUGUI[] optionLabels; //label to each button
    public TextMeshProUGUI levelLabel;

    //Placeholder option names, we change this later
    private string[] placeHolderOptions = {"Option A", "Option B", "Option C"};

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
            if (optionLabels[i] != null)
            {
                optionLabels[i].text = placeHolderOptions[i];
            }
        }

    }

    void OnOptionChosen(int index)
    {
        Debug.Log($"Player chose option {index} : {placeHolderOptions[index]}");

        //Apply upgrade here
        //For example: UpgradeManager.instance.Apply(chosenUpgrade);

        Close();
    }

    void Close()
    {
        menuPanel.SetActive(false);
        //Unpause game
        Time.timeScale = 1f;
    }
}
