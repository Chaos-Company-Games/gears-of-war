using UnityEngine;
using UnityEngine.Events;

public class XPSystem : MonoBehaviour
{

    public static XPSystem instance {get; private set; }

    [Header("Solo Leveling")]
    public int baseXPRequired = 100; //Xp needed for lvl 2
    public float xpScalingFactor = 1.4f; //multiply and such

    private int currentLevel = 1;
    private float currentXP = 0f;
    private float xpToNextLevel;

    public UnityEvent<int> onLevelUp;
    public UnityEvent<float, float> onXPChanged; //current, required to lvl

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
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        onXPChanged?.Invoke(currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
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
}
