using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Stats")]
    public float maxHp = 100f;

    private float currentHP;

//Make some events for flexibility sake
    public UnityEvent onDeath;
    public UnityEvent onHealthChanged; //current, max
    [SerializeField] private RawImage hpBar;
    void Start()
    {
        currentHP = maxHp;
        onHealthChanged.AddListener(UpdateHPBar);
        onHealthChanged?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        if (currentHP <= 0f) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        onHealthChanged?.Invoke();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHp, currentHP + amount);
        onHealthChanged?.Invoke();
    }

    void Die()
    {
        onDeath?.Invoke();
        Debug.Log("You lose!! idiot..");
        //game over screen tbd V
    }

    void UpdateHPBar()
    {
        float newWidth = currentHP/maxHp * 490f; //490 is the base size of the HP bar
        hpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }
}
