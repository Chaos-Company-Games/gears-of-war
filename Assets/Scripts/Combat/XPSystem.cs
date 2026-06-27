using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class XPSystem : MonoBehaviour
{

    public static XPSystem instance {get; private set; }

    [Header("Solo Leveling")]
    public int baseXPRequired = 100; //Xp needed for lvl 2
    public float xpScalingFactor = 1.4f; //multiply and such

    [SerializeField] private RawImage xpBar;
    private int currentLevel = 1;
    private float currentXP = 0f;
    private float xpToNextLevel;

    public UnityEvent<int> onLevelUp;
    public UnityEvent onXPChanged; //current, required to lvl

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

            xpToNextLevel = XPRequiredForLevel(currentLevel);
        }

        onXPChanged.AddListener(UpdateXPBar);
        onXPChanged.Invoke();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
        onXPChanged?.Invoke();
    }

    void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = XPRequiredForLevel(currentLevel);

        Debug.Log($"Level up! Frick yeah! Now level {currentLevel}, next level needs {xpToNextLevel} XP.");

        onLevelUp?.Invoke(currentLevel);

        //Freeze game while we pick 3
        //Time.timeScale = 0f;
        LevelUpMenu.instance?.Show(currentLevel);
    }

    float XPRequiredForLevel(int level)
    {
        //Exponential scaling
        return Mathf.Round(baseXPRequired * Mathf.Pow(xpScalingFactor, level - 1));
    }

    public int CurrentLevel => currentLevel;
    public float CurrentXP => currentXP;
    public float XPToNextLevel => xpToNextLevel;

    void UpdateXPBar()
    {
        float newWidth = currentXP/xpToNextLevel * 490f; //490 is the base size of the HP bar
        if (newWidth > 490) newWidth = 490f;
        xpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }
}
